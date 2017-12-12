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

        private void DoResumableUploadMultiThread(string bucketName, string key, ResumableContext resumableContext, Stream fs,
                                       EventHandler<StreamTransferProgressArgs> uploadProgressCallback)
        {

            var uploadedBytes = resumableContext.GetUploadedBytes();

            Exception e = null;
            int parallel = Math.Min(Math.Min(_conf.MaxResumableUploadThreads, resumableContext.PartContextList.Count), Environment.ProcessorCount);
            MemoryStream[] mss = new MemoryStream[parallel];
            MemoryStream preReadBuffer = new MemoryStream((int)resumableContext.PartContextList[0].Length);
            ManualResetEvent[] taskFinishEvents = new ManualResetEvent[parallel];
            UploadTaskParam[] taskParams = new UploadTaskParam[parallel];
            Console.WriteLine("Starting {0} Thread. Total Parts:{1}", parallel, resumableContext.PartContextList.Count);
            fs.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < parallel; i++)
            {
                var part = resumableContext.PartContextList[i];
                mss[i] = new MemoryStream((int)part.Length);
                taskFinishEvents[i] = new ManualResetEvent(false);
                UploadTaskParam param = new UploadTaskParam();
                taskParams[i] = param;
                param.UploadFileStream = fs;
                param.InputStream = mss[i];
                param.ResumableUploadContext = resumableContext;
                param.ResumableUploadPartContext = resumableContext.PartContextList[i];
                param.UploadProgressCallback = uploadProgressCallback;
                param.ProgressUpdateInterval = _conf.ProgressUpdateInterval;
                param.uploadedBytes = uploadedBytes;
                param.Finished = taskFinishEvents[i];
                CreateUploadPartTask(param);
            }

            bool allTaskDone = false;
            int nextPart = parallel;
            try
            {
                while (nextPart < resumableContext.PartContextList.Count)
                {
                    UploadTaskParam newTaskParam = new UploadTaskParam();
                    newTaskParam.UploadFileStream = fs;
                    newTaskParam.InputStream = preReadBuffer;
                    newTaskParam.ResumableUploadContext = resumableContext;
                    newTaskParam.ResumableUploadPartContext = resumableContext.PartContextList[nextPart++];
                    newTaskParam.UploadProgressCallback = uploadProgressCallback;
                    newTaskParam.ProgressUpdateInterval = _conf.ProgressUpdateInterval;
                    newTaskParam.uploadedBytes = uploadedBytes;
                    newTaskParam.Finished = new ManualResetEvent(false);

                    CreateUploadPartTask(newTaskParam);
                    int index = ManualResetEvent.WaitAny(taskFinishEvents);
                    if (taskParams[index].Error == null)
                    {
                        resumableContext.Dump();
                        uploadedBytes += resumableContext.PartContextList[index].Length;
                        taskParams[index].Finished.Close();
                    }
                    else
                    {
                        e = taskParams[index].Error;
                    }

                    taskParams[index] = newTaskParam;
                    taskFinishEvents[index] = newTaskParam.Finished; 
                    var t = mss[index];
                    mss[index] = preReadBuffer;
                    preReadBuffer = t;
                }

                WaitHandle.WaitAll(taskFinishEvents);
                allTaskDone = true;
            }
            finally
            {
                preReadBuffer.Dispose();
                if (!allTaskDone)
                {
                    WaitHandle.WaitAll(taskFinishEvents);
                }

                for (int i = 0; i < parallel; i++)
                {
                    taskFinishEvents[i].Close();
                    mss[i].Dispose();
                    if (taskParams[i].Error != null)
                    {
                        e = taskParams[i].Error;
                    }
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

            public long uploadedBytes
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
        private void CreateUploadPartTask(UploadTaskParam taskParam)
        {
            int readCount = 0;
            while (readCount != taskParam.ResumableUploadPartContext.Length)
            {
                int count = taskParam.UploadFileStream.Read(taskParam.InputStream.GetBuffer(), readCount, (int)taskParam.ResumableUploadPartContext.Length - readCount);
                if (count == 0)
                {
                    throw new System.IO.IOException(string.Format("Unable to read data with expected size. Expected size:{0}, actual read size: {1}", taskParam.ResumableUploadPartContext.Length, readCount));
                }
                readCount += count;
            }

            taskParam.InputStream.SetLength(readCount);
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
                                                                             stream.Length,
                                                                             taskParam.uploadedBytes,
                                                                             taskParam.ProgressUpdateInterval,
                                                                             taskParam.UploadProgressCallback);
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
