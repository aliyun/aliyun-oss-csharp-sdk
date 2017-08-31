/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Threading;
using System.Collections.Generic;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for listing objects.
    /// </summary>
    public static class ListObjectsSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static AutoResetEvent _event = new AutoResetEvent(false);

        public static void ListObjects(string bucketName)
        {
            SimpleListObjects(bucketName);
            ListObjectsWithRequest(bucketName);
            AsyncListObjects(bucketName);
        }

        public static void SimpleListObjects(string bucketName)
        {
            try
            {
                var result = client.ListObjects(bucketName);

                Console.WriteLine("List objects of bucket:{0} succeeded ", bucketName);
                foreach (var summary in result.ObjectSummaries)
                {
                    Console.WriteLine(summary.Key);
                }

                Console.WriteLine("List objects of bucket:{0} succeeded, is list all objects ? {1}", bucketName, !result.IsTruncated);
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

        public static void ListObjectsWithRequest(string bucketName)
        {
            try
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
                        Console.WriteLine(summary.Key);
                        keys.Add(summary.Key);
                    }

                    nextMarker = result.NextMarker;
                } while (result.IsTruncated);

                Console.WriteLine("List objects of bucket:{0} succeeded ", bucketName);
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

        public static void AsyncListObjects(string bucketName)
        {
            try
            {
                var listObjectsRequest = new ListObjectsRequest(bucketName);
                client.BeginListObjects(listObjectsRequest, ListObjectCallback, null);

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

        private static void ListObjectCallback(IAsyncResult ar)
        {
            try
            {
                var result = client.EndListObjects(ar);

                Console.WriteLine("List objects of bucket:{0} succeeded ", result.BucketName);

                foreach (var summary in result.ObjectSummaries)
                {
                    Console.WriteLine(summary.Key);
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
                _event.Set();
            }
        }
    }
}
