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
    internal class SetBucketEncryptionRequestSerializer : RequestSerializer<SetBucketEncryptionRequest, ServerSideEncryptionRule>
    {
        public SetBucketEncryptionRequestSerializer(ISerializer<ServerSideEncryptionRule, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(SetBucketEncryptionRequest request)
        {
            var model = new ServerSideEncryptionRule()
            {
                ApplyServerSideEncryptionByDefault = new ServerSideEncryptionRule.ApplyServerSideEncryptionByDefaultModel()
            };

            model.ApplyServerSideEncryptionByDefault.SSEAlgorithm = request.SSEAlgorithm;
            model.ApplyServerSideEncryptionByDefault.KMSMasterKeyID = request.KMSMasterKeyID;
            return ContentSerializer.Serialize(model);
        }
    }
}

