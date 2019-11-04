/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
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
       static string accessKeyId = Config.AccessKeyId;
       static string accessKeySecret = Config.AccessKeySecret;
       static string endpoint = Config.Endpoint;
       static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

       static string key = "GetObjectSample";
       static string fileToUpload = Config.FileToUpload;
       static string dirToDownload = Config.DirToDownload;

       static AutoResetEvent _event = new AutoResetEvent(false);

       public static void GetObjects(string bucketName)
       {
           GetObject(bucketName);

           GetObjectByRequest(bucketName);

           AsyncGetObject(bucketName);
       }

       public static void GetObject(string bucketName)
       {
           try
           {
               client.PutObject(bucketName, key, fileToUpload);

               var result = client.GetObject(bucketName, key);

               using (var requestStream = result.Content)
               {
                   using (var fs = File.Open(dirToDownload + "/sample.data", FileMode.OpenOrCreate))
                   {
                       int length = 4 * 1024;
                       var buf = new byte[length]; 
                       do{
                           length = requestStream.Read(buf, 0, length);
                           fs.Write(buf, 0, length);
                       } while (length != 0);
                   }
               }

               Console.WriteLine("Get object succeeded");
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

       public static void GetObjectByRequest(string bucketName)
       {
           try
           {
               client.PutObject(bucketName, key, fileToUpload);

               var request = new GetObjectRequest(bucketName, key);
               request.SetRange(0, 100);

               var result = client.GetObject(request);

               Console.WriteLine("Get object succeeded, length:{0}", result.Metadata.ContentLength);
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

       public static void AsyncGetObject(string bucketName)
       {
           const string key = "AsyncGetObject";
           try
           {
               client.PutObject(bucketName, key, fileToUpload);

               string result = "Notice user: put object finish";
               client.BeginGetObject(bucketName, key, GetObjectCallback, result.Clone());

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

       private static void GetObjectCallback(IAsyncResult ar)
       {
           try
           {
               var result = client.EndGetObject(ar);

               using (var requestStream = result.Content)
               {
                   using (var fs = File.Open(dirToDownload + "/sample2.data", FileMode.OpenOrCreate))
                   {
                       int length;
                       int bufLength = 4 * 1024;
                       var buf = new byte[bufLength];
                       do
                       {
                           length = requestStream.Read(buf, 0, bufLength);
                           fs.Write(buf, 0, length);
                       } while (length != 0);
                   }
               }

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
