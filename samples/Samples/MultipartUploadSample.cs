/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Aliyun.OSS.Samples
{
    public class MultipartUploadSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string key = "key-1";
        static string fileToUpload = Config.BigFileToUpload;

        static int partSize = 10 * 1024 * 1024;

        public class UploadPartContext
        {
            public string BucketName { get; set; }
            public string ObjectName { get; set; }

            public List<PartETag> PartETags { get; set; }
 
            public string UploadId { get; set; }
            public long TotalParts { get; set; }
            public long CompletedParts { get; set; }
            public object SyncLock { get; set; }
            public ManualResetEvent WaitEvent { get; set; }
        }

        public class UploadPartContextWrapper
        {
            public UploadPartContext Context { get; set; }
            public int PartNumber { get; set; }
            public Stream PartStream { get; set; }

            public UploadPartContextWrapper(UploadPartContext context, Stream partStream, int partNumber)
            {
                Context = context;
                PartStream = partStream;
                PartNumber = partNumber;
            }
        }

        public class UploadPartCopyContext
        {
            public string TargetBucket { get; set; }
            public string TargetObject { get; set; }

            public List<PartETag> PartETags { get; set; }

            public string UploadId { get; set; }
            public long TotalParts { get; set; }
            public long CompletedParts { get; set; }
            public object SyncLock { get; set; }
            public ManualResetEvent WaitEvent { get; set; }
        }

        public class UploadPartCopyContextWrapper
        {
            public UploadPartCopyContext Context { get; set; }
            public int PartNumber { get; set; }

            public UploadPartCopyContextWrapper(UploadPartCopyContext context, int partNumber)
            {
                Context = context;
                PartNumber = partNumber;
            }
        }

        /// <summary>
        /// 分片上传。
        /// </summary>
        public static void UploadMultipart(String bucketName)
        {
            var uploadId = InitiateMultipartUpload(bucketName, key);
            var partETags = UploadParts(bucketName, key, fileToUpload, uploadId, partSize);
            var completeResult = CompleteUploadPart(bucketName, key, uploadId, partETags);

            Console.WriteLine("Multipart put object:{0} succeeded", key);
        }

        /// <summary>
        /// 异步分片上传。
        /// </summary>
        public static void AsyncUploadMultipart(String bucketName)
        {
            var uploadId = InitiateMultipartUpload(bucketName, key);
            AsyncUploadParts(bucketName, key, fileToUpload, uploadId, partSize);
        }

        /// <summary>
        /// 分片拷贝。
        /// </summary>
        public static void UploadMultipartCopy(String targetBucket, String targetObject, String sourceBucket, String sourceObject)
        {
            var uploadId = InitiateMultipartUpload(targetBucket, targetObject);
            var partETags = UploadPartCopys(targetBucket, targetObject, sourceBucket, sourceObject, uploadId, partSize);
            var completeResult = CompleteUploadPart(targetBucket, targetObject, uploadId, partETags);

            Console.WriteLine(@"Upload multipart copy result : ");
            Console.WriteLine(completeResult.Location);
        }

        /// <summary>
        /// 异步分片拷贝。
        /// </summary>
        public static void AsyncUploadMultipartCopy(String targetBucket, String targetObject, String sourceBucket, String sourceObject)
        {
            var uploadId = InitiateMultipartUpload(targetBucket, targetObject);
            AsyncUploadPartCopys(targetBucket, targetObject, sourceBucket, sourceObject, uploadId, partSize);
        }
        /// <summary>
        /// 列出所有执行中的Multipart Upload事件
        /// </summary>
        /// <param name="bucketName">目标bucket名称</param>
        public static void ListMultipartUploads(String bucketName)
        {
            var listMultipartUploadsRequest = new ListMultipartUploadsRequest(bucketName);
            var result = client.ListMultipartUploads(listMultipartUploadsRequest);
            Console.WriteLine("Bucket name:" + result.BucketName);
            Console.WriteLine("Key marker:" + result.KeyMarker);
            Console.WriteLine("Delimiter:" + result.Delimiter);
            Console.WriteLine("Prefix:" + result.Prefix);
            Console.WriteLine("UploadIdMarker:" + result.UploadIdMarker);

            foreach (var part in result.MultipartUploads)
            {
                Console.WriteLine(part.ToString());
            }
        }

        private static string InitiateMultipartUpload(String bucketName, String objectName)
        {
            var request = new InitiateMultipartUploadRequest(bucketName, objectName);
            var result = client.InitiateMultipartUpload(request);
            return result.UploadId;
        }

        private static List<PartETag> UploadParts(String bucketName, String objectName, String fileToUpload,
                                                  String uploadId, int partSize)
        {
            var fi = new FileInfo(fileToUpload);
            var fileSize = fi.Length;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            var partETags = new List<PartETag>();
            using (var fs = File.Open(fileToUpload, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    var skipBytes = (long)partSize * i;
                    fs.Seek(skipBytes, 0);
                    var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                    var request = new UploadPartRequest(bucketName, objectName, uploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = i + 1
                    };
                   
                    var result = client.UploadPart(request);
                    Console.WriteLine("oss etag:{0}", result.PartETag.ETag);

                    partETags.Add(result.PartETag);
                }
            }
            return partETags;
        }

        private static void AsyncUploadParts(String bucketName, String objectName, String fileToUpload,
            String uploadId, int partSize)
        {
            var fi = new FileInfo(fileToUpload);
            var fileSize = fi.Length;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            var ctx = new UploadPartContext()
            {
                BucketName = bucketName,
                ObjectName = objectName,
                UploadId = uploadId,
                TotalParts = partCount,
                CompletedParts = 0,
                SyncLock = new object(),
                PartETags = new List<PartETag>(),
                WaitEvent = new ManualResetEvent(false)
            };

            for (var i = 0; i < partCount; i++)
            {
                var fs = new FileStream(fileToUpload, FileMode.Open, FileAccess.Read, FileShare.Read);
                var skipBytes = (long)partSize * i;
                fs.Seek(skipBytes, 0);
                var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                var request = new UploadPartRequest(bucketName, objectName, uploadId)
                {
                    InputStream = fs,
                    PartSize = size,
                    PartNumber = i + 1
                };
                client.BeginUploadPart(request, UploadPartCallback, new UploadPartContextWrapper(ctx, fs, i + 1));
            }

            ctx.WaitEvent.WaitOne();
        }

        private static void UploadPartCallback(IAsyncResult ar)
        {
            var result = client.EndUploadPart(ar);
            var wrappedContext = (UploadPartContextWrapper)ar.AsyncState;
            wrappedContext.PartStream.Close();

            var ctx = wrappedContext.Context;
            lock (ctx.SyncLock)
            {
                var partETags = ctx.PartETags;
                partETags.Add(new PartETag(wrappedContext.PartNumber, result.ETag));
                ctx.CompletedParts++;

                if (ctx.CompletedParts == ctx.TotalParts)
                {
                    partETags.Sort((e1, e2) => (e1.PartNumber - e2.PartNumber));
                    var completeMultipartUploadRequest = 
                        new CompleteMultipartUploadRequest(ctx.BucketName, ctx.ObjectName, ctx.UploadId);
                    foreach (var partETag in partETags)
                    {
                        completeMultipartUploadRequest.PartETags.Add(partETag);
                    }

                    var completeMultipartUploadResult = client.CompleteMultipartUpload(completeMultipartUploadRequest);
                    Console.WriteLine(@"Async upload multipart result : " + completeMultipartUploadResult.Location);

                    ctx.WaitEvent.Set();
                }
            }
        }

        private static List<PartETag> UploadPartCopys(String targetBucket, String targetObject, String sourceBucket, String sourceObject,
            String uploadId, int partSize)
        {
            var metadata = client.GetObjectMetadata(sourceBucket, sourceObject);
            var fileSize = metadata.ContentLength;

            var partCount = (int)fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            var partETags = new List<PartETag>();
            for (var i = 0; i < partCount; i++)
            {
                var skipBytes = (long)partSize * i;
                var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                var request =
                    new UploadPartCopyRequest(targetBucket, targetObject, sourceBucket, sourceObject, uploadId)
                    {
                        PartSize = size,
                        PartNumber = i + 1,
                        BeginIndex = skipBytes
                    };
                var result = client.UploadPartCopy(request);
                partETags.Add(result.PartETag);
            }

            return partETags;
        }

        private static void AsyncUploadPartCopys(String targetBucket, String targetObject, String sourceBucket, String sourceObject,
            String uploadId, int partSize)
        {
            var metadata = client.GetObjectMetadata(sourceBucket, sourceObject);
            var fileSize = metadata.ContentLength;

            var partCount = (int)fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            var ctx = new UploadPartCopyContext()
            {
                TargetBucket = targetBucket,
                TargetObject = targetObject,
                UploadId = uploadId,
                TotalParts = partCount,
                CompletedParts = 0,
                SyncLock = new object(),
                PartETags = new List<PartETag>(),
                WaitEvent = new ManualResetEvent(false)
            };

            for (var i = 0; i < partCount; i++)
            {
                var skipBytes = (long)partSize * i;
                var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                var request =
                    new UploadPartCopyRequest(targetBucket, targetObject, sourceBucket, sourceObject, uploadId)
                    {
                        PartSize = size,
                        PartNumber = i + 1,
                        BeginIndex = skipBytes
                    };
                client.BeginUploadPartCopy(request, UploadPartCopyCallback, new UploadPartCopyContextWrapper(ctx, i + 1));
            }

            ctx.WaitEvent.WaitOne();
        }

        private static void UploadPartCopyCallback(IAsyncResult ar)
        {
            var result = client.EndUploadPartCopy(ar);
            var wrappedContext = (UploadPartCopyContextWrapper)ar.AsyncState;

            var ctx = wrappedContext.Context;
            lock (ctx.SyncLock)
            {
                var partETags = ctx.PartETags;
                partETags.Add(new PartETag(wrappedContext.PartNumber, result.ETag));
                ctx.CompletedParts++;

                if (ctx.CompletedParts == ctx.TotalParts)
                {
                    partETags.Sort((e1, e2) => (e1.PartNumber - e2.PartNumber));
                    var completeMultipartUploadRequest =
                        new CompleteMultipartUploadRequest(ctx.TargetBucket, ctx.TargetObject, ctx.UploadId);
                    foreach (var partETag in partETags)
                    {
                        completeMultipartUploadRequest.PartETags.Add(partETag);
                    }

                    var completeMultipartUploadResult = client.CompleteMultipartUpload(completeMultipartUploadRequest);
                    Console.WriteLine(@"Async upload multipart copy result : " + completeMultipartUploadResult.Location);

                    ctx.WaitEvent.Set();
                }
            }
        }

        private static CompleteMultipartUploadResult CompleteUploadPart(String bucketName, String objectName,
            String uploadId, List<PartETag> partETags)
        {
            var completeMultipartUploadRequest =
                new CompleteMultipartUploadRequest(bucketName, objectName, uploadId);
            foreach (var partETag in partETags)
            {
                completeMultipartUploadRequest.PartETags.Add(partETag);
            }

            return client.CompleteMultipartUpload(completeMultipartUploadRequest);
        }
    }
 }

