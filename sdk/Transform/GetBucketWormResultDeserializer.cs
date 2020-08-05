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
    internal class GetBucketWormResultDeserializer : ResponseDeserializer<GetBucketWormResult, WormConfigurationModel>
    {
        public GetBucketWormResultDeserializer(IDeserializer<Stream, WormConfigurationModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override GetBucketWormResult Deserialize(ServiceResponse xmlStream)
        {
            GetBucketWormResult result = new GetBucketWormResult();

            var mode = ContentDeserializer.Deserialize(xmlStream.Content);

            result.WormId = mode.WormId;
            result.State = mode.State;
            result.RetentionPeriodInDays = mode.Days;
            result.CreationDate = mode.CreationDate;
            this.DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}

