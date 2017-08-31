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
    public static class SetObjectAclSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string key = "GetObjectSample";
        static string fileToUpload = Config.FileToUpload;

        public static void SetObjectAcl(string bucketName)
        {
            SetObjectAclWithRawParameter(bucketName);
            SetObjectAclWithRequest(bucketName);
        }

        public static void SetObjectAclWithRawParameter(string bucketName)
        {

            try
            {
                client.PutObject(bucketName, key, fileToUpload);

                client.SetObjectAcl(bucketName, key, CannedAccessControlList.PublicRead);
                Console.WriteLine("Set Object:{0} Acl succeeded ", key);
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

        public static void SetObjectAclWithRequest(string bucketName)
        {
            try
            {
                client.PutObject(bucketName, key, fileToUpload);

                var request = new SetObjectAclRequest(bucketName, key, CannedAccessControlList.PublicRead);
                client.SetObjectAcl(request);
                Console.WriteLine("Set Object:{0} Acl succeeded ", key);
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
