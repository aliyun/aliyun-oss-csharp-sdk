/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using System.IO;

namespace Aliyun.OSS.Transform
{
    internal class GetBucketStorageCapacityResultDeserializer : ResponseDeserializer<GetBucketStorageCapacityResult, BucketStorageCapacityModel>
    {
        public GetBucketStorageCapacityResultDeserializer(IDeserializer<Stream, BucketStorageCapacityModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override GetBucketStorageCapacityResult Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);
            var getBucketStorageCapacityResult = new GetBucketStorageCapacityResult
            {
                StorageCapacity = model.StorageCapacity
            };

            DeserializeGeneric(xmlStream, getBucketStorageCapacityResult);

            return getBucketStorageCapacityResult;
        }
    }
}
