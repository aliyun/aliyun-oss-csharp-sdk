/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using System.Collections.Generic;

namespace Aliyun.OSS.Transform
{
    internal class ListBucketInventoryConfigurationResultDeserializer
        : ResponseDeserializer<ListBucketInventoryConfigurationResult, ListInventoryConfigurationModel>
    {
        public ListBucketInventoryConfigurationResultDeserializer(IDeserializer<Stream, ListInventoryConfigurationModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override ListBucketInventoryConfigurationResult Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);

            var result = new ListBucketInventoryConfigurationResult
            {
                IsTruncated = model.IsTruncated.HasValue? model.IsTruncated.Value: false,
                NextContinuationToken = model.NextContinuationToken,
            };

            var configs = new List<InventoryConfiguration>();
            if (model.Configurations != null)
            {
                foreach (var e in model.Configurations)
                {
                    configs.Add(GetBucketInventoryConfigurationResultDeserializer.ToInventoryConfiguration(e));
                }
            }
            result.Configurations = configs;

            DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}

