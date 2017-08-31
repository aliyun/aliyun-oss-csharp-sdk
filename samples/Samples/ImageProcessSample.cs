/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Text;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for setting bucket acl.
    /// </summary>
    public static class ImageProcessSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string key = "ImageProcessSample.jpg";
        static string dirToDownload = Config.DirToDownload;
        static string imageFile = Config.ImageFileToUpload;

        public static void ImageProcess(string bucketName)
        {
            ResizeImage(bucketName);
            CropImage(bucketName);
            RotateImage(bucketName);
            SharpenImage(bucketName);
            WatermarkImage(bucketName);
            FormatImage(bucketName);
            InfoImage(bucketName);

            GenerateIamgeUri(bucketName);
        }

        public static void ResizeImage(string bucketName)
        {
            try
            {
                client.PutObject(bucketName, key, imageFile);

                // 图片缩放
                var process = "image/resize,m_fixed,w_100,h_100";
                var ossObject = client.GetObject(new GetObjectRequest(bucketName, key, process));

                WriteToFile(dirToDownload + "/sample.pdf", ossObject.Content);

                Console.WriteLine("Get Object:{0} with process:{1} succeeded ", key, process);
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

        public static void CropImage(string bucketName)
        {
            try
            {
                client.PutObject(bucketName, key, imageFile);

                // 图片裁剪
                var process = "image/crop,w_100,h_100,x_100,y_100,r_1";
                var ossObject = client.GetObject(new GetObjectRequest(bucketName, key, process));

                WriteToFile(dirToDownload + "/sample.pdf", ossObject.Content);

                Console.WriteLine("Get Object:{0} with process:{1} succeeded ", key, process);
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

        public static void RotateImage(string bucketName)
        {
            try
            {
                client.PutObject(bucketName, key, imageFile);

                // 图片旋转
                var process = "image/rotate,90";
                var ossObject = client.GetObject(new GetObjectRequest(bucketName, key, process));

                WriteToFile(dirToDownload + "/sample.pdf", ossObject.Content);

                Console.WriteLine("Get Object:{0} with process:{1} succeeded ", key, process);
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

        public static void SharpenImage(string bucketName)
        {
            try
            {
                client.PutObject(bucketName, key, imageFile);

                // 图片锐化
                var process = "image/sharpen,100";
                var ossObject = client.GetObject(new GetObjectRequest(bucketName, key, process));

                WriteToFile(dirToDownload + "/sample.pdf", ossObject.Content);

                Console.WriteLine("Get Object:{0} with process:{1} succeeded ", key, process);
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

        public static void WatermarkImage(string bucketName)
        {
            try
            {
                client.PutObject(bucketName, key, imageFile);

                // 图片加文字水印
                var process = "image/watermark,text_SGVsbG8g5Zu-54mH5pyN5YqhIQ";
                var ossObject = client.GetObject(new GetObjectRequest(bucketName, key, process));

                WriteToFile(dirToDownload + "/sample.pdf", ossObject.Content);

                Console.WriteLine("Get Object:{0} with process:{1} succeeded ", key, process);
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

        public static void FormatImage(string bucketName)
        {
            try
            {
                client.PutObject(bucketName, key, imageFile);

                // 图片格式转换
                var process = "image/format,png";
                var ossObject = client.GetObject(new GetObjectRequest(bucketName, key, process));

                WriteToFile(dirToDownload + "/sample.pdf", ossObject.Content);

                Console.WriteLine("Get Object:{0} with process:{1} succeeded ", key, process);
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

        public static void InfoImage(string bucketName)
        {
            try
            {
                client.PutObject(bucketName, key, imageFile);

                // 图片信息
                var process = "image/info";
                var ossObject = client.GetObject(new GetObjectRequest(bucketName, key, process));

                WriteToFile(dirToDownload + "/sample.pdf", ossObject.Content);

                Console.WriteLine("Get Object:{0} with process:{1} succeeded ", key, process);
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

        public static void GenerateIamgeUri(string bucketName)
        {
            try
            {
               
                var process = "image/resize,m_fixed,w_100,h_100";
                var req = new GeneratePresignedUriRequest(bucketName, key, SignHttpMethod.Get)
                {
                    Expiration = DateTime.Now.AddHours(1),
                    Process = process
                };

                // 产生带有签名的URI
                var uri = client.GeneratePresignedUri(req);

                Console.WriteLine("Generate Presigned Uri:{0} with process:{1} succeeded ", uri, process);
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

        private static void WriteToFile(string filePath, Stream stream)
        {
            using (var requestStream = stream)
            {
                using (var fs = File.Open(filePath, FileMode.OpenOrCreate))
                {
                    IoUtils.WriteTo(stream, fs);
                }
            }
        }
    }
}
