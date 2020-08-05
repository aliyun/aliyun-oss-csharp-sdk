/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class InitiateBucketWormRequestSerializer : RequestSerializer<InitiateBucketWormRequest, InitiateBucketWormModel>
    {
        public InitiateBucketWormRequestSerializer(ISerializer<InitiateBucketWormModel, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(InitiateBucketWormRequest request)
        {
            var model = new InitiateBucketWormModel();
            model.Days = request.RetentionPeriodInDays;
            return ContentSerializer.Serialize(model);
        }
    }
}

