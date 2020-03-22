/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class CompleteMultipartUploadResultDeserializer 
        : ResponseDeserializer<CompleteMultipartUploadResult, CompleteMultipartUploadResultModel>
    {
        private readonly CompleteMultipartUploadRequest _completeMultipartUploadRequest;

        public CompleteMultipartUploadResultDeserializer(IDeserializer<Stream, CompleteMultipartUploadResultModel> contentDeserializer,
            CompleteMultipartUploadRequest completeMultipartUploadRequest)
            : base(contentDeserializer)
        {
            _completeMultipartUploadRequest = completeMultipartUploadRequest;
        }
        
        public override CompleteMultipartUploadResult Deserialize(ServiceResponse xmlStream)
        {
            var completeMultipartUploadResult = new CompleteMultipartUploadResult();

            if (_completeMultipartUploadRequest.IsNeedResponseStream())
            {
                completeMultipartUploadResult.BucketName = _completeMultipartUploadRequest.BucketName;
                completeMultipartUploadResult.Key = _completeMultipartUploadRequest.Key;
                completeMultipartUploadResult.ETag = OssUtils.TrimQuotes(xmlStream.Headers[HttpHeaders.ETag]);
                completeMultipartUploadResult.ResponseStream = xmlStream.Content;
            }
            else
            {
                var result = ContentDeserializer.Deserialize(xmlStream.Content);
                completeMultipartUploadResult.BucketName = result.Bucket;
                completeMultipartUploadResult.Key = result.Key;
                completeMultipartUploadResult.Location = result.Location;
                completeMultipartUploadResult.ETag = OssUtils.TrimQuotes(result.ETag);
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.VersionId))
            {
                completeMultipartUploadResult.VersionId = xmlStream.Headers[HttpHeaders.VersionId];
            }

            DeserializeGeneric(xmlStream, completeMultipartUploadResult);

            return completeMultipartUploadResult;
        }
    }
}
