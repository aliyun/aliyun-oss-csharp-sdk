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
        public ResumableUploadManager(OssClient ossClient, int maxRetryTimes, ClientConfiguration conf)
        {
            this._ossClient = ossClient;
            this._maxRetryTimes = maxRetryTimes;
            this._conf = conf;
        }

        public void ResumableUploadWithRetry(string bucketName, string key, Stream content, ResumableContext resumableContext,
                                              EventHandler<StreamTransferProgressArgs> uploadProgressCallback)
        {
            using (var fs = content)
            {
                for (int i = 0; i < _maxRetryTimes; i++)
                {
                    try
                    {
                        DoResumableUpload(bucketName, key, resumableContext, fs, uploadProgressCallback);
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
            if (resumableContext.PartContextList[0].Length > _conf.MaxPartCachingSize || _conf.MaxResumableUploadThreads <= 1)
            {
                DoResumableUploadSingleThread(bucketName, key, resumableContext, fs, uploadProgressCallback);
            }
            else
            {
                DoResumableUploadMultiThread(bucketName, key, resumableContext, fs, uploadProgressCallback);
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
                    PartNumber = part.PartId
                };

                var partResult = _ossClient.UploadPart(request);
                part.PartETag = partResult.PartETag;
                part.IsCompleted = true;
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

            private Queue<UploadTaskParam> Queue = new Queue<UploadTaskParam>();

            private Queue<MemoryStream> BufferPool = new Queue<MemoryStream>();

            private object _bufferLock = new object();

            private object _taskLock = new object();

            private ManualResetEvent _taskAvailable = new ManualResetEvent(false);
            private ManualResetEvent _bufferAvailable = new ManualResetEvent(false);

            public MemoryStream TakeBuffer()
            {
                if (!_bufferAvailable.WaitOne(1000 * 10))
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

            public UploadTaskParam TakeTask()
            {
                if (!_taskAvailable.WaitOne(1000 * 10))
                {
                    return null;
                }

                lock(_taskLock)
                {
                    UploadTaskParam param = Queue.Dequeue();
                    if (Queue.Count == 0)
                    {
                        _taskAvailable.Reset();
                    }

                    return param;
                }
            }

            public void CreateTask(UploadTaskParam task)
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
                while (nextPart < preReadThreadParam.ResumableContext.PartContextList.Count)
                {
                    MemoryStream buffer = preReadThreadParam.TakeBuffer();
                    if (buffer == null)
                    {
                        continue;
                    }

                    UploadTaskParam param = new UploadTaskParam();
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
                        int count = preReadThreadParam.Fs.Read(param.InputStream.GetBuffer(), readCount, (int)param.ResumableUploadPartContext.Length - readCount);
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

        private void DoResumableUploadMultiThread(string bucketName, string key, ResumableContext resumableContext, Stream fs,
                                       EventHandler<StreamTransferProgressArgs> uploadProgressCallback)
        {
            _uploadProgressCallback = uploadProgressCallback;
            _uploadedBytes = resumableContext.GetUploadedBytes();
            _totalBytes = fs.Length;
            _incrementalUploadedBytes = 0;

            Exception e = null;
            int parallel = Math.Min(_conf.MaxResumableUploadThreads, resumableContext.PartContextList.Count);
            Queue<UploadTaskParam> preReadItems = new Queue<UploadTaskParam>();

            int preReadPartCount = Math.Min(parallel, _conf.PreReadBufferCount) + parallel;

            ManualResetEvent[] taskFinishEvents = new ManualResetEvent[parallel];
            UploadTaskParam[] runningTasks = new UploadTaskParam[parallel];
            Console.WriteLine("Starting {0} Thread. Total Parts:{1}", parallel, resumableContext.PartContextList.Count);
            fs.Seek(0, SeekOrigin.Begin);

            // init the buffer pool
            PreReadThreadParam param = new PreReadThreadParam();
            param.Fs = fs;
            param.ResumableContext = resumableContext;
            for (int i = 0; i < preReadPartCount && i < resumableContext.PartContextList.Count; i++)
            {
                var part = resumableContext.PartContextList[i];
                param.ReturnBuffer(new MemoryStream((int)part.Length));
            }

            Thread thread = new Thread(new ParameterizedThreadStart(StartPreRead)); 
            thread.Start(param);

            bool allTaskDone = false;
            for (int i = 0; i < parallel; i++)
            {
                UploadTaskParam task = param.TakeTask();
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
                        runningTasks[index].Finished.Close();
                    }
                    else
                    {
                        e = runningTasks[index].Error;
                    }

                    param.ReturnBuffer(runningTasks[index].InputStream);
                    UploadTaskParam task = param.TakeTask();
                    if (task == null)
                    {
                        waitingCount++;
                        if (param.PreReadError != null) // no more task will be created;
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

                if (param.PreReadError != null)
                {
                    e = param.PreReadError;
                }

                MemoryStream buffer = null;
                while (param.GetBufferLength() != 0 && (buffer = param.TakeBuffer()) != null)
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

        internal class UploadTaskParam
        {
            public Stream UploadFileStream
            {
                get;
                set;
            }

            public MemoryStream InputStream
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
        private void StartUploadPartTask(UploadTaskParam taskParam)
        {           
            ThreadPool.QueueUserWorkItem(UploadPart, taskParam);
        }

        private void UploadPart(object state) 
        {
            UploadTaskParam taskParam = state as UploadTaskParam;
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
                    stream.Seek(0, SeekOrigin.Begin);
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
                            PartNumber = part.PartId
                        };

                        var partResult = _ossClient.UploadPart(request);
                        part.PartETag = partResult.PartETag;
                        part.IsCompleted = true;
                        break;
                    }
                    catch (Exception ex) // when the connection is closed while sending the data, it will run into ObjectDisposedException.
                    {
                        if ((ex is ObjectDisposedException || ex is WebException) && i != retryCount - 1)
                        {
                            Console.WriteLine(string.Format("Retry:{0}. UploadPart fails:{1}", i, ex.ToString()));
                        }
                        else
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
