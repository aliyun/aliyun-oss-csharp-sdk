/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class UploadPartCopyResultDeserializer : ResponseDeserializer<UploadPartCopyResult, UploadPartCopyRequestModel>
    {
        private readonly int _partNumber;

        public UploadPartCopyResultDeserializer(IDeserializer<Stream, UploadPartCopyRequestModel> contentDeserializer, int partNumber)
            : base(contentDeserializer)
        {
            _partNumber = partNumber;
        }
        
        public override UploadPartCopyResult Deserialize(ServiceResponse xmlStream)
        {
            var partCopyRequestModel = ContentDeserializer.Deserialize(xmlStream.Content);
            var result = new UploadPartCopyResult
            {
                ETag = OssUtils.TrimQuotes(partCopyRequestModel.ETag),
                PartNumber = _partNumber
            };

            DeserializeGeneric(xmlStream, result);

            if (result.ResponseMetadata.ContainsKey(HttpHeaders.HashCrc64Ecma))
            {
                result.Crc64 = result.ResponseMetadata[HttpHeaders.HashCrc64Ecma];
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.QuotaDeltaSize))
            {
                result.Length = long.Parse(xmlStream.Headers[HttpHeaders.QuotaDeltaSize]);
            }

            if (xmlStream.Headers.ContainsKey("x-oss-copy-source-version-id"))
            {
                result.CopySourceVersionId = xmlStream.Headers["x-oss-copy-source-version-id"];
            }

            return result;
        }
    }
}
