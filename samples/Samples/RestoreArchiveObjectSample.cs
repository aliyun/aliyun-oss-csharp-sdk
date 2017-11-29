/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Net;
using System.Threading;
using Aliyun.OSS.Model;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample usage of RestoreObject API
    /// </summary>
    public static class RestoreArchiveObjectSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void RestoreArchiveObject(string bucketName, string key, bool waitUtilFinished = true, int maxWaitTimeInSeconds = 600)
        {
            RestoreObjectResult result = client.RestoreObject(bucketName, key);
            if (result.HttpStatusCode != HttpStatusCode.Accepted || !waitUtilFinished)
            {
                throw new OssException(result.RequestId + ", " + result.HttpStatusCode + " ,");
            }

            while (maxWaitTimeInSeconds > 0)
            {
                var meta = client.GetObjectMetadata(bucketName, key);
                string restoreStatus = meta.HttpMetadata["x-oss-restore"] as string;
                if (restoreStatus != null && restoreStatus.StartsWith("ongoing-request=\"false\"", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }

                Thread.Sleep(1000);
                maxWaitTimeInSeconds--;
            }

            if (maxWaitTimeInSeconds == 0)
            {
                throw new TimeoutException();
            }
        }

    }
}
