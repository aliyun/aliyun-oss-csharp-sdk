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
    /// Sample for putting object.
    /// </summary>
    public static class PutObjectSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string fileToUpload = Config.FileToUpload;

        static AutoResetEvent _event = new AutoResetEvent(false);

        /// <summary>
        /// sample for put object to oss
        /// </summary>
        public static void PutObject(string bucketName)
        {
            PutObjectFromFile(bucketName);

            PutObjectFromString(bucketName);

            PutObjectWithDir(bucketName);

            PutObjectWithMd5(bucketName);

            PutObjectWithHeader(bucketName);

            AsyncPutObject(bucketName);
        }

        public static void PutObjectFromFile(string bucketName)
        {
            const string key = "PutObjectFromFile";
            try
            {
                client.PutObject(bucketName, key, fileToUpload);
                Console.WriteLine("Put object:{0} succeeded", key);
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

        public static void PutObjectFromString(string bucketName)
        {
            const string key = "PutObjectFromString";
            const string str = "Aliyun OSS SDK for C#";

            try
            {
                byte[] binaryData = Encoding.ASCII.GetBytes(str); 
                var stream = new MemoryStream(binaryData);
                
                client.PutObject(bucketName, key, stream);
                Console.WriteLine("Put object:{0} succeeded", key);
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

        public static void PutObjectWithDir(string bucketName)
        {
            const string key = "folder/sub_folder/PutObjectFromFile";

            try
            {
                client.PutObject(bucketName, key, fileToUpload);
                Console.WriteLine("Put object:{0} succeeded", key);
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

        public static void PutObjectWithMd5(string bucketName)
        {
            const string key = "PutObjectWithMd5";

            string eTag;
            using (var fs = File.Open(fileToUpload, FileMode.Open))
            {
                eTag = OssUtils.ComputeContentMd5(fs, fs.Length);
            }

            var meta = new ObjectMetadata() { ETag = eTag };
            try
            {
                client.PutObject(bucketName, key, fileToUpload, meta);

                Console.WriteLine("Put object:{0} succeeded", key);
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

        public static void PutObjectWithHeader(string bucketName)
        {
            const string key = "PutObjectWithHeader";
            try
            {
                using (var content = File.Open(fileToUpload, FileMode.Open))
                {
                    var metadata = new ObjectMetadata();                    
                    metadata.ContentLength = content.Length;

                    metadata.UserMetadata.Add("github-account", "qiyuewuyi");

                    client.PutObject(bucketName, key, content, metadata);

                    Console.WriteLine("Put object:{0} succeeded", key);
                }
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

        public static void AsyncPutObject(string bucketName)
        {
            const string key = "AsyncPutObject";
            try
            {
                // 1. put object to specified output stream
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var metadata = new ObjectMetadata();
                    metadata.UserMetadata.Add("mykey1", "myval1");
                    metadata.UserMetadata.Add("mykey2", "myval2");
                    metadata.CacheControl = "No-Cache";
                    metadata.ContentType = "text/html";

                    string result = "Notice user: put object finish";
                    client.BeginPutObject(bucketName, key, fs, metadata, PutObjectCallback, result.ToCharArray());

                    _event.WaitOne();
                }
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

        private static void PutObjectCallback(IAsyncResult ar)
        {
            try
            {
                var result = client.EndPutObject(ar);

                Console.WriteLine(ar.AsyncState as string);
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
