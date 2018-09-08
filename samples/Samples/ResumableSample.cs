/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
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
    /// Sample for resumbale upload and copy.
    /// </summary>
    public static class ResumbaleSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string fileToUpload = Config.BigFileToUpload;

        private static void streamProgressCallback(object sender, StreamTransferProgressArgs args)
        {
            Console.WriteLine("ProgressCallback - TotalBytes:{0}, TransferredBytes:{1}, IncrementTransferred:{2}",
                args.TotalBytes, args.TransferredBytes, args.IncrementTransferred);
        }


        public static void ResumableUploadObject(string bucketName)
        {
            const string key = "ResumableUploadObject";
            string checkpointDir = Config.DirToDownload;
            try
            {
                UploadObjectRequest request = new UploadObjectRequest(bucketName, key, fileToUpload)
                {
                    PartSize = 8 * 1024 * 1024,
                    ParallelThreadCount = 3,
                    CheckpointDir = checkpointDir,
                    StreamTransferProgress = streamProgressCallback,
                };
                client.ResumableUploadObject(request);
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

        public static void ResumableDownloadObject(string bucketName)
        {
            const string key = "ResumableDownloadObject";
            string fileToDownload = Config.DirToDownload + key;
            string checkpointDir = Config.DirToDownload;
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(bucketName, key, fileToDownload)
                {
                    PartSize = 8 * 1024 * 1024,
                    ParallelThreadCount = 3,
                    CheckpointDir = Config.DirToDownload,
                    StreamTransferProgress = streamProgressCallback,
                };
                client.ResumableDownloadObject(request);
                Console.WriteLine("Resumable download object:{0} succeeded", key);
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
