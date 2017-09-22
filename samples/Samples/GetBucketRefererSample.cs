/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for getting bucket referer list.
    /// </summary>
    public static class GetBucketRefererSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void GetBucketReferer(string bucketName)
        {
            try
            {
                var rc = client.GetBucketReferer(bucketName);

                Console.WriteLine("Get bucket:{0} Referer succeeded ", bucketName);

                Console.WriteLine("allow？" + (rc.AllowEmptyReferer ? "yes" : "no"));
                if (rc.RefererList.Referers != null)
                {
                    for (var i = 0; i < rc.RefererList.Referers.Length; i++)
                        Console.WriteLine(rc.RefererList.Referers[i]);
                }
                else
                {
                    Console.WriteLine("Empty Referer List");
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
                client.SetBucketReferer(new SetBucketRefererRequest(bucketName));
            }
        }
    }
}
