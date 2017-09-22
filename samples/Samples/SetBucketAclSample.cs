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
    /// Sample for setting bucket acl.
    /// </summary>
    public static class SetBucketAclSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void SetBucketAcl(string bucketName)
        {
            SetBucketAclWithRawParameter(bucketName);
            SetBucketAclWithRequest(bucketName);
        }

        public static void SetBucketAclWithRawParameter(string bucketName)
        {
            try
            {
                client.SetBucketAcl(bucketName, CannedAccessControlList.PublicRead);
                Console.WriteLine("Set bucket:{0} Acl succeeded ", bucketName);
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

        public static void SetBucketAclWithRequest(string bucketName)
        {
            try
            {
                var request = new SetBucketAclRequest(bucketName, CannedAccessControlList.PublicRead);
                client.SetBucketAcl(request);
                Console.WriteLine("Set bucket:{0} Acl succeeded ", bucketName);
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
