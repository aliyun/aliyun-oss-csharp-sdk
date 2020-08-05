/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class ExtendBucketWormRequestSerializer : RequestSerializer<ExtendBucketWormRequest, ExtendBucketWormModel>
    {
        public ExtendBucketWormRequestSerializer(ISerializer<ExtendBucketWormModel, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(ExtendBucketWormRequest request)
        {
            var model = new ExtendBucketWormModel();
            model.Days = request.RetentionPeriodInDays;
            return ContentSerializer.Serialize(model);
        }
    }
}

