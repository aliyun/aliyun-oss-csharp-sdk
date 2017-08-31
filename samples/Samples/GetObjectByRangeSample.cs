/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for getting object by range.
    /// </summary>
    public static class GetObjectByRangeSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        static string key = "GetObjectSample";
        static string dirToDownload = Config.DirToDownload;

        public static void GetObjectPartly(string bucketName)
        {
            client.PutObject(bucketName, key, Config.BigFileToUpload);

            string localFilePath = dirToDownload + "/sample.3.data";
            using (var fileStream = new FileStream(localFilePath, FileMode.OpenOrCreate))
            {
                var bufferedStream = new BufferedStream(fileStream);
                var objectMetadata = client.GetObjectMetadata(bucketName, key);
                var fileLength = objectMetadata.ContentLength;
                const int partSize = 1024 * 1024 * 10;

                var partCount = CalPartCount(fileLength, partSize);

                for (var i = 0; i < partCount; i++)
                {
                    var startPos = partSize * i;
                    var endPos = partSize * i + (partSize < (fileLength - startPos) ? partSize : (fileLength - startPos)) - 1;
                    Download(bufferedStream, startPos, endPos, localFilePath, bucketName, key);
                }
                bufferedStream.Flush();
            }
        }

        /// <summary>
        /// 计算下载的块数
        /// </summary>
        /// <param name="fileLength"></param>
        /// <param name="partSize"></param>
        /// <returns></returns>
        private static int CalPartCount(long fileLength, long partSize)
        {
            var partCount = (int)(fileLength / partSize);
            if (fileLength % partSize != 0)
            {
                partCount++;
            }
            return partCount;
        }

        /// <summary>
        /// 分块下载
        /// </summary>
        /// <param name="bufferedStream"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="localFilePath"></param>
        /// <param name="bucketName"></param>
        /// <param name="fileKey"></param>
        private static void Download(BufferedStream bufferedStream, long startPos, long endPos, String localFilePath, String bucketName, String fileKey)
        {
            Stream contentStream = null;
            try
            {
                var getObjectRequest = new GetObjectRequest(bucketName, fileKey);
                getObjectRequest.SetRange(startPos, endPos);
                var ossObject = client.GetObject(getObjectRequest);
                byte[] buffer = new byte[1024 * 1024];
                var bytesRead = 0;
                bufferedStream.Seek(startPos, SeekOrigin.Begin);
                contentStream = ossObject.Content;
                while ((bytesRead = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bufferedStream.Write(buffer, 0, bytesRead);
                }
            }
            finally
            {
                if (contentStream != null)
                {
                    contentStream.Dispose();
                }
            }
        }

    }
}
