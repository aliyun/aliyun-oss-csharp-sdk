/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;
using System.Threading;
using Aliyun.OSS.Common;
using System.Text;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// 具有断点续传功能的上传和拷贝示例程序
    /// </summary>
    public static class ResumbaleSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string fileToUpload = Config.BigFileToUpload;

        public static void ResumableUploadObject(string bucketName) 
        {
            const string key = "ResumableUploadObject";
            string checkpointDir = Config.DirToDownload;
            try
            {
                client.ResumableUploadObject(bucketName, key, fileToUpload, null, checkpointDir);
                Console.WriteLine("Resumable upload object:{0} succeeded", key);
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

        public static void ResumableCopyObject(string sourceBucketName, string sourceKey, 
                                               string destBucketName, string destKey)
        {
            
            string checkpointDir = Config.DirToDownload;
            try
            {
                var request = new CopyObjectRequest(sourceBucketName, sourceKey, destBucketName, destKey);
                client.ResumableCopyObject(request, checkpointDir);
                Console.WriteLine("Resumable copy new object:{0} succeeded", request.DestinationKey);
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
