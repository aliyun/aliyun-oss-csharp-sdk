/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */


using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;

using Aliyun.OSS.Domain;
using Aliyun.OSS.Commands;
using Aliyun.OSS.Util;
using Aliyun.OSS.Common;

namespace Aliyun.OSS
{
    internal class ResumableUploadManager
    {
        private OssClient _ossClient;
        private int _maxRetryTimes;
        private ClientConfiguration _conf;
        private long _uploadedBytes;
        private long _totalBytes;
        private EventHandler<StreamTransferProgressArgs> _uploadProgressCallback;
        private long _incrementalUploadedBytes;
        private object _callbackLock = new object();
        private UploadObjectRequest _request;

        public ResumableUploadManager(OssClient ossClient, int maxRetryTimes, ClientConfiguration conf)
        {
            this._ossClient = ossClient;
            this._maxRetryTimes = maxRetryTimes;
            this._conf = conf;
        }

        public void ResumableUploadWithRetry(UploadObjectRequest request, ResumableContext resumableContext)
        {
            _request = request;
            using (Stream fs = request.UploadStream ?? new FileStream(request.UploadFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                for (int i = 0; i < _maxRetryTimes; i++)
                {
                    try
                    {
                        DoResumableUpload(request.BucketName, request.Key, resumableContext, fs, request.StreamTransferProgress);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i != _maxRetryTimes - 1)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                }
            }
        }

        private void DoResumableUpload(string bucketName, string key, ResumableContext resumableContext, Stream fs,
                                       EventHandler<StreamTransferProgressArgs> uploadProgressCallback)
        {
            bool isFileStream = fs is FileStream;

            // use single thread if MaxResumableUploadThreads is no bigger than 1
            // or when the stream is not file stream and the part size is bigger than the conf.MaxPartCachingSize
            if (_request.ParallelThreadCount <= 1 || (!isFileStream || _conf.UseSingleThreadReadInResumableUpload) && resumableContext.PartContextList[0].Length > _conf.MaxPartCachingSize)
            {
                DoResumableUploadSingleThread(bucketName, key, resumableContext, fs, uploadProgressCallback);
            }
            else if (isFileStream && !_conf.UseSingleThreadReadInResumableUpload)
            {
                // multi-threaded read file and send the data
                DoResumableUploadFileMultiThread(bucketName, key, resumableContext, fs as FileStream, uploadProgressCallback);
            }
            else
            {
                // single thread pre-read the data and multi-thread send the data.
                DoResumableUploadPreReadMultiThread(bucketName, key, resumableContext, fs, uploadProgressCallback);
            }
        }

        private void DoResumableUploadSingleThread(string bucketName, string key, ResumableContext resumableContext, Stream fs,
                                                   EventHandler<StreamTransferProgressArgs> uploadProgressCallback)
        {
            var uploadedBytes = resumableContext.GetUploadedBytes();

            foreach (var part in resumableContext.PartContextList)
            {
                if (part.IsCompleted)
                {
                    continue;
                }

                fs.Seek(part.Position, SeekOrigin.Begin);
                var originalStream = fs;
                if (uploadProgressCallback != null)
                {
                    originalStream = _ossClient.SetupProgressListeners(originalStream,
                                                                     fs.Length,
                                                                     uploadedBytes,
                                                                     _conf.ProgressUpdateInterval,
                                                                     uploadProgressCallback);
                }

                var request = new UploadPartRequest(bucketName, key, resumableContext.UploadId)
                {
                    InputStream = originalStream,
                    PartSize = part.Length,
                    PartNumber = part.PartId,
                    RequestPayer = _request.RequestPayer,
                    TrafficLimit = _request.TrafficLimit
                };

                var partResult = _ossClient.UploadPart(request);
                part.PartETag = partResult.PartETag;
                part.IsCompleted = true;
                if (partResult.ResponseMetadata.ContainsKey(HttpHeaders.HashCrc64Ecma))
                {
                    part.Crc64 = ulong.Parse(partResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma]);
                }
                resumableContext.Dump();
                uploadedBytes += part.Length;
            }
        }

        private void ProcessCallbackInternal(object sender, StreamTransferProgressArgs e)
        {
            Interlocked.Add(ref _uploadedBytes, e.IncrementTransferred);
            Interlocked.Add(ref _incrementalUploadedBytes, e.IncrementTransferred);
            if (_uploadProgressCallback != null && _incrementalUploadedBytes >= _conf.ProgressUpdateInterval)
            {
                lock (this._callbackLock)
                {
                    if (_incrementalUploadedBytes >= _conf.ProgressUpdateInterval)
                    {
                        long incrementalUploadedBytes = _incrementalUploadedBytes;
                        StreamTransferProgressArgs progress = new StreamTransferProgressArgs(incrementalUploadedBytes, _uploadedBytes, _totalBytes);
                        _uploadProgressCallback.Invoke(this, progress);
                        Interlocked.Add(ref _incrementalUploadedBytes, -incrementalUploadedBytes);
                    }
                }
            }
        }

        internal class PreReadThreadParam
        {
            public Stream Fs;

            public ResumableContext ResumableContext;

            public Exception PreReadError;

            private Queue<UploadTask> Queue = new Queue<UploadTask>();

            private Queue<MemoryStream> BufferPool = new Queue<MemoryStream>();

            private object _bufferLock = new object();

            private object _taskLock = new object();

            private ManualResetEvent _taskAvailable = new ManualResetEvent(false);
            private ManualResetEvent _bufferAvailable = new ManualResetEvent(false);

            public MemoryStream TakeBuffer()
            {
                if (!_bufferAvailable.WaitOne(1000))
                {
                    return null;
                }
               
                lock(_bufferLock)
                {
                    MemoryStream buffer = BufferPool.Dequeue();
                    if (BufferPool.Count == 0)
                    {
                        _bufferAvailable.Reset();
                    }

                    return buffer;
                }
            }

            public void ReturnBuffer(MemoryStream ms)
            {
                lock(_bufferLock)
                {
                    BufferPool.Enqueue(ms);
                    if (BufferPool.Count == 1)
                    {
                        _bufferAvailable.Set();
                    }
                }
            }

            public UploadTask TakeTask()
            {
                if (!_taskAvailable.WaitOne(1000 * 10))
                {
                    return null;
                }

                lock(_taskLock)
                {
                    UploadTask param = Queue.Dequeue();
                    if (Queue.Count == 0)
                    {
                        _taskAvailable.Reset();
                    }

                    return param;
                }
            }

            public void CreateTask(UploadTask task)
            {
                lock(_taskLock)
                {
                    Queue.Enqueue(task);
                    if (Queue.Count == 1)
                    {
                        _taskAvailable.Set();
                    }
                }
            }

            public int GetTaskLength()
            {
                return Queue.Count;
            }

            public int GetBufferLength()
            {
                return BufferPool.Count;
            }

            public bool RequestStopPreRead
            {
                get;
                set;
            }
        }

        private void StartPreRead(object state)
        {
            PreReadThreadParam preReadThreadParam = state as PreReadThreadParam;
            if (preReadThreadParam == null)
            {
                throw new ClientException("Internal error: the state must be type of PreReadThreadParam");
            }

            int nextPart = 0;
            try
            {
                while (!preReadThreadParam.RequestStopPreRead && nextPart < preReadThreadParam.ResumableContext.PartContextList.Count)
                {
                    MemoryStream buffer = preReadThreadParam.TakeBuffer();
                    if (buffer == null)
                    {
                        continue;
                    }

                    UploadTask param = new UploadTask();
                    param.UploadFileStream = preReadThreadParam.Fs;
                    param.InputStream = buffer;
                    param.ResumableUploadContext = preReadThreadParam.ResumableContext;
                    param.ResumableUploadPartContext = preReadThreadParam.ResumableContext.PartContextList[nextPart++];
                    param.UploadProgressCallback = _uploadProgressCallback;
                    param.ProgressUpdateInterval = _conf.ProgressUpdateInterval;
                    param.Finished = new ManualResetEvent(false);

                    int readCount = 0;
                    while (readCount != param.ResumableUploadPartContext.Length)
                    {
                        int count = preReadThreadParam.Fs.Read(buffer.GetBuffer(), readCount, (int)param.ResumableUploadPartContext.Length - readCount);
                        if (count == 0)
                        {
                            throw new System.IO.IOException(string.Format("Unable to read data with expected size. Expected size:{0}, actual read size: {1}", param.ResumableUploadPartContext.Length, readCount));
                        }
                        readCount += count;
                    }

                    param.InputStream.SetLength(readCount);

                    preReadThreadParam.CreateTask(param);
                }
            }
            catch(Exception e)
            {
                preReadThreadParam.PreReadError = e;
            }
        }

        private UploadTask CreateTask(int i, ResumableContext resumableContext, FileStream fs)
        {
            UploadTask param = new UploadTask();
            param.UploadFileStream = fs;
            param.InputStream = new FileStream(fs.Name, FileMode.Open, FileAccess.Read, FileShare.Read);
            param.ResumableUploadContext = resumableContext;
            param.ResumableUploadPartContext = resumableContext.PartContextList[i];
            param.UploadProgressCallback = _uploadProgressCallback;
            param.ProgressUpdateInterval = _conf.ProgressUpdateInterval;
            param.Finished = new ManualResetEvent(false);
            return param;
        }

        /// <summary>
        /// Do the resumable upload with multithread from file stream.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="key">Key.</param>
        /// <param name="resumableContext">Resumable context.</param>
        /// <param name="fs">Fs.</param>
        /// <param name="uploadProgressCallback">Upload progress callback.</param>
        private void DoResumableUploadFileMultiThread(string bucketName, string key, ResumableContext resumableContext, FileStream fs,
                                   EventHandler<StreamTransferProgressArgs> uploadProgressCallback)
        {
            _uploadProgressCallback = uploadProgressCallback;
            _uploadedBytes = resumableContext.GetUploadedBytes();
            _totalBytes = fs.Length;
            _incrementalUploadedBytes = 0;

            Exception e = null;
            int parallel = Math.Min(_request.ParallelThreadCount, resumableContext.PartContextList.Count);

            ManualResetEvent[] taskFinishEvents = new ManualResetEvent[parallel];
            UploadTask[] runningTasks = new UploadTask[parallel];
            fs.Seek(0, SeekOrigin.Begin);

            bool allTaskDone = false;
            for (int i = 0; i < parallel; i++)
            {
                UploadTask param = CreateTask(i, resumableContext, fs);
                taskFinishEvents[i] = param.Finished;
                runningTasks[i] = param;
                StartUploadPartTask(param);
            }

            int nextPart = parallel;
            try
            {
                while (nextPart < resumableContext.PartContextList.Count)
                {
                    int index = ManualResetEvent.WaitAny(taskFinishEvents);
                    if (runningTasks[index].Error == null)
                    {
                        resumableContext.Dump();
                    }
                    else
                    {
                        e = runningTasks[index].Error;
                    }

                    runningTasks[index].Finished.Close();
                    runningTasks[index].InputStream.Dispose();
                    UploadTask task = CreateTask(nextPart, resumableContext, fs);
                    StartUploadPartTask(task);
                    runningTasks[index] = task;
                    taskFinishEvents[index] = runningTasks[index].Finished;
                    nextPart++;
                }

                WaitHandle.WaitAll(taskFinishEvents);
                allTaskDone = true;
            }
            finally
            {
                if (!allTaskDone)
                {
                    WaitHandle.WaitAll(taskFinishEvents);
                }

                if (uploadProgressCallback != null)
                {
                    long latestUploadedBytes = resumableContext.GetUploadedBytes();
                    long lastIncrementalUploadedBytes = latestUploadedBytes - _uploadedBytes + _incrementalUploadedBytes;
                    if (lastIncrementalUploadedBytes > 0)
                    {
                        StreamTransferProgressArgs progress = new StreamTransferProgressArgs(lastIncrementalUploadedBytes, latestUploadedBytes, fs.Length);
                        uploadProgressCallback.Invoke(this, progress);
                    }

                    _uploadedBytes = latestUploadedBytes;
                }

                for (int i = 0; i < parallel; i++)
                {
                    taskFinishEvents[i].Close();
                    if (runningTasks[i].Error != null)
                    {
                        e = runningTasks[i].Error;
                    }
                    runningTasks[i].InputStream.Dispose();
                }

                resumableContext.Dump();
                if (e != null)
                {
                    throw e;
                }
            }

        }

        /// <summary>
        /// Do the resumable upload with multithread from non file stream
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="key">Key.</param>
        /// <param name="resumableContext">Resumable context.</param>
        /// <param name="fs">Fs.</param>
        /// <param name="uploadProgressCallback">Upload progress callback.</param>
        private void DoResumableUploadPreReadMultiThread(string bucketName, string key, ResumableContext resumableContext, Stream fs,
                                       EventHandler<StreamTransferProgressArgs> uploadProgressCallback)
        {
            _uploadProgressCallback = uploadProgressCallback;
            _uploadedBytes = resumableContext.GetUploadedBytes();
            _totalBytes = fs.Length;
            _incrementalUploadedBytes = 0;

            Exception e = null;
            int parallel = Math.Min(_request.ParallelThreadCount, resumableContext.PartContextList.Count);

            int preReadPartCount = Math.Min(parallel, _conf.PreReadBufferCount) + parallel;

            ManualResetEvent[] taskFinishEvents = new ManualResetEvent[parallel];
            UploadTask[] runningTasks = new UploadTask[parallel];
            fs.Seek(0, SeekOrigin.Begin);

            // init the buffer pool
            PreReadThreadParam preReadParam = new PreReadThreadParam();
            preReadParam.Fs = fs;
            preReadParam.ResumableContext = resumableContext;
            for (int i = 0; i < preReadPartCount && i < resumableContext.PartContextList.Count; i++)
            {
                var part = resumableContext.PartContextList[i];
                preReadParam.ReturnBuffer(new MemoryStream((int)part.Length));
            }

            Thread thread = new Thread(new ParameterizedThreadStart(StartPreRead)); 
            thread.Start(preReadParam);

            bool allTaskDone = false;
            for (int i = 0; i < parallel; i++)
            {
                UploadTask task = preReadParam.TakeTask();
                if (task == null)
                {
                    continue;
                }
                taskFinishEvents[i] = task.Finished;
                runningTasks[i] = task;
                StartUploadPartTask(task);
            }

            int nextPart = parallel;
            try
            {
                int waitingCount = 0;
                const int MaxWaitingCount = 100;
                while (nextPart < resumableContext.PartContextList.Count && waitingCount < MaxWaitingCount)
                {
                    int index = ManualResetEvent.WaitAny(taskFinishEvents);
                    if (runningTasks[index].Error == null)
                    {
                        resumableContext.Dump();  
                    }
                    else
                    {
                        e = runningTasks[index].Error;
                    }

                    runningTasks[index].Finished.Close();
                    preReadParam.ReturnBuffer(runningTasks[index].InputStream as MemoryStream);
                    UploadTask task = preReadParam.TakeTask();
                    if (task == null)
                    {
                        waitingCount++;
                        if (preReadParam.PreReadError != null) // no more task will be created;
                        {
                            break;
                        }

                        continue;
                    }
                    StartUploadPartTask(task);
                    runningTasks[index] = task;
                    taskFinishEvents[index] = runningTasks[index].Finished; 
                    nextPart++;
                }

                if (waitingCount >= MaxWaitingCount)
                {
                    e = new ClientException("Fail to read the data from local stream");
                }

                WaitHandle.WaitAll(taskFinishEvents);
                allTaskDone = true;
            }
            finally
            {
                preReadParam.RequestStopPreRead = true;
                if (!allTaskDone)
                {
                    WaitHandle.WaitAll(taskFinishEvents);
                }

                if (uploadProgressCallback != null)
                {
                    long latestUploadedBytes = resumableContext.GetUploadedBytes();
                    long lastIncrementalUploadedBytes = latestUploadedBytes - _uploadedBytes + _incrementalUploadedBytes;
                    if (lastIncrementalUploadedBytes > 0)
                    {
                        StreamTransferProgressArgs progress = new StreamTransferProgressArgs(lastIncrementalUploadedBytes, latestUploadedBytes, fs.Length);
                        uploadProgressCallback.Invoke(this, progress);
                    }

                    _uploadedBytes = latestUploadedBytes;
                }

                for (int i = 0; i < parallel; i++)
                {
                    if (runningTasks[i].Error != null)
                    {
                        e = runningTasks[i].Error;
                    }
                    runningTasks[i].Finished.Close();
                }

                if (preReadParam.PreReadError != null)
                {
                    e = preReadParam.PreReadError;
                }

                MemoryStream buffer = null;
                while (preReadParam.GetBufferLength() != 0 && (buffer = preReadParam.TakeBuffer()) != null)
                {
                    buffer.Dispose();
                }

                resumableContext.Dump();
                if (e != null)
                {
                    throw e;
                }
            }
        }

        internal class UploadTask
        {
            public Stream UploadFileStream
            {
                get;
                set;
            }

            public Stream InputStream
            {
                get;
                set;
            }

            public ResumableContext ResumableUploadContext
            {
                get;
                set;
            }

            public ResumablePartContext ResumableUploadPartContext
            {
                get;
                set;
            }

            public EventHandler<StreamTransferProgressArgs> UploadProgressCallback
            {
                get;
                set;
            }

            public long ProgressUpdateInterval
            {
                get;
                set;
            }

            public ManualResetEvent Finished
            {
                get;
                set;
            }

            public Exception Error
            {
                get;
                set;
            }
        }

        private void StartUploadPartTask(UploadTask taskParam)
        {           
            ThreadPool.QueueUserWorkItem(UploadPart, taskParam);
        }

        private void UploadPart(object state) 
        {
            UploadTask taskParam = state as UploadTask;
            if (taskParam == null)
            {
                throw new ClientException("Internal error. The state object should be an instance of class UploadTaskParam");
            }

            try
            {
                ResumablePartContext part = taskParam.ResumableUploadPartContext;
                if (part.IsCompleted)
                {
                    return;
                }

                const int retryCount = 3;
                Stream stream = taskParam.InputStream;
                for (int i = 0; i < retryCount; i++)
                {
                    if (stream is FileStream)
                    {
                        stream.Seek(part.Position, SeekOrigin.Begin);
                    }
                    else
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }

                    Stream progressCallbackStream = null;
                    try
                    {
                        if (taskParam.UploadProgressCallback != null)
                        {
                            progressCallbackStream = _ossClient.SetupProgressListeners(stream,
                                                                             taskParam.UploadFileStream.Length, // does not matter
                                                                             _uploadedBytes,  // does not matter
                                                                             Math.Min(taskParam.ProgressUpdateInterval, 1024 * 4),
                                                                             this.ProcessCallbackInternal);
                        }

                        var request = new UploadPartRequest(taskParam.ResumableUploadContext.BucketName, taskParam.ResumableUploadContext.Key, taskParam.ResumableUploadContext.UploadId)
                        {
                            InputStream = progressCallbackStream ?? stream,
                            PartSize = part.Length,
                            PartNumber = part.PartId,
                            RequestPayer = _request.RequestPayer,
                            TrafficLimit = _request.TrafficLimit
                        };

                        var partResult = _ossClient.UploadPart(request);
                        part.PartETag = partResult.PartETag;
                        if (partResult.ResponseMetadata.ContainsKey(HttpHeaders.HashCrc64Ecma))
                        {
                            part.Crc64 = ulong.Parse(partResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma]);
                        }

                        part.IsCompleted = true;
                        break;
                    }
                    catch (Exception ex) // when the connection is closed while sending the data, it will run into ObjectDisposedException.
                    {
                        if (!(ex is ObjectDisposedException || ex is WebException) || i == retryCount - 1)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        if (progressCallbackStream != null)
                        {
                            progressCallbackStream.Dispose();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                taskParam.Error = e;
            }
            finally
            {
                taskParam.Finished.Set();
            }
        }
    }
}
