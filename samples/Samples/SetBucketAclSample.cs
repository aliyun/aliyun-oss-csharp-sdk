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
    public static class SetBucketAclSample
    {
        public static void SetBucketAcl()
        {
            const string accessKeyId = "<your access key id>";
            const string accessKeySecret = "<your access key secret>";
            const string endpoint = "<valid host name>";

            const string bucketName = "<bucket name>";

            OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            try
            {
                client.SetBucketAcl(bucketName, CannedAccessControlList.PublicRead);
                //client.SetBucketAcl(new SetBucketAclRequest(bucketName, CannedAccessControlList.PublicRead));
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
