/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Threading;
using Aliyun.OSS.Common;
using Aliyun.OSS.Common.Internal;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for putting object.
    /// </summary>
    public static class AppendObjectSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string fileToUpload = Config.FileToUpload;

        static AutoResetEvent _event = new AutoResetEvent(false);

        /// <summary>
        /// sample for append object to oss
        /// </summary>
        public static void AppendObject(string bucketName)
        {
            SyncAppendObject(bucketName);
            AsyncAppendObject(bucketName);
            SyncAppendObjectWithPartSize(bucketName);
        }

        public static void SyncAppendObject(string bucketName)
        {
            const string key = "AppendObject";
            long position = 0;
            ulong initCrc = 0;
            try
            {
                var metadata = client.GetObjectMetadata(bucketName, key);
                position = metadata.ContentLength;
                initCrc = ulong.Parse(metadata.Crc64);
            }
            catch(Exception)  {}

            try
            {
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var request = new AppendObjectRequest(bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position,
                        InitCrc = initCrc
                    };

                    var result = client.AppendObject(request);
                    position = result.NextAppendPosition;
                    initCrc = result.HashCrc64Ecma;

                    Console.WriteLine("Append object succeeded, next append position:{0}", position);
                }

                // append object by using NextAppendPosition
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var request = new AppendObjectRequest(bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position,
                        InitCrc = initCrc
                    };

                    var result = client.AppendObject(request);
                    position = result.NextAppendPosition;

                    Console.WriteLine("Append object succeeded too, next append position:{0}", position);
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

        public static void AsyncAppendObject(string bucketName)
        {
            const string key = "AsyncAppendObject";
            long position = 0;
            try
            {
                var metadata = client.GetObjectMetadata(bucketName, key);
                position = metadata.ContentLength;
            }
            catch (Exception) { }

            try
            {
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    var request = new AppendObjectRequest(bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position
                    };


                    const string notice = "Append object succeeded";
                    client.BeginAppendObject(request, AppendObjectCallback, notice.Clone());

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

        public static void SyncAppendObjectWithPartSize(string bucketName)
        {
            const long partSize = 1 * 1024 * 1024;
            const string key = "AppendObjectWithPartsize";
            long position = 0;
            ulong initCrc = 0;
            try
            {
                var metadata = client.GetObjectMetadata(bucketName, key);
                position = metadata.ContentLength;
                initCrc = ulong.Parse(metadata.Crc64);
            }
            catch (Exception) { }

            try
            {
                using (var fs = File.Open(fileToUpload, FileMode.Open))
                {
                    do
                    {
                        fs.Seek(position, SeekOrigin.Begin);
                        var request = new AppendObjectRequest(bucketName, key)
                        {
                            ObjectMetadata = new ObjectMetadata(),
                            Content = new PartialWrapperStream(fs, partSize),
                            Position = position,
                            InitCrc = initCrc
                        };
                        var result = client.AppendObject(request);
                        position = result.NextAppendPosition;
                        initCrc = result.HashCrc64Ecma;
                        Console.WriteLine("Append object succeeded, next append position:{0}, crc64:{1}", position, initCrc);
                    } while (position < fs.Length);
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

        private static void AppendObjectCallback(IAsyncResult ar)
        {
            try
            {
                var result = client.EndAppendObject(ar);
                Console.WriteLine("Append object succeeded, next append position:{0}", result.NextAppendPosition);
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
