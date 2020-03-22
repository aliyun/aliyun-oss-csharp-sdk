/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class PutObjectResponseDeserializer : ResponseDeserializer<PutObjectResult, PutObjectResult>
    {
        private readonly PutObjectRequest _putObjectRequest;

        public PutObjectResponseDeserializer(PutObjectRequest putObjectRequest)
            : base(null)
        {
            _putObjectRequest = putObjectRequest;
        }
        
        public override PutObjectResult Deserialize(ServiceResponse xmlStream)
        {
            var result = new PutObjectResult();

            if (xmlStream.Headers.ContainsKey(HttpHeaders.ETag))
            {
                result.ETag = OssUtils.TrimQuotes(xmlStream.Headers[HttpHeaders.ETag]);
            }
            if (_putObjectRequest.IsNeedResponseStream())
            {
                result.ResponseStream = xmlStream.Content;
            }
            if (xmlStream.Headers.ContainsKey(HttpHeaders.VersionId))
            {
                result.VersionId = xmlStream.Headers[HttpHeaders.VersionId];
            }

            DeserializeGeneric(xmlStream, result);

            return result;
        }
    }
}
