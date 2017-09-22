/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class InitiateMultipartUploadResultDeserializer 
        : ResponseDeserializer<InitiateMultipartUploadResult, InitiateMultipartResult>
    {
        public InitiateMultipartUploadResultDeserializer(IDeserializer<Stream, InitiateMultipartResult> contentDeserializer)
                 : base(contentDeserializer)
        { }
        
        public override InitiateMultipartUploadResult Deserialize(ServiceResponse xmlStream)
        {
            var result = ContentDeserializer.Deserialize(xmlStream.Content);
            string encodeType = result.EncodingType == null ?
                    string.Empty : result.EncodingType.ToLowerInvariant();
            
            var initiateMultipartUploadResult = new InitiateMultipartUploadResult
            {
                BucketName = result.Bucket,
                Key = Decode(result.Key, encodeType),
                UploadId = result.UploadId
            };

            DeserializeGeneric(xmlStream, initiateMultipartUploadResult);

            return initiateMultipartUploadResult;
        }
    }
}
