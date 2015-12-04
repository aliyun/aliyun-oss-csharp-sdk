/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for setting bucket referer list.
    /// </summary>
    public static class SetBucketRefererSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void SetBucketReferer(string bucketName)
        {
            try
            {
                var refererList = new List<string>();
                refererList.Add(" http://www.aliyun.com");
                refererList.Add(" http://www.*.com");
                refererList.Add(" http://www.?.aliyuncs.com");
                var srq = new SetBucketRefererRequest(bucketName, refererList);
                client.SetBucketReferer(srq);

                Console.WriteLine("Set bucket:{0} Referer succeeded ", bucketName);
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
