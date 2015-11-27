/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
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
        public static void SetBucketCors()
        {
            const string accessKeyId = "<your access key id>";
            const string accessKeySecret = "<your access key secret>";
            const string endpoint = "<valid host name>";

            const string bucketName = "<bucket name>";

            OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            try
            {
                var req = new SetBucketCorsRequest(bucketName);
                var r1 = new CORSRule();
                // Note: AllowedOrigin & AllowdMethod must not be empty.
                r1.AddAllowedOrigin("http://www.a.com");
                r1.AddAllowedMethod("POST");
                r1.AddAllowedHeader("*");
                r1.AddExposeHeader("x-oss-test");
                req.AddCORSRule(r1);

                var r2 = new CORSRule();
                r2.AddAllowedOrigin("http://www.b.com");
                r2.AddAllowedMethod("GET");
                r2.AddExposeHeader("x-oss-test2");
                r2.MaxAgeSeconds = 1000000000;
                req.AddCORSRule(r2);

                client.SetBucketCors(req);

                var rs = client.GetBucketCors(bucketName);
                foreach (var r in rs)
                {
                    // do something with CORSRule here...
                }
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
            finally
            {
                client.DeleteBucketCors(bucketName);
            }
        }
    }
}
