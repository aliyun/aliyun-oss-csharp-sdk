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
        public static void SetBucketReferer()
        {
            const string accessKeyId = "<your access key id>";
            const string accessKeySecret = "<your access key secret>";
            const string endpoint = "<valid host name>";

            const string bucketName = "<bucket name>";

            OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            try
            {
                var refererList = new List<string>();
                refererList.Add(" http://www.aliyun.com");
                refererList.Add(" http://www.*.com");
                refererList.Add(" http://www.?.aliyuncs.com");
                var srq = new SetBucketRefererRequest(bucketName, refererList);
                //srq.ClearRefererList();
                client.SetBucketReferer(srq);

                var rc = client.GetBucketReferer(bucketName);
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
