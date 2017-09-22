/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class UploadPartResultDeserializer : ResponseDeserializer<UploadPartResult, UploadPartResult>
    {
        private readonly int _partNumber;
        
        public UploadPartResultDeserializer(int partNumber)
            : base(null)
        {
            _partNumber = partNumber;
        }
        
        public override UploadPartResult Deserialize(ServiceResponse xmlStream)
        {
            var result = new UploadPartResult();

            if (xmlStream.Headers.ContainsKey(HttpHeaders.ETag))
            {
                result.ETag = OssUtils.TrimQuotes(xmlStream.Headers[HttpHeaders.ETag]);
            }
            result.PartNumber = _partNumber;

            DeserializeGeneric(xmlStream, result);

            return result;
        }
    }
}
