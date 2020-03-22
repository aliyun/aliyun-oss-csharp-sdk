/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class SetBucketVersioningRequestSerializer : RequestSerializer<SetBucketVersioningRequest, VersioningConfiguration>
    {
        public SetBucketVersioningRequestSerializer(ISerializer<VersioningConfiguration, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(SetBucketVersioningRequest request)
        {
            var model = new VersioningConfiguration();
            model.Status = request.Status;
            return ContentSerializer.Serialize(model);
        }
    }
}

