/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using Aliyun.OSS.Model;
using System.IO;

namespace Aliyun.OSS.Transform
{
    internal class SetBucketLoggingRequestSerializer : RequestSerializer<SetBucketLoggingRequest, SetBucketLoggingRequestModel>
    {
        public SetBucketLoggingRequestSerializer(ISerializer<SetBucketLoggingRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(SetBucketLoggingRequest request)
        {
            var model = new SetBucketLoggingRequestModel();

            var setBucketLoggingEnabled = new SetBucketLoggingRequestModel.SetBucketLoggingEnabled
            {
                TargetBucket = request.TargetBucket,
                TargetPrefix = request.TargetPrefix
            };
            model.LoggingEnabled = setBucketLoggingEnabled;
            
            return ContentSerializer.Serialize(model);
        }
    }
}
