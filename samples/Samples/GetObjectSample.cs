/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for getting object.
    /// </summary>
   public static class GetObjectSample
   {
        const string accessKeyId = "<your access key id>";
        const string accessKeySecret = "<your access key secret>";
        const string endpoint = "<valid host name>";


       static OssClient ossClient = new OssClient(new Uri(endpoint), accessKeyId, accessKeySecret);

        const string bucketName = "<your bucket name>";
        const string key = "TestOSS";
        const string fileToUpload = "<your file to upload path>";
        const string fileToDownload = "<your file to download path>";

        static AutoResetEvent _event = new AutoResetEvent(false);

       public static void GetObject()
       {
           try
           {
                string eTag;
                using (Stream fs = File.Open(fileToUpload, FileMode.Open))
                {
                    // compute content's md5
                    eTag = ComputeContentMd5(fs);
                }

                // put object
                var metadata = new ObjectMetadata {ETag = eTag};
                ossClient.PutObject(bucketName, key, fileToUpload, metadata);

               Console.WriteLine("PutObject done.");

                // verify etag
                var o = ossClient.GetObject(bucketName, key);
                using (var requestStream = o.Content)
                {
                    int len = (int) o.Metadata.ContentLength;
                    var buf = new byte[len];
                    requestStream.Read(buf, 0, len);                  
                    var fs = File.Open(fileToDownload, FileMode.OpenOrCreate);
                    fs.Write(buf, 0, len);
                }

               Console.WriteLine("GetObject done.");
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

        private static string ComputeContentMd5(Stream inputStream)
        {
            using (var md5 = MD5.Create())
            {
                var data = md5.ComputeHash(inputStream);
                var sBuilder = new StringBuilder();
                foreach (var t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
    }
}
