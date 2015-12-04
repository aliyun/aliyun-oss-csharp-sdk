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
    /// Sample for the usage of CNAME.
    /// </summary>
    public static class CNameSample
    {
        static string accessKeyId = "your access key id for CNAME";
        static string accessKeySecret = "your access key secret for CNAME";
        static string endpoint = "<your endpoint for CNAME>";

        static string key = "key-name";
        static string fileToUpload = Config.FileToUpload;

        static ClientConfiguration clientConfig = new ClientConfiguration();
        static OssClient client = null;

        static CNameSample()
        {
            clientConfig.IsCname = true;
            client = new OssClient(new Uri(endpoint), accessKeyId, accessKeySecret, clientConfig);
        }

        public static void CNameOperation(string bucketName)
        {
            PutObject(bucketName);
            DeleteObject(bucketName);
            SetBucketAcl(bucketName);
        }

        public static void PutObject(string bucketName) 
        {
            try
            {
                client.PutObject(bucketName, key, fileToUpload);

                Console.WriteLine("Put object with CNAME succeeded");
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

        public static void DeleteObject(string bucketName)
        {
            try
            {
                client.DeleteObject(bucketName, key);

                Console.WriteLine("Delete object with CNAME succeeded");
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

        public static void SetBucketAcl(string bucketName)
        {
            try
            {
                client.SetBucketAcl(bucketName, CannedAccessControlList.PublicRead);

                Console.WriteLine("Set bucket acl with CNAME succeeded");
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
