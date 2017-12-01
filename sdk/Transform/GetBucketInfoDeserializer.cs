/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class GetBucketInfoDeserializer : ResponseDeserializer<BucketInfo, BucketInfo>
    {
        public GetBucketInfoDeserializer(IDeserializer<Stream, BucketInfo> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override BucketInfo Deserialize(ServiceResponse xmlStream)
        {
            return ContentDeserializer.Deserialize(xmlStream.Content);
        }
    }
}
