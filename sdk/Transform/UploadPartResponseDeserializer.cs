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
        private readonly long _length;
        
        public UploadPartResultDeserializer(int partNumber, long length = 0)
            : base(null)
        {
            _partNumber = partNumber;
            _length = length;
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

            if (result.ResponseMetadata.ContainsKey(HttpHeaders.HashCrc64Ecma))
            {
                result.Crc64 = result.ResponseMetadata[HttpHeaders.HashCrc64Ecma];
            }

            result.Length = _length;

            return result;
        }
    }
}
