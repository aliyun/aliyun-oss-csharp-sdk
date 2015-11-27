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
        public static void CreateEmptyFolder()
        {
            const string accessKeyId = "<your access key id>";
            const string accessKeySecret = "<your access key secret>";
            const string endpoint = "<valid endpoint>";

            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            const string bucketName = "<your bucket>";
            // Note: key treats as a folder and must end with slash.
            const string key = "yourfolder/";  
            var created = false;
            var uploaded = false;
            try
            {
                // create bucket
                client.CreateBucket(bucketName);
                created = true;
                Console.WriteLine("Created bucket name: " + bucketName);

                // put object with zero bytes stream.
                using (MemoryStream memStream = new MemoryStream())
                {
                    PutObjectResult ret = client.PutObject(bucketName, key, memStream);
                    uploaded = true;
                    Console.WriteLine("Uploaded empty object's ETag: " + ret.ETag);
                }
            }
            catch (OssException ex)
            {
                if (ex.ErrorCode == OssErrorCode.BucketAlreadyExists)
                {
                    Console.WriteLine("Bucket '{0}' already exists, please modify and recreate it.", bucketName);
                }
                else
                {
                    Console.WriteLine("CreateBucket Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}", 
                        ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
                }
            }
            finally
            {
                if (uploaded)
                {
                    client.DeleteObject(bucketName, key);   
                }
                if (created)
                {
                    client.DeleteBucket(bucketName);       
                }
            }
        }
    }
}
