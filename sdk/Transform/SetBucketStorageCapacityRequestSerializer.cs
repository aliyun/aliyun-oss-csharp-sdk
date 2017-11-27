/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
using System.IO;

namespace Aliyun.OSS.Transform
{
    internal class SetBucketStorageCapacityRequestSerializer : RequestSerializer<SetBucketStorageCapacityRequest, BucketStorageCapacityModel>
    {
        public SetBucketStorageCapacityRequestSerializer(ISerializer<BucketStorageCapacityModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(SetBucketStorageCapacityRequest request)
        {
            var model = new BucketStorageCapacityModel
            {
                StorageCapacity = request.StorageCapacity,
            };            
            return ContentSerializer.Serialize(model);
        }
    }
}
