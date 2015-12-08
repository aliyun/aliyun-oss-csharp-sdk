/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;
using System.Net;
using Aliyun.OSS.Common;
using System.Text;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for the usage of URL Signature.
    /// </summary>
    public static class UrlSignatureSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string fileToSave = Config.DirToDownload + "/sample.data";
        static string fileToUpload = Config.FileToUpload;

        public static void UrlSignature(string bucketName)
        {
            const string key = "UrlSignature";

            PutLocalFileBySignedUrl(bucketName, key);

            PutStreamBySignedUrl(bucketName, key);

            GetObjectBySignedUrlWithWebRequest(bucketName, key);

            GetObjectBySignedUrlWithClient(bucketName, key);
        }

        public static void GetObjectBySignedUrlWithWebRequest(string bucketName, string key)
        {
            try
            {
                var metadata = client.GetObjectMetadata(bucketName, key);
                var etag = metadata.ETag;

                var req = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Get);
                req.AddQueryParam("param1", "value1");
                req.ContentType = "text/html";
                req.ContentMd5 = etag;
                req.AddUserMetadata("mk1", "mv1");
                req.ResponseHeaders.CacheControl = "No-Cache";
                req.ResponseHeaders.ContentEncoding = "utf-8";
                req.ResponseHeaders.ContentType = "text/html";

                // Generates url signature for accessing specified object.
                var uri = client.GeneratePresignedUri(req);

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.ContentType = "text/html";
                webRequest.Headers.Add(HttpRequestHeader.ContentMd5, etag);
                webRequest.Headers.Add("x-oss-meta-mk1", "mv1");
                var resp = webRequest.GetResponse() as HttpWebResponse;
                var output = resp.GetResponseStream();
                var bufferSize = 2048;
                var bytes = new byte[bufferSize];
                try
                {
                    using (StreamWriter outfile = new StreamWriter(fileToSave)) 
                    {
                        var length = 0;
                        do
                        {
                            length = output.Read(bytes, 0, bufferSize);
                            outfile.Write(bytes);
                        } while (length > 0);
                    }
                    output.Close();

                    Console.WriteLine("Get object by signatrue succeeded.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ex : " + ex.Message);
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

        public static void GetObjectBySignedUrlWithClient(string bucketName, string key)
        {
            try
            {
                var metadata = client.GetObjectMetadata(bucketName, key);
                var etag = metadata.ETag;

                var req = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Get);
                // Generates url signature for accessing specified object.
                var uri = client.GeneratePresignedUri(req);

                OssObject ossObject = client.GetObject(uri);
                using (var file = File.Open("fileToSave", FileMode.OpenOrCreate))
                {
                    using (Stream stream = ossObject.Content)
                    {
                        int length = 4 * 1024;
                        var buf = new byte[length];
                        do
                        {
                            length = stream.Read(buf, 0, length);
                            file.Write(buf, 0, length);
                        } while (length != 0);
                    }
                }

                Console.WriteLine("Get object by signatrue succeeded.");
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

        public static void PutLocalFileBySignedUrl(string bucketName, string key)
        {
            OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            try
            {
                // Step1: Genereates url signature
                var request = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Put);
                request.Expiration = DateTime.Now.AddMinutes(5); 

                var signedUrl = client.GeneratePresignedUri(request);

                // Step2: Prepares for filepath to be uploaded and sends out this request.
                client.PutObject(signedUrl, fileToUpload);

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

        public static void PutStreamBySignedUrl(string bucketName, string key)
        {
            OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            try
            {
                // Step1: Genereates url signature
                var generatePresignedUriRequest = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Put);
                var signedUrl = client.GeneratePresignedUri(generatePresignedUriRequest);

                // Step2: Prepares for stream to be uploaded and sends out this request.
                var buffer = Encoding.UTF8.GetBytes("Aliyun OSS SDK for C#");
                using (var ms = new MemoryStream(buffer))
                {
                    client.PutObject(signedUrl, ms);
                }

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
