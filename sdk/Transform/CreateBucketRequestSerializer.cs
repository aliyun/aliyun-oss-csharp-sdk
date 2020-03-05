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
    internal class CreateBucketRequestSerializer : RequestSerializer<CreateBucketRequest, CreateBucketRequestModel>
    {
        public CreateBucketRequestSerializer(ISerializer<CreateBucketRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(CreateBucketRequest request)
        {
            var model = new CreateBucketRequestModel
            {
                StorageClass = request.StorageClass,
                DataRedundancyType = request.DataRedundancyType
            };
            return ContentSerializer.Serialize(model);
        }
    }
}
