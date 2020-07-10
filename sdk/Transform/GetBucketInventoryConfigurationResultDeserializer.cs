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
    internal class GetBucketInventoryConfigurationResultDeserializer
        : ResponseDeserializer<BucketInventoryConfigurationResult, InventoryConfiguration>
    {
        public GetBucketInventoryConfigurationResultDeserializer(IDeserializer<Stream, InventoryConfiguration> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override BucketInventoryConfigurationResult Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);

            var bucketInventoryConfigurationResult = new BucketInventoryConfigurationResult
            {
                InventoryConfiguration =  new InventoryConfiguration()
            };

            bucketInventoryConfigurationResult.InventoryConfiguration.Id = model.Id;
            bucketInventoryConfigurationResult.InventoryConfiguration.IsEnabled = model.IsEnabled;
            bucketInventoryConfigurationResult.InventoryConfiguration.filter = new InventoryConfiguration.Filter();
            bucketInventoryConfigurationResult.InventoryConfiguration.filter.Prefix = model.filter.Prefix;
            bucketInventoryConfigurationResult.InventoryConfiguration.destination = new InventoryConfiguration.Destination();
            bucketInventoryConfigurationResult.InventoryConfiguration.destination.OSSBucketDestination = new InventoryConfiguration.OSSBucketDestination();
            bucketInventoryConfigurationResult.InventoryConfiguration.destination.OSSBucketDestination.Format = model.destination.OSSBucketDestination.Format;
            bucketInventoryConfigurationResult.InventoryConfiguration.destination.OSSBucketDestination.AccountId = model.destination.OSSBucketDestination.AccountId;
            bucketInventoryConfigurationResult.InventoryConfiguration.destination.OSSBucketDestination.RoleArn = model.destination.OSSBucketDestination.RoleArn;
            bucketInventoryConfigurationResult.InventoryConfiguration.destination.OSSBucketDestination.Bucket = model.destination.OSSBucketDestination.Bucket;
            bucketInventoryConfigurationResult.InventoryConfiguration.destination.OSSBucketDestination.Prefix = model.destination.OSSBucketDestination.Prefix;
            bucketInventoryConfigurationResult.InventoryConfiguration.schedule = new InventoryConfiguration.Schedule();
            bucketInventoryConfigurationResult.InventoryConfiguration.schedule.Frequency = model.schedule.Frequency;
            bucketInventoryConfigurationResult.InventoryConfiguration.IncludedObjectVersions = model.IncludedObjectVersions;
            bucketInventoryConfigurationResult.InventoryConfiguration.OptionalFields = new InventoryConfiguration.OptionalField();
            bucketInventoryConfigurationResult.InventoryConfiguration.OptionalFields.Field = new InventoryOptionalField[model.OptionalFields.Field.Length];
            bucketInventoryConfigurationResult.InventoryConfiguration.destination.OSSBucketDestination.encryption = new InventoryConfiguration.OSSBucketDestination.Encryption();
            if (model.destination.OSSBucketDestination.encryption.SSEKMS != null)
            {               
                bucketInventoryConfigurationResult.InventoryConfiguration.destination.OSSBucketDestination.encryption.SSEKMS = new InventoryConfiguration.OSSBucketDestination.SSEKMS();
                bucketInventoryConfigurationResult.InventoryConfiguration.destination.OSSBucketDestination.encryption.SSEKMS.KeyId = model.destination.OSSBucketDestination.encryption.SSEKMS.KeyId;
            }
            else
            {
                bucketInventoryConfigurationResult.InventoryConfiguration.destination.OSSBucketDestination.encryption.EncryptionOSS = "true";
            }
            for (var i = 0; i < model.OptionalFields.Field.Length; i++)
            {
                bucketInventoryConfigurationResult.InventoryConfiguration.OptionalFields.Field[i] = model.OptionalFields.Field[i];
            }
            DeserializeGeneric(xmlStream, bucketInventoryConfigurationResult);
            return bucketInventoryConfigurationResult;
        }
    }
}

