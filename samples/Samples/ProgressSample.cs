/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Text;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;
using System.Threading;
using System.Collections.Generic;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for progress.
    /// </summary>
    public static class ProgressSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string fileToUpload = Config.FileToUpload;

        static AutoResetEvent _event = new AutoResetEvent(false);

        public static void Progress(string bucketName)
        {
            PutObjectProgress(bucketName);
            AsyncPutObjectProgress(bucketName);
            AppendObjectProgress(bucketName);
            AsyncAppendObjectProgress(bucketName);
            MultipartUploadProgress(bucketName);
            ResumableUploadProgress(bucketName);
            GetObjectProgress(bucketName);
            AsyncGetObjectProgress(bucketName);
        }
    
        #region put object
        public static void PutObjectProgress(string bucketName)
        {
            const string key = "PutObjectProgress";
            try
            {
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var putObjectRequest = new PutObjectRequest(bucketName, key, fs);
                    putObjectRequest.StreamTransferProgress += streamProgressCallback;
                    client.PutObject(putObjectRequest);
                }
                Console.WriteLine("Put object:{0} succeeded", key);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}", 
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }

        public static void AsyncPutObjectProgress(string bucketName)
        {
            const string key = "AsyncPutObjectProgress";
            try
            {
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var putObjectRequest = new PutObjectRequest(bucketName, key, fs);
                    putObjectRequest.StreamTransferProgress += streamProgressCallback;

                    string result = "Notice user: put object finish";
                    client.BeginPutObject(putObjectRequest, PutObjectCallback, result.ToCharArray());

                    _event.WaitOne();
                }
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }
        #endregion

        #region append object
        public static void AppendObjectProgress(string bucketName)
        {
            const string key = "AppendObjectProgress";
            long position = 0;

            try
            {
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var request = new AppendObjectRequest(bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position
                    };
                    request.StreamTransferProgress += streamProgressCallback;

                    var result = client.AppendObject(request);
                    position = result.NextAppendPosition;

                    Console.WriteLine("Append object succeeded, next append position:{0}", position);
                }
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
            finally
            {
                client.DeleteObject(bucketName, key);
            }
        }

        public static void AsyncAppendObjectProgress(string bucketName)
        {
            const string key = "AsyncAppendObjectProgress";
            long position = 0;

            try
            {
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var request = new AppendObjectRequest(bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position
                    };
                    request.StreamTransferProgress += streamProgressCallback;

                    const string notice = "Append object succeeded";
                    client.BeginAppendObject(request, AppendObjectCallback, notice.Clone());

                    _event.WaitOne();
                }
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
            finally
            {
                client.DeleteObject(bucketName, key);
            }
        }
        #endregion

        #region multipart upload
        public static void MultipartUploadProgress(string bucketName)
        {
            const string key = "MultipartUploadProgress";

            try
            {
                // 初始化分片上传任务
                var initRequest = new InitiateMultipartUploadRequest(bucketName, key);
                var initResult = client.InitiateMultipartUpload(initRequest);

                // 设置每块为 1M
                const int partSize = 1024 * 1024 * 1;
                var partFile = new FileInfo(Config.FileToUpload);
                // 计算分块数目
                var partCount = CalculatePartCount(partFile.Length, partSize);

                // 新建一个List保存每个分块上传后的ETag和PartNumber
                var partETags = new List<PartETag>();
                //upload the file
                using (var fs = new FileStream(partFile.FullName, FileMode.Open))
                {
                    for (var i = 0; i < partCount; i++)
                    {
                        // 跳到每个分块的开头
                        long skipBytes = partSize * i;
                        fs.Position = skipBytes;

                        // 计算每个分块的大小
                        var size = partSize < partFile.Length - skipBytes ? partSize : partFile.Length - skipBytes;

                        // 创建UploadPartRequest，上传分块
                        var uploadPartRequest = new UploadPartRequest(bucketName, key, initResult.UploadId)
                        {
                            InputStream = fs,
                            PartSize = size,
                            PartNumber = (i + 1)
                        };
                        uploadPartRequest.StreamTransferProgress += streamProgressCallback;
                        var uploadPartResult = client.UploadPart(uploadPartRequest);

                        // 将返回的PartETag保存到List中。
                        partETags.Add(uploadPartResult.PartETag);
                    }
                }

                // 提交上传任务
                var completeRequest = new CompleteMultipartUploadRequest(bucketName, key, initResult.UploadId);
                foreach (var partETag in partETags)
                {
                    completeRequest.PartETags.Add(partETag);
                }
                client.CompleteMultipartUpload(completeRequest);

                Console.WriteLine("Multipart upload object:{0} succeeded", key);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }
        #endregion

        #region resumable upload
        public static void ResumableUploadProgress(string bucketName)
        {
            const string key = "ResumableUploadProgress";

            try
            {
                client.ResumableUploadObject(bucketName, key, fileToUpload, null, null, 1024 * 1024, streamProgressCallback);

                Console.WriteLine("Resumable upload object:{0} succeeded", key);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }
        #endregion

        #region get object
        public static void GetObjectProgress(string bucketName)
        {
            const string key = "GetObjectProgress";
            try
            {
                client.PutObject(bucketName, key, fileToUpload);

                var getObjectRequest = new GetObjectRequest(bucketName, key);
                getObjectRequest.StreamTransferProgress += streamProgressCallback;
                var ossObject = client.GetObject(getObjectRequest);
                using (var stream = ossObject.Content)
                {
                    var buffer = new byte[1024 * 1024];
                    var bytesTotal = 0;
                    var bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bytesTotal += bytesRead;
                        // Process read data
                        // TODO
                    }                    
                }
                
                Console.WriteLine("Get object:{0} succeeded", key);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }

        public static void AsyncGetObjectProgress(string bucketName)
        {
            const string key = "AsyncGetObjectProgress";
            try
            {
                client.PutObject(bucketName, key, fileToUpload);

                var getObjectRequest = new GetObjectRequest(bucketName, key);
                getObjectRequest.StreamTransferProgress += streamProgressCallback;

                string result = "Notice user: put object finish";
                client.BeginGetObject(getObjectRequest, GetObjectCallback, result.Clone());

                _event.WaitOne();
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }
        #endregion

        #region private
        private static void streamProgressCallback(object sender, StreamTransferProgressArgs args)
        {
            Console.WriteLine("ProgressCallback - TotalBytes:{0}, TransferredBytes:{1}, IncrementTransferred:{2}",
                args.TotalBytes, args.TransferredBytes, args.IncrementTransferred);
        }

        private static void PutObjectCallback(IAsyncResult ar)
        {
            try
            {
                client.EndPutObject(ar);
                Console.WriteLine(ar.AsyncState as string);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _event.Set();
            }
        }

        private static void AppendObjectCallback(IAsyncResult ar)
        {
            try
            {
                var result = client.EndAppendObject(ar);
                Console.WriteLine("Append object succeeded, next append position:{0}", result.NextAppendPosition);
                Console.WriteLine(ar.AsyncState as string);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _event.Set();
            }
        }

        private static int CalculatePartCount(long totalSize, int partSize)
        {
            var partCount = (int)(totalSize / partSize);
            if (totalSize % partSize != 0)
            {
                partCount++;
            }
            return partCount;
        }

        private static void GetObjectCallback(IAsyncResult ar)
        {
            try
            {
                var result = client.EndGetObject(ar);

                using (var stream = result.Content)
                {
                    var buffer = new byte[1024 * 1024];
                    var bytesTotal = 0;
                    var bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bytesTotal += bytesRead;
                        // Process read data
                        // TODO
                    }
                }

                Console.WriteLine(ar.AsyncState as string);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _event.Set();
            }
        }
        #endregion
    }
}
