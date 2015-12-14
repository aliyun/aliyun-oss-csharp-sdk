/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for creating an empty folder.
    /// </summary>
    public static class CreateEmptyFolderSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void CreateEmptyFolder(string bucketName)
        {
            // Note: key treats as a folder and must end with slash.
            const string key = "yourfolder/";  
            try
            {
                // put object with zero bytes stream.
                using (MemoryStream memStream = new MemoryStream())
                {
                    client.PutObject(bucketName, key, memStream);
                    Console.WriteLine("Create dir:{0} succeeded", key);
                }
            }
            catch (OssException ex)
            {
                Console.WriteLine("CreateBucket Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}", 
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
        }
    }
}
