/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common;
using System.Collections.Generic;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for creating bucket.
    /// </summary>
    public static class DeleteBucketSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void DeleteBucket(string bucketName)
        {
            try
            {
                client.DeleteBucket(bucketName);

                Console.WriteLine("Delete bucket name:{0} succeeded ", bucketName);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}", 
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
        }

        public static void DeleteNoEmptyBucket(string bucketName)
        {
            try
            {
                DeleteObjectsOfBucket(bucketName);

                AbortMultipartUploadOfBucket(bucketName);

                client.DeleteBucket(bucketName);

                Console.WriteLine("Delete bucket name:{0} succeeded ", bucketName);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
        }

        private static void AbortMultipartUploadOfBucket(string bucketName)
        {
            var multipartUploads = new Dictionary<string, string>();
            MultipartUploadListing result = null;
            string nextKeyMarker = string.Empty;
            string nextUploadIdMarker = string.Empty;

            do
            {
                var listUploadRequest = new ListMultipartUploadsRequest(bucketName)
                {
                    KeyMarker = nextKeyMarker,
                    UploadIdMarker = nextUploadIdMarker,
                    MaxUploads = 100
                };
                result = client.ListMultipartUploads(listUploadRequest);

                foreach (var upload in result.MultipartUploads)
                {
                    multipartUploads[upload.UploadId] = upload.Key;
                }

                nextKeyMarker = result.NextKeyMarker;
                nextUploadIdMarker = result.NextUploadIdMarker;

                foreach (var uploadId in multipartUploads.Keys)
                {
                    client.AbortMultipartUpload(new AbortMultipartUploadRequest(bucketName, multipartUploads[uploadId], uploadId));
                } 

                multipartUploads.Clear();

            } while (result.IsTruncated);
        }

        private static void DeleteObjectsOfBucket(string bucketName)
        {
            var keys = new List<string>();
            ObjectListing result = null;
            string nextMarker = string.Empty;
            do
            {
                var listObjectsRequest = new ListObjectsRequest(bucketName)
                {
                    Marker = nextMarker,
                    MaxKeys = 100
                };
                result = client.ListObjects(listObjectsRequest);

                foreach (var summary in result.ObjectSummaries)
                {
                    keys.Add(summary.Key);
                }

                nextMarker = result.NextMarker;

                if (keys.Count != 0)
                {
                    client.DeleteObjects(new DeleteObjectsRequest(bucketName, keys));
                    keys.Clear();
                }
            } while (result.IsTruncated);
        }
    }
}
