/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using System.IO;
using System.Xml;

namespace Aliyun.OSS.Transform
{
    internal class GetBucketEncryptionResultDeserializer : ResponseDeserializer<BucketEncryptionResult, ServerSideEncryptionRule>
    {
        public GetBucketEncryptionResultDeserializer(IDeserializer<Stream, ServerSideEncryptionRule> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override BucketEncryptionResult Deserialize(ServiceResponse xmlStream)
        {
            var result = new BucketEncryptionResult();
            var mode = ContentDeserializer.Deserialize(xmlStream.Content);

            result.SSEAlgorithm = mode.ApplyServerSideEncryptionByDefault.SSEAlgorithm;
            result.KMSMasterKeyID = mode.ApplyServerSideEncryptionByDefault.KMSMasterKeyID;

            this.DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}
