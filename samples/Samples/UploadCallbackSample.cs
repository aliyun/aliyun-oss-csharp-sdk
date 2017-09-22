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
    /// Sample for the callback usage of put object and mutlipart upload.
    /// </summary>
    public static class UploadCallbackSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string fileToUpload = Config.FileToUpload;

        static string callbackUrl = Config.CallbackServer;
        static string callbackBody = "bucket=${bucket}&object=${object}&etag=${etag}&size=${size}&mimeType=${mimeType}&" +
                                     "my_var1=${x:var1}&my_var2=${x:var2}";

        static AutoResetEvent _event = new AutoResetEvent(false);

        public static void UploadCallback(string bucketName)
        {
            PutObjectProgress(bucketName);
            AsyncPutObjectProgress(bucketName);
            MultipartUploadProgress(bucketName);
            ResumableUploadProgress(bucketName);
        }
    
        #region put object
        public static void PutObjectProgress(string bucketName)
        {
            const string key = "PutObjectProgress";

            try
            {
                string responseContent = "";
                var metadata = BuildCallbackMetadata(callbackUrl, callbackBody);
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var putObjectRequest = new PutObjectRequest(bucketName, key, fs, metadata);
                    var result = client.PutObject(putObjectRequest);
                    responseContent = GetCallbackResponse(result);
                }
                Console.WriteLine("Put object:{0} succeeded, callback response content:{1}", key, responseContent);
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
                var metadata = BuildCallbackMetadata(callbackUrl, callbackBody);
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var putObjectRequest = new PutObjectRequest(bucketName, key, fs, metadata);
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
                        var uploadPartResult = client.UploadPart(uploadPartRequest);

                        // 将返回的PartETag保存到List中。
                        partETags.Add(uploadPartResult.PartETag);
                    }
                }

                // 提交上传任务
                var metadata = BuildCallbackMetadata(callbackUrl, callbackBody);
                var completeRequest = new CompleteMultipartUploadRequest(bucketName, key, initResult.UploadId)
                {
                    Metadata = metadata
                };
                foreach (var partETag in partETags)
                {
                    completeRequest.PartETags.Add(partETag);
                }
                var completeUploadResult = client.CompleteMultipartUpload(completeRequest);
                var responseContent = GetCallbackResponse(completeUploadResult);

                Console.WriteLine("Multipart upload object:{0} succeeded, callback response content:{1}", key, responseContent);
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
                var metadata = BuildCallbackMetadata(callbackUrl, callbackBody);
                var uploadResult = client.ResumableUploadObject(bucketName, key, fileToUpload, metadata, null, 102400);
                var responseContent = GetCallbackResponse(uploadResult);
                Console.WriteLine("Resumable upload object:{0} succeeded, callback response content:{1}", key, responseContent);
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
        private static void PutObjectCallback(IAsyncResult ar)
        {
            try
            {
                var result = client.EndPutObject(ar);
                Console.WriteLine("Put object succeeded, callback response content:{0}", GetCallbackResponse(result));

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

        private static string GetCallbackResponse(PutObjectResult putObjectResult)
        {
            string callbackResponse = null;
            using (var stream = putObjectResult.ResponseStream)
            {
                var buffer = new byte[4 * 1024];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                callbackResponse = Encoding.Default.GetString(buffer, 0, bytesRead);
            }
            return callbackResponse;
        }

        private static ObjectMetadata BuildCallbackMetadata(string callbackUrl, string callbackBody)
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(callbackUrl, callbackBody).Build();
            string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
                AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();

            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);
            return metadata;
        }
        #endregion
    }
}
