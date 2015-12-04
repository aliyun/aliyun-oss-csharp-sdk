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
    /// Sample for setting bucket acl.
    /// </summary>
    public static class GetBucketAclSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void GetBucketAcl(string bucketName)
        {
            try
            {
                var result = client.GetBucketAcl(bucketName);

                foreach (var grant in result.Grants)
                {
                    Console.WriteLine("Get Bucket Acl succeeded,Id:{0} Acl:{1} succeeded", 
                                       grant.Grantee.Identifier.ToString(), grant.Permission.ToString());
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
