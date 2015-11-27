/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
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
        private static OssClient _client = null;

        public static void GetObjectPartly()
        {
            const string localFilePath = "<your localFilePath>";
            const string bucketName = "<your bucketName>";
            const string fileKey = "<your fileKey>";
            const string accessKeyId = "<your access key id>";
            const string accessKeySecret = "<your access key secret>";
            const string endpoint = "<your endpoint>";

            _client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            using (var fileStream = new FileStream(localFilePath, FileMode.OpenOrCreate))
            {
                var bufferedStream = new BufferedStream(fileStream);
                var objectMetadata = _client.GetObjectMetadata(bucketName, fileKey);
                var fileLength = objectMetadata.ContentLength;
                const int partSize = 1024 * 1024 * 10;

                var partCount = CalPartCount(fileLength, partSize);

                for (var i = 0; i < partCount; i++)
                {
                    var startPos = partSize * i;
                    var endPos = partSize * i + (partSize < (fileLength - startPos) ? partSize : (fileLength - startPos)) - 1;
                    Download(bufferedStream, startPos, endPos, localFilePath, bucketName, fileKey);
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
                var ossObject = _client.GetObject(getObjectRequest);
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
