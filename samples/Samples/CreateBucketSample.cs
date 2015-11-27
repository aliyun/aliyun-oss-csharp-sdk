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
    /// Sample for creating bucket.
    /// </summary>
    public static class CreateBucketSample
    {
        public static void CreateBucket(string bucketName)
        {
            const string accessKeyId = "<your access key id>"; 
            const string accessKeySecret = "<your access key secret>";
            const string endpoint = "<valid host name>";

            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            var created = false;
            try
            {
                client.CreateBucket(bucketName);
                created = true;
                Console.WriteLine("Created bucket name: " + bucketName);
                client.CreateBucket(bucketName);
            }
            catch (OssException ex)
            {
                if (ex.ErrorCode == OssErrorCode.BucketAlreadyExists)
                {
                    Console.WriteLine("Bucket '{0}' already exists, please modify and recreate it.", bucketName);
                }
                else
                {
                    Console.WriteLine("Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}", 
                        ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
                }
            }
            finally
            {
                if (created)
                {
                    client.DeleteBucket(bucketName);       
                }
            }
        }
    }
}
