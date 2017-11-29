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
    internal class CreateBucketRequestSerializer : RequestSerializer<StorageClass, CreateBucketRequestModel>
    {
        public CreateBucketRequestSerializer(ISerializer<CreateBucketRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        {
        }

        public override Stream Serialize(StorageClass request)
        {
            var model = new CreateBucketRequestModel
            {
                StorageClass = request
            };

            return ContentSerializer.Serialize(model);
        }
    }
}
