/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for listing buckets.
    /// </summary>
    public static class ListBucketsSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void ListBuckets()
        {
            ListAllBuckets();
            ListBucketsByConditions();
        }

        // 1. Try to list all buckets. 
        public static void ListAllBuckets()
        {
            try
            {
                var buckets = client.ListBuckets();

                Console.WriteLine("List all buckets: ");
                foreach (var bucket in buckets)
                {
                    Console.WriteLine("Name:{0}", bucket.Name);
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

        // 2. List buckets by specified conditions, such as prefix/marker/max-keys.
        public static void ListBucketsByConditions()
        {
            try
            {
                var req = new ListBucketsRequest {Prefix = "test", MaxKeys = 3, Marker = "test2"};
                var result = client.ListBuckets(req);
                var buckets = result.Buckets;
                Console.WriteLine("List buckets by page: ");
                Console.WriteLine("Prefix: {0}, MaxKeys: {1},  Marker: {2}, NextMarker:{3}", 
                                   result.Prefix, result.MaxKeys, result.Marker, result.NextMaker);
                foreach (var bucket in buckets)
                {
                    Console.WriteLine("Name:{0}, Location:{1}, Owner:{2}", bucket.Name, bucket.Location, bucket.Owner);
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
    }
}
