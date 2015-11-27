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

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for the usage of URL Signature.
    /// </summary>
    public static class UrlSignatureSample
    {
        public static void getObjectBySignedUrl()
        {
            const string accessKeyId = "<your access key id>";
            const string accessKeySecret = "<your access key secret>";
            const string endpoint = "<valid host name>";

            const string bucketName = "<bucket name>";
            const string key = "<object name>";

            OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            try
            {
                var metadata = client.GetObjectMetadata(bucketName, key);
                var etag = metadata.ETag;

                var req = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Get);
                // Set optional properties(be blind to them usually) 
                req.AddQueryParam("param1", "value1");
                req.ContentType = "text/html";
                req.ContentMd5 = etag;
                req.AddUserMetadata("mk1", "mv1");
                req.AddUserMetadata("mk2", "mv2");
                req.ResponseHeaders.CacheControl = "No-Cache";
                req.ResponseHeaders.ContentEncoding = "utf-8";
                req.ResponseHeaders.ContentType = "text/html";
                // Generates url signature for accessing specified object.
                var uri = client.GeneratePresignedUri(req);

                Console.WriteLine(uri.ToString());

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.ContentType = "text/html";
                webRequest.Headers.Add(HttpRequestHeader.ContentMd5, etag);
                webRequest.Headers.Add("x-oss-meta-mk1", "mv1");
                webRequest.Headers.Add("x-oss-meta-mk2", "mv2");
                var resp = webRequest.GetResponse() as HttpWebResponse;
                var output = resp.GetResponseStream();
                var bufferSize = 2048;
                var bytes = new byte[bufferSize];
                try
                {
                    var length = 0;
                    do
                    {
                        length = output.Read(bytes, 0, bufferSize);
                        // to do something with bytes...
                    } while (length > 0);
                    output.Close();
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

        public static void getObjectBySignedUrl2()
        {
            const string accessKeyId = "<your access key id>";
            const string accessKeySecret = "<your access key secret>";
            const string endpoint = "<valid host name>";

            const string bucketName = "<bucket name>";
            const string key = "<object name>";

            OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            try
            {
                var metadata = client.GetObjectMetadata(bucketName, key);
                var etag = metadata.ETag;

                var req = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Get);
                // Generates url signature for accessing specified object.
                var uri = client.GeneratePresignedUri(req);

                Console.WriteLine(uri.ToString());

                OssObject ossObject = client.GetObject(uri);
                var file = File.Open("<file to hold object content>", FileMode.OpenOrCreate);
                Stream stream = ossObject.Content;
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                file.Write(bytes, 0, bytes.Length);
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

        public static void putObjectBySignedUrl()
        {
            const string accessKeyId = "<your access key id>";
            const string accessKeySecret = "<your access key secret>";
            const string endpoint = "<valid host name>";

            const string bucketName = "<bucket name>";
            const string key = "<object name>";

            //const string postData = "<your data to upload>";
            const string fileToUpload = "<file to upload>";

            OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            try
            {
                // Puts object by using URL Signature Style(two steps included).
                // Step1: Genereates url signature
                var generatePresignedUriRequest = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Put);
                var signedUrl = client.GeneratePresignedUri(generatePresignedUriRequest);

                // Step2: Prepares for stream to be uploaded and sends out this request.
                //var buffer = Encoding.UTF8.GetBytes(postData);
                //PutObjectResult result = null;
                //using (var ms = new MemoryStream(buffer))
                //{
                //    result = client.PutObject(signedUrl, ms);
                //}

                // Step2: Prepares for filepath to be uploaded and sends out this request.
                var result = client.PutObject(signedUrl, fileToUpload);
                
                Console.WriteLine("Uploaded File's ETag: {0}", result.ETag);
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
