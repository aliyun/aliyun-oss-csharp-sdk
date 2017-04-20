/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for the usage of GeneratePostPolicy.
    /// </summary>
    public static class PostPolicySample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        private static string ComputeSignature(string key, string data)
        {
            using (var algorithm = KeyedHashAlgorithm.Create("HmacSHA1".ToUpperInvariant()))
            {
                algorithm.Key = Encoding.UTF8.GetBytes(key.ToCharArray());
                return Convert.ToBase64String(
                    algorithm.ComputeHash(Encoding.UTF8.GetBytes(data.ToCharArray())));
            }
        }

        private static string BuildRequestUri(string endpoint, string bucketName)
        {   
            var requestUriBuilder = new StringBuilder();

            if (endpoint.StartsWith("http://")) 
            {
                requestUriBuilder.Append("http://");
                requestUriBuilder.Append(bucketName);
                requestUriBuilder.Append('.');
                requestUriBuilder.Append(endpoint.Substring("http://".Length));
            }
            else if (endpoint.StartsWith("https://"))
            {
                requestUriBuilder.Append("https://");
                requestUriBuilder.Append(bucketName);
                requestUriBuilder.Append('.');
                requestUriBuilder.Append(endpoint.Substring("https://".Length));
            }
            else
            {
                requestUriBuilder.Append("http://");
                requestUriBuilder.Append(bucketName);
                requestUriBuilder.Append('.');
                requestUriBuilder.Append(endpoint);
            }
            return requestUriBuilder.ToString();
        }

        public static void GenPostPolicy(string bucketName)
        {
            try
            {
                var expiration = DateTime.Now.AddMinutes(10);
                var policyConds = new PolicyConditions();
                policyConds.AddConditionItem("bucket", bucketName);
                // $ must be escaped with backslash.
                policyConds.AddConditionItem(MatchMode.Exact, PolicyConditions.CondKey, "user/eric/\\${filename}");
                policyConds.AddConditionItem(MatchMode.StartWith, PolicyConditions.CondKey, "user/eric");
                policyConds.AddConditionItem(MatchMode.StartWith, "x-oss-meta-tag", "dummy_etag");
                policyConds.AddConditionItem(PolicyConditions.CondContentLengthRange, 1, 1024);

                var postPolicy = client.GeneratePostPolicy(expiration, policyConds);
                var encPolicy = Convert.ToBase64String(Encoding.UTF8.GetBytes(postPolicy));
                Console.WriteLine("Generated post policy: {0}", postPolicy);

                var requestUri = BuildRequestUri(endpoint, bucketName);
                System.Console.WriteLine("RequestUri:" + requestUri);
                var boundary = "9431149156168";
                var webRequest = (HttpWebRequest)WebRequest.Create(requestUri);
                webRequest.Timeout = -1;
                webRequest.Method = "POST";
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

                var objectName = "xxx";
                var signature = ComputeSignature(accessKeySecret, encPolicy);

                var fileContent = "这是一行简单的测试文本";
                var requestBody = "--" + boundary + "\r\n" 
                        + "Content-Disposition: form-data; name=\"key\"\r\n"
                        + "\r\n" + "user/eric/${filename}" + "\r\n"
                        + "--" + boundary + "\r\n"
                        + "Content-Disposition: form-data; name=\"bucket\"\r\n"
                        + "\r\n" + bucketName + "\r\n"
                        + "--" + boundary + "\r\n"
                        + "Content-Disposition: form-data; name=\"x-oss-meta-tag\"\r\n"
                        + "\r\n" + "dummy_etag_xxx" + "\r\n"
                        + "--" + boundary + "\r\n"
                        + "Content-Disposition: form-data; name=\"OSSAccessKeyId\"\r\n"
                        + "\r\n" + accessKeyId + "\r\n"
                        + "--" + boundary + "\r\n"
                        + "Content-Disposition: form-data; name=\"policy\"\r\n"
                        + "\r\n" + encPolicy + "\r\n"
                        + "--" + boundary + "\r\n"
                        + "Content-Disposition: form-data; name=\"Signature\"\r\n"
                        + "\r\n" + signature + "\r\n"
                        + "--" + boundary + "\r\n"
                        + "Content-Disposition: form-data; name=\"file\"; filename=\"" + objectName + "\"\r\n\r\n" 
                        + fileContent + "\r\n" 
                        + "--" + boundary + "\r\n" 
                        + "Content-Disposition: form-data; name=\"submit\"\r\n\r\nUpload to OSS\r\n" 
                        + "--" + boundary + "--\r\n";

                webRequest.ContentLength = requestBody.Length;
                using (var ms = new MemoryStream())
                {
                    var writer = new StreamWriter(ms, new UTF8Encoding());
                    try
                    {
                        writer.Write(requestBody);
                        writer.Flush();
                        ms.Seek(0, SeekOrigin.Begin);

                        webRequest.ContentLength = ms.Length;
                        using (var requestStream = webRequest.GetRequestStream())
                        {
                            ms.WriteTo(requestStream);
                        }
                    }
                    finally
                    {
                        writer.Dispose();
                    }
                }

                var response = webRequest.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    Console.WriteLine("Post object succeed!");
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