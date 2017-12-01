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
    internal class GetBucketStatDeserializer : ResponseDeserializer<BucketStat, BucketStat>
    {
        public GetBucketStatDeserializer(IDeserializer<Stream, BucketStat> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override BucketStat Deserialize(ServiceResponse xmlStream)
        {
            return ContentDeserializer.Deserialize(xmlStream.Content);
        }
    }
}
