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
    internal class GetBucketLoggingResultDeserializer 
        : ResponseDeserializer<BucketLoggingResult, SetBucketLoggingRequestModel>
    {
        public GetBucketLoggingResultDeserializer(IDeserializer<Stream, SetBucketLoggingRequestModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override BucketLoggingResult Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);
            var bucketLoggingResult = new BucketLoggingResult
            {
                TargetBucket = model.LoggingEnabled.TargetBucket,
                TargetPrefix = model.LoggingEnabled.TargetPrefix
            };

            DeserializeGeneric(xmlStream, bucketLoggingResult);

            return bucketLoggingResult;
       }
    }
}
