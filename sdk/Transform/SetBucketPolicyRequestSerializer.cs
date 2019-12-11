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
    internal class SetBucketPolicyRequestSerializer : RequestSerializer<SetBucketPolicyRequest, string>
    {
        public SetBucketPolicyRequestSerializer(ISerializer<string, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(SetBucketPolicyRequest request)
        {
            return ContentSerializer.Serialize(request.Policy);
        }
    }
}
