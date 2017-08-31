/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Globalization;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for setting bucket lifecycle
    /// </summary>
    public static class SetBucketLifecycleSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void SetBucketLifecycle(string bucketName)
        {
            try
            {
                var setBucketLifecycleRequest = new SetBucketLifecycleRequest(bucketName);
                LifecycleRule lcr1 = new LifecycleRule()
                {
                    ID = "delete obsoleted files",
                    Prefix = "obsoleted/",
                    Status = RuleStatus.Enabled,
                    ExpriationDays = 3
                };

                LifecycleRule lcr2 = new LifecycleRule()
                {
                    ID = "delete temporary files",
                    Prefix = "temporary/",
                    Status = RuleStatus.Enabled,
                    ExpirationTime = DateTime.Parse("2022-10-12T00:00:00.000Z")
                };

                setBucketLifecycleRequest.AddLifecycleRule(lcr1);
                setBucketLifecycleRequest.AddLifecycleRule(lcr2);

                client.SetBucketLifecycle(setBucketLifecycleRequest);

                Console.WriteLine("Set bucket:{0} Lifecycle succeeded ", bucketName);
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
