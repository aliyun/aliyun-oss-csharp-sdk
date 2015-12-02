/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Test.Util
{
    public static class OssTestUtils
    {
        public static readonly List<string> InvalidBucketNamesList = new List<string>
            { "a", "1", "!", "aa", "12", "a1",
                "a!", "1!", "aAa", "1A1", "a!a", "FengChao@123", "-a123", "a_123", "a123-",
                "1234567890123456789012345678901234567890123456789012345678901234", 
                string.Empty, null 
            };

        public static readonly List<string> InvalidObjectKeyNamesList = new List<string>
        {
            "/abc", "\\123", string.Empty, null
        };

        public static readonly List<string> InvalidLoggingPrefixNamesList = new List<string>
        {
            "1", "-a", "@@", "a_", "abcdefghijklmnopqrstuvwxyz1234567"
        };

        public static readonly List<string> InvalidPageNamesList = new List<string>
        {
            "a", ".html", string.Empty, null
        };

        #region Bucket related
        public static string GetBucketName(string prefix)
        {
            return prefix.ToLower() + "-bucket-" + DateTime.Now.ToFileTimeUtc();
        }
        
        public static bool BucketExists(IOss oss, string bucketName)
        {
            return oss.DoesBucketExist(bucketName);
        }
        
        public static void EnsureBucketExist(IOss oss, string bucketName)
        {
            if (!BucketExists(oss, bucketName))
            {
                oss.CreateBucket(bucketName);
            }
        }
        
        public static void CleanBucket(IOss oss, string bucketName)
        {
            if (!BucketExists(oss, bucketName))
                return;
            //abort in progress multipart uploading
            var multipartListing = oss.ListMultipartUploads(new ListMultipartUploadsRequest(bucketName));
            foreach (var upload in multipartListing.MultipartUploads)
            {
                var abortRequest = new AbortMultipartUploadRequest(bucketName, upload.Key, upload.UploadId);
                oss.AbortMultipartUpload(abortRequest);
            }
            
            // Clean up objects
            var objects = oss.ListObjects(bucketName);
            foreach(var obj in objects.ObjectSummaries)
            {
                oss.DeleteObject(bucketName, obj.Key);
            }
            
            // Delete the bucket.
            oss.DeleteBucket(bucketName);
        }
        #endregion

        #region Object related
        public static string GetObjectKey(string prefix)
        {
            return prefix.ToLower() + "-object-" + DateTime.Now.ToFileTimeUtc();
        }

        public static PutObjectResult UploadObject(IOss ossClient, string bucketName, 
            string objectKeyName, string originalFile, ObjectMetadata metadata)
        {
            //using (var fs = File.Open(originalFile, FileMode.Open))
            //{
            //    return ossClient.PutObject(bucketName, objectKeyName, fs, metadata);
            //}
            return ossClient.PutObject(bucketName, objectKeyName, originalFile, metadata);
        }

        public static PutObjectResult UploadObject(IOss ossClient, string bucketName,
            string objectKeyName, string originalFile)
        {
            using (var fs = File.Open(originalFile, FileMode.Open))
            {
                return ossClient.PutObject(bucketName, objectKeyName, fs);
            }
        }

        public static void MultiPartUpload(IOss ossClient, string bucketName,
            string objectKeyName, string originalFile, int numberOfParts, int totalSize)
        {
            var initRequest = new InitiateMultipartUploadRequest(bucketName, objectKeyName);
            var initResult = ossClient.InitiateMultipartUpload(initRequest);

            var partSize = totalSize%numberOfParts == 0 ? totalSize/numberOfParts : totalSize/numberOfParts + 1;
            LogUtility.LogMessage("Each part size is {0} KB", partSize);
            //change to Byte
            partSize *= 1024;

            var partFile = new FileInfo(originalFile);

            // 新建一个List保存每个分块上传后的ETag和PartNumber
            var partETags = new List<PartETag>();
            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                for (var i = 0; i < numberOfParts; i++)
                {
                    // 跳到每个分块的开头
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // 计算每个分块的大小
                    long size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // 创建UploadPartRequest，上传分块
                    var uploadPartRequest = new UploadPartRequest(bucketName, objectKeyName, initResult.UploadId);
                    uploadPartRequest.InputStream = fs;
                    uploadPartRequest.PartSize = size;
                    uploadPartRequest.PartNumber = (i + 1);
                    var uploadPartResult = ossClient.UploadPart(uploadPartRequest);

                    // 将返回的PartETag保存到List中。
                    partETags.Add(uploadPartResult.PartETag);
                }
            }

            var completeRequest = new CompleteMultipartUploadRequest(bucketName, objectKeyName, initResult.UploadId);
            foreach (var partETag in partETags)
            {
                completeRequest.PartETags.Add(partETag);
            }
            ossClient.CompleteMultipartUpload(completeRequest);
        }

        public static bool ObjectExists(IOss ossClient, string bucketName, string objectKeyName)
        {
            return ossClient.ListObjects(bucketName).ObjectSummaries.Select( e => e.Key).Contains(objectKeyName);
        }

        public static void DownloadObject(IOss ossClient, string bucketName, string objectKeyName, string targetFile)
        {
            var originalObject = ossClient.GetObject(bucketName, objectKeyName);
            using (var requestStream = originalObject.Content)
            {
                using (var localfile = File.Open(targetFile, FileMode.OpenOrCreate))
                {
                    WriteTo(requestStream, localfile);
                }
            }
        }

        public static void DownloadObjectUsingRange(IOss ossClient, string bucketName, string objectKeyName, string targetFile)
        {
            var length = ossClient.GetObjectMetadata(bucketName, objectKeyName).ContentLength;
            var goRequest = new GetObjectRequest(bucketName, objectKeyName);
            goRequest.SetRange(0, length / 2);
            var originalObject = ossClient.GetObject(goRequest);
            using (var requestStream = originalObject.Content)
            {
                using (var localfile = File.Open(targetFile, FileMode.OpenOrCreate))
                {
                    WriteTo(requestStream, localfile);
                }
            }

            goRequest.SetRange(length / 2 + 1, length -1);
            originalObject = ossClient.GetObject(goRequest);
            using (var requestStream = originalObject.Content)
            {
                using (var localfile = File.Open(targetFile, FileMode.Append))
                {
                    WriteTo(requestStream, localfile);
                }
            }
        }

        private static void WriteTo(Stream src, Stream dest)
        {
            var buffer = new byte[1024 * 1024];
            var bytesRead = 0;
            while ((bytesRead = src.Read(buffer, 0, buffer.Length)) > 0)
            {
                dest.Write(buffer, 0, bytesRead);
            }
            dest.Flush();
        }
        #endregion

        public static string GetTargetFileName(string prefix)
        {
            return prefix.ToLower() + "-file-" + DateTime.Now.ToFileTimeUtc();
        }

        //bug: 5616977
        //wait for 5 seconds so that the cache is expired
        //and new setting values can be retrieved
        public static void WaitForCacheExpire()
        {
            Thread.Sleep(5 * 1000);
        }

        public static int CalculatePartCount(long totalSize, int singleSize)
        {
            // 计算分块数目
            var partCount = (int)(totalSize / singleSize);
            if (totalSize % singleSize != 0)
            {
                partCount++;
            }
            return partCount;
        }
    }
}
