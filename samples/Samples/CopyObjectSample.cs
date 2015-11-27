/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Threading;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for copying object.
    /// </summary>
    public static class CopyObjectSample
    {
        const string accessKeyId = "<your access key id>";
        const string accessKeySecret= "<your access key secret>";
        const string endpoint = "<valid host name>";

        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        const string sourceBucket = "<source bucket name>";
        const string sourceKey = "<source object name>";
        const string targetBucket = "<target bucket name>";
        const string targetKey = "<target object name>";

        static AutoResetEvent _event = new AutoResetEvent(false);

        public static void CopyObject()
        {
            try
            {
                var metadata = new ObjectMetadata();
                metadata.AddHeader("mk1", "mv1");
                metadata.AddHeader("mk2", "mv2");
                var req = new CopyObjectRequest(sourceBucket, sourceKey, targetBucket, targetKey)
                {
                    NewObjectMetadata = metadata
                };
                var ret = client.CopyObject(req);
                Console.WriteLine("target key's etag: " + ret.ETag);
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

        public static void AsyncCopyObject()
        {
            try
            {
                var metadata = new ObjectMetadata();
                metadata.AddHeader("mk1", "mv1");
                metadata.AddHeader("mk2", "mv2");
                var req = new CopyObjectRequest(sourceBucket, sourceKey, targetBucket, targetKey)
                {
                    NewObjectMetadata = metadata
                };
                client.BeginCopyObject(req, CopyObjectCallback, null);

                _event.WaitOne();
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

        private static void CopyObjectCallback(IAsyncResult ar)
        {
            try
            {
                var result = client.EndCopyResult(ar);
                Console.WriteLine(result.ETag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _event.Set();
            }
        }
    }
}
