/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Globalization;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for the usage of GetBucketLifecycle/SetBucketLifecycle.
    /// </summary>
    public static class BucketLifecycleSample
    {
        const string accessKeyId = "<your access key id>";
        const string accessKeySecret= "<your access key secret>";
        const string endpoint = "<valid host name>";

        static IOss client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        const string bucketName = "<bucket name>";

        public static void SetBucketLifecycle()
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

        public static void GetBucketLifecycle()
        {
            try
            {
                var rules = client.GetBucketLifecycle(bucketName);
                foreach (var rule in rules)
                {
                    Console.WriteLine("ID: {0}", rule.ID);
                    Console.WriteLine("Prefix: {0}", rule.Prefix);
                    Console.WriteLine("Status: {0}", rule.Status);
                    if (rule.ExpriationDays.HasValue)
                        Console.WriteLine("ExpirationDays: {0}", rule.ExpriationDays);
                    if (rule.ExpirationTime.HasValue)
                        Console.WriteLine("ExpirationTime: {0}", FormatIso8601Date(rule.ExpirationTime.Value));

                    Console.WriteLine();
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

        private static string FormatIso8601Date(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                               CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}
