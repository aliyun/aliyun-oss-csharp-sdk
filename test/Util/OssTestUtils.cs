/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

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
            return prefix.ToLower() + DateTime.Now.Ticks.ToString();
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
            using (Stream fs = File.OpenRead(originalFile))
            {
                return ossClient.PutObject(bucketName, objectKeyName, fs);
            }
        }

        public static IAsyncResult BeginUploadObject(IOss ossClient, string bucketName, 
                                                    string objectKeyName, string originalFile, AsyncCallback callback, object callbackObject)
        {
            var metadata = new ObjectMetadata();
            metadata.CacheControl = "No-Cache";
            metadata.ContentType = "text/html";
            return ossClient.BeginPutObject(bucketName, objectKeyName, originalFile, metadata, callback, callbackObject);
        }

        public static PutObjectResult EndUploadObject(IOss ossClient, IAsyncResult ar)
        {
            return ossClient.EndPutObject(ar);
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

            // Create a list to save the result
            var partETags = new List<PartETag>();
            //upload the file
            using (Stream fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                for (var i = 0; i < numberOfParts; i++)
                {
                    // Skip to the start position
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // Calculate the part size 
                    long size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // Create a UploadPartRequest, uploading the part
                    var uploadPartRequest = new UploadPartRequest(bucketName, objectKeyName, initResult.UploadId);
                    uploadPartRequest.InputStream = fs;
                    uploadPartRequest.PartSize = size;
                    uploadPartRequest.PartNumber = (i + 1);
                    var uploadPartResult = ossClient.UploadPart(uploadPartRequest);

                    // Save the result 
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
            return ossClient.DoesObjectExist(bucketName, objectKeyName);
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

        public static void WriteTo(Stream src, Stream dest)
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

        public static void WaitForCacheExpire(int sec)
        {
            Thread.Sleep(sec * 1000);
        }

        public static int CalculatePartCount(long totalSize, int singleSize)
        {
            // Calculate the part count
            var partCount = (int)(totalSize / singleSize);
            if (totalSize % singleSize != 0)
            {
                partCount++;
            }
            return partCount;
        }

        public static char GetRandomChar(Random rnd)
        {
            int ret = rnd.Next(122);
            while (ret < 48 || (ret > 57 && ret < 65) || (ret > 90 && ret < 97))
            {
                ret = rnd.Next(122);
            }
            return (char)ret;
        }

        public static string GetRandomString(int length)
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(GetRandomChar(rnd));
            }
            return sb.ToString();
        } 

        public static List<T> ToArray<T>(IEnumerable<T> source)
        {
            var list = new List<T>();
            foreach (var entry in source)
            {
                list.Add(entry);
            }
            return list;
        }

        public static bool IsIp(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static string GetIpByEndpoint(string endpoint)
        {
            if (IsIp(endpoint))
            {
                return endpoint;
            }

            var uri = new Uri(endpoint);
            if (IsIp(uri.Host))
            {
                return uri.Host;
            }

            var ips = Dns.GetHostEntry(uri.Host);
            return ips.AddressList[0].ToString();
        }
    }
}
