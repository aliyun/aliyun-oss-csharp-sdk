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
using Aliyun.OSS.Common.Internal;

namespace Aliyun.OSS
{
    internal class ResumableDownloadManager
    {
        private OssClient _ossClient;
        private int _maxRetryTimes;
        private ClientConfiguration _conf;
        private long _downloadedBytes;

        public ResumableDownloadManager(OssClient ossClient, int maxRetryTimes, ClientConfiguration conf)
        {
            this._ossClient = ossClient;
            this._maxRetryTimes = maxRetryTimes;
            this._conf = conf;
        }

        public void ResumableDownloadWithRetry(DownloadObjectRequest request, ResumableDownloadContext resumableContext)
        {
            for (int i = 0; i < _maxRetryTimes; i++)
            {
                try
                {
                    DoResumableDownload(request, resumableContext, request.StreamTransferProgress);
                    break;
                }
                catch(NoneRetryableException e)
                {
                    throw new OssException(e.ToString());
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

        private void DoResumableDownload(DownloadObjectRequest request, ResumableDownloadContext resumableContext,
                                       EventHandler<StreamTransferProgressArgs> downloadProgressCallback)
        {
            // use single thread if MaxResumableUploadThreads is no bigger than 1
            // or the part size is bigger than the conf.MaxPartCachingSize
            if (resumableContext.PartContextList[0].Length > _conf.MaxPartCachingSize || request.ParallelThreadCount <= 1)
            {
                DoResumableDownloadSingleThread(request, resumableContext, downloadProgressCallback);
            }
            else
            {
                // multi-threaded download the object and write the data
                DoResumableDownloadMultiThread(request, resumableContext, downloadProgressCallback);
            }
        }

        private void DoResumableDownloadSingleThread(DownloadObjectRequest request, ResumableDownloadContext resumableContext,
                                      EventHandler<StreamTransferProgressArgs> downloadProgressCallback)
        {
            _downloadedBytes = resumableContext.GetDownloadedBytes();
            if (!request.MatchingETagConstraints.Contains(resumableContext.ETag))
            {
                request.MatchingETagConstraints.Add(resumableContext.ETag);
            }

            long totalBytes = resumableContext.GetTotalBytes();
            foreach (var part in resumableContext.PartContextList)
            {
                if (part.IsCompleted)
                {
                    // is CRC is enabled and part.Crc64 is 0, then redownload the data
                    if (!_conf.EnableCrcCheck || part.Crc64 != 0)
                    {
                        continue;
                    }
                }

                using (Stream fs = File.Open(GetTempDownloadFile(request), FileMode.OpenOrCreate))
                {
                    fs.Seek(part.Position, SeekOrigin.Begin);
                    var originalStream = fs;
                    if (downloadProgressCallback != null)
                    {
                        originalStream = _ossClient.SetupDownloadProgressListeners(fs,
                                                                        totalBytes,
                                                                        _downloadedBytes,
                                                                        _conf.ProgressUpdateInterval,
                                                                        downloadProgressCallback);
                    }

                    if (_conf.EnableCrcCheck)
                    {
                        originalStream = new Crc64Stream(originalStream, null, part.Length);
                    }

                    var getPartRequest = request.ToGetObjectRequest();
                    getPartRequest.SetRange(part.Position, part.Length + part.Position - 1);

                    var partResult = _ossClient.GetObject(getPartRequest);
                    WriteTo(partResult.Content, originalStream);

                    if (originalStream is Crc64Stream)
                    {
                        Crc64Stream crcStream = originalStream as Crc64Stream;

                        if (crcStream.CalculatedHash == null)
                        {
                            crcStream.CalculateHash();
                        }

                        part.Crc64 = BitConverter.ToUInt64(crcStream.CalculatedHash, 0);
                    }
                }

                part.IsCompleted = true;
                resumableContext.Dump();
                _downloadedBytes += part.Length;
            }

            Validate(request, resumableContext);
        }

        internal static long WriteTo(Stream src, Stream dest)
        {
            var buffer = new byte[32 * 1024];
            var bytesRead = 0;
            var totalBytes = 0;
            while ((bytesRead = src.Read(buffer, 0, buffer.Length)) > 0)
            {
                dest.Write(buffer, 0, bytesRead);
                totalBytes += bytesRead;
            }
            dest.Flush();

            return totalBytes;
        }

        internal class DownloadTaskParam
        {
            public DownloadObjectRequest Request
            {
                get;
                set;
            }

            public ResumableDownloadContext Context
            {
                get;
                set;
            }

            public ResumablePartContext Part
            {
                get;
                set;
            }
            public EventHandler<StreamTransferProgressArgs> DownloadProgressCallback
            {
                get;
                set;
            }

            public Exception Error
            {
                get;
                set;
            }

            public ManualResetEvent DownloadFinished
            {
                get;
                set;
            }
        }

        private void DoResumableDownloadMultiThread(DownloadObjectRequest request, ResumableDownloadContext resumableContext,
                                                    EventHandler<StreamTransferProgressArgs> downloadProgressCallback)
        {
            _downloadedBytes = resumableContext.GetDownloadedBytes();
            if (!request.MatchingETagConstraints.Contains(resumableContext.ETag))
            {
                request.MatchingETagConstraints.Add(resumableContext.ETag);
            } 

            Exception e = null;
            int parallel = Math.Min(Math.Min(request.ParallelThreadCount, resumableContext.PartContextList.Count), Environment.ProcessorCount);
            ManualResetEvent[] taskFinishedEvents = new ManualResetEvent[parallel];
            DownloadTaskParam[] taskParams = new DownloadTaskParam[parallel];
            int nextPart = 0;
            for (nextPart = 0; nextPart < parallel;nextPart++)
            {
                var part = resumableContext.PartContextList[nextPart];
                taskParams[nextPart] = StartDownloadPartTask(request, part, downloadProgressCallback);
                taskFinishedEvents[nextPart] = taskParams[nextPart].DownloadFinished;
            }

            bool allTasksDone = false;

            try
            {
                long totalBytes = resumableContext.GetTotalBytes();
                long lastDownloadedBytes = _downloadedBytes;
                while (nextPart < resumableContext.PartContextList.Count)
                {
                    int index = ManualResetEvent.WaitAny(taskFinishedEvents);
                    if (taskParams[index].Error == null)
                    {
                        if (request.StreamTransferProgress != null)
                        {
                            lastDownloadedBytes = _downloadedBytes;
                            StreamTransferProgressArgs args = new StreamTransferProgressArgs(resumableContext.PartContextList[index].Length, lastDownloadedBytes, totalBytes);
                            request.StreamTransferProgress.Invoke(this, args);
                        }

                        resumableContext.Dump();
                    }
                    else
                    {
                        e = taskParams[index].Error;
                    }

                    taskFinishedEvents[index].Close();
                    taskParams[index] = StartDownloadPartTask(request, resumableContext.PartContextList[nextPart++], downloadProgressCallback);
                    taskFinishedEvents[index] = taskParams[index].DownloadFinished;
                }

                ManualResetEvent.WaitAll(taskFinishedEvents);
                allTasksDone = true;

                if (request.StreamTransferProgress != null)
                {
                    StreamTransferProgressArgs args = new StreamTransferProgressArgs(_downloadedBytes - lastDownloadedBytes, _downloadedBytes, totalBytes);
                    request.StreamTransferProgress.Invoke(this, args);
                }

                if (e == null)
                {
                    Validate(request, resumableContext);
                }
            }
            finally
            {
                if (!allTasksDone)
                {
                    ManualResetEvent.WaitAll(taskFinishedEvents);
                }

                for (int i = 0; i < parallel; i++)
                {
                    taskFinishedEvents[i].Close();
                    if (taskParams[i].Error != null)
                    {
                        e = taskParams[i].Error;
                    }
                }

                if (e != null)
                {
                    throw e;
                }
            }
        }

        private DownloadTaskParam StartDownloadPartTask(DownloadObjectRequest request, ResumablePartContext part, EventHandler<StreamTransferProgressArgs> downloadProgressCallback)
        {
            DownloadTaskParam taskParam = new DownloadTaskParam();
            taskParam.Request = request;
            taskParam.Part = part;
            taskParam.DownloadProgressCallback = downloadProgressCallback;
            taskParam.DownloadFinished = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(DownloadPart, taskParam);
            return taskParam;
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new ArgumentException("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        private void Validate(DownloadObjectRequest request, ResumableDownloadContext resumableContext)
        {
            if (_conf.EnalbeMD5Check && !string.IsNullOrEmpty(resumableContext.ContentMd5))
            {
                using (var fs = File.Open(GetTempDownloadFile(request), FileMode.Open))
                {
                    string calcuatedMd5 = OssUtils.ComputeContentMd5(fs, fs.Length);
                    if (calcuatedMd5 != resumableContext.ContentMd5)
                    {
                        throw new OssException(string.Format("The Md5 of the downloaded file {0} does not match the expected. Expected:{1}, actual:{2}",
                                                             GetTempDownloadFile(request),
                                                             resumableContext.ContentMd5,
                                                             calcuatedMd5
                                                            ));
                    }
                }
            }
            else if (_conf.EnableCrcCheck && !string.IsNullOrEmpty(resumableContext.Crc64))
            {
                ulong calculatedCrc = 0;
                foreach (var part in resumableContext.PartContextList)
                {
                    calculatedCrc = Crc64.Combine(calculatedCrc, part.Crc64, part.Length);
                }

                if (calculatedCrc.ToString() != resumableContext.Crc64)
                {
                    throw new OssException(string.Format("The Crc64 of the downloaded file {0} does not match the expected. Expected:{1}, actual:{2}",
                                                           GetTempDownloadFile(request),
                                                           resumableContext.Crc64,
                                                           calculatedCrc
                                                          ));
                }
            }

            File.Move(GetTempDownloadFile(request), request.DownloadFile);
        }

        private string GetTempDownloadFile(DownloadObjectRequest request)
        {
            return request.DownloadFile + ".tmp";
        }

        private void DownloadPart(object state)
        {
            DownloadTaskParam taskParam = state as DownloadTaskParam;
            if (taskParam == null)
            {
                throw new ClientException("Internal error. The taskParam should be type of DownloadTaskParam");
            }
            DownloadObjectRequest request = taskParam.Request;
            ResumablePartContext part = taskParam.Part;
            EventHandler<StreamTransferProgressArgs> downloadProgressCallback = taskParam.DownloadProgressCallback;

            try
            {
                string fileName = GetTempDownloadFile(request); 
                if (part.IsCompleted && File.Exists(fileName))
                {
                    // is CRC is enabled and part.Crc64 is 0, then redownload the data
                    if (!_conf.EnableCrcCheck || part.Crc64 != 0)
                    {
                        return;
                    }
                }

                const int retryCount = 3;
                for (int i = 0; i < retryCount; i++)
                {
                    try
                    {
                        GetObjectRequest partRequest = request.ToGetObjectRequest();
                        partRequest.SetRange(part.Position, part.Position + part.Length - 1);
                        using(var partResult = _ossClient.GetObject(partRequest))
                        {
                            Crc64Stream crcStream = null;
                            if (_conf.EnableCrcCheck)
                            {
                                crcStream = new Crc64Stream(partResult.Content, null, part.Length, 0);
                            }

                            using(var fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                fs.Seek(part.Position, SeekOrigin.Begin);

                                long totalBytes = WriteTo(crcStream ?? partResult.Content, fs);
                                if (totalBytes != part.Length)
                                {
                                    throw new OssException(string.Format("Part {0} returns {1} bytes. Expected size is {2} bytes", 
                                                           part.PartId, totalBytes, part.Length));
                                }
                                Interlocked.Add(ref _downloadedBytes, partResult.ContentLength);
                            }

                            part.IsCompleted = true;
                            if (crcStream != null)
                            {
                                if (crcStream.CalculatedHash == null)
                                {
                                    crcStream.CalculateHash();
                                }
                                part.Crc64 = BitConverter.ToUInt64(crcStream.CalculatedHash, 0);
                            }

                            return;
                        }
                    }
                    catch (Exception ex) // when the connection is closed while sending the data, it will run into ObjectDisposedException.
                    {
                        if (!(ex is ObjectDisposedException || ex is WebException) || i == retryCount - 1)
                        {
                            throw;
                        }
                    }
                }

                throw new ClientException("DownloadPart runs into internal error");
            }
            catch(Exception e)
            {
                taskParam.Error = e;
            }
            finally
            {
                taskParam.DownloadFinished.Set();
            }
        }

        class NoneRetryableException : Exception
        {
            public NoneRetryableException(string msg) : base(msg)
            {}
        }
    }
}
