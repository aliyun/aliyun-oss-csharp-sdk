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
    /// Sample for deleting objects.
    /// </summary>
    public static class DeleteObjectsSample
    {
        public static void DeleteObjects()
        {
            const string accessKeyId = "<your access key id>";
            const string accessKeySecret = "<your access key secret>";
            const string endpoint = "<valid host name>";

            const string bucketName = "<your bucket name>";

            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            try
            {
                var keys = new List<string>();
                var listResult = client.ListObjects(bucketName);
                foreach (var summary in listResult.ObjectSummaries)
                {
                    Console.WriteLine(summary.Key);
                    keys.Add(summary.Key);
                }
                var request = new DeleteObjectsRequest(bucketName, keys, false);
                client.DeleteObjects(request);
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
