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
using System.Security.Cryptography;

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
        static HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();

        /// <summary>
        /// sample for put object to oss
        /// </summary>
        public static void PutObject(string bucketName)
        {
            PutObjectFromFile(bucketName);

            PutObjectFromFileWithTimeout(bucketName);

            PutObjectFromString(bucketName);

            PutObjectWithDir(bucketName);

            PutObjectWithMd5(bucketName);

            PutObjectWithHeader(bucketName);

            AsyncPutObject(bucketName);

            PutObjectFromStringWithHashPrefix(bucketName);

            PutObjectWithContentDispositionHeader(bucketName);
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

        public static void PutObjectFromFileWithTimeout(string bucketName)
        {
            var _configuration = new ClientConfiguration();
            _configuration.ConnectionTimeout = 20000; 
            var _client = new OssClient(endpoint, accessKeyId, accessKeySecret, _configuration);

            const string key = "PutObjectFromFile";
            try
            {
                _client.PutObject(bucketName, key, fileToUpload);
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

        private static string GetMd5Hash(string str)
        {
            var output = new byte[128];
            var buffer = System.Text.Encoding.Default.GetBytes(str);
            hashAlgorithm.TransformBlock(buffer, 0, str.Length, output, 0);
            hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
            var md5 = BitConverter.ToString(hashAlgorithm.Hash).ToLower();
            md5 = md5.Replace("-", "");
            return md5;
        }

        public static void PutObjectFromStringWithHashPrefix(string bucketName)
        {
            DateTime begin = new DateTime(2018,6,1);
            DateTime end = new DateTime(2018,6,1);

            for (var i = begin; i <= end; i = i.AddDays(1))
            {
                var hash_prefix = "camera_01/" + i.Year + "-" + i.Month.ToString().PadLeft(2, '0');
                var key_prefix  = GetMd5Hash(hash_prefix).Substring(0, 4) + "/" + hash_prefix + "-" + i.Day.ToString().PadLeft(2, '0') + "/";

                for (var j = 1; j < 2; j++)
                {
                    var key = key_prefix + j.ToString().PadLeft(8, '0') + ".dat";
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

            string md5;
            using (var fs = File.Open(fileToUpload, FileMode.Open))
            {
                md5 = OssUtils.ComputeContentMd5(fs, fs.Length);
            }

            var meta = new ObjectMetadata() { ContentMd5 = md5 };
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
                client.EndPutObject(ar);

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

        public static void PutObjectWithContentDispositionHeader(string bucketName)
        {
            const string key = "PutObjectWithContentDispositionHeader";
            try
            {
                using (var content = File.Open(fileToUpload, FileMode.Open))
                {
                    var saveAs = "测试123.txt";
                    var contentDisposition = string.Format("attachment;filename*=utf-8''{0}", HttpUtils.EncodeUri(saveAs, "utf-8"));

                    var metadata = new ObjectMetadata();
                    metadata.ContentLength = content.Length;
                    metadata.ContentDisposition = contentDisposition;
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
    }
}
