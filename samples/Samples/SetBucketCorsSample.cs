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
    /// Sample for setting bucket cors.
    /// </summary>
    public static class SetBucketCorsSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void SetBucketCors(string bucketName)
        {
            try
            {
                var request = new SetBucketCorsRequest(bucketName);
                var rule1 = new CORSRule();
                // Note: AllowedOrigin & AllowdMethod must not be empty.
                rule1.AddAllowedOrigin("http://www.a.com");
                rule1.AddAllowedMethod("POST");
                rule1.AddAllowedHeader("*");
                rule1.AddExposeHeader("x-oss-test");
                request.AddCORSRule(rule1);

                var rule2 = new CORSRule();
                rule2.AddAllowedOrigin("http://www.b.com");
                rule2.AddAllowedMethod("GET");
                rule2.AddExposeHeader("x-oss-test2");
                rule2.MaxAgeSeconds = 100;
                request.AddCORSRule(rule2);

                client.SetBucketCors(request);

                Console.WriteLine("Set bucket:{0} Cors succeeded ", bucketName);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}", 
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }
    }
}
