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
                IsTruncated = model.IsTruncated,
                NextContinuationToken = model.NextContinuationToken,
                BucketInventory = new InventoryConfiguration[model.inventoryConfiguration.Length]
            };

            for (var i = 0; i < model.inventoryConfiguration.Length; i++)
            {
                var inventoryConfigurationResult = new InventoryConfiguration
                {
                    Id = model.inventoryConfiguration[i].Id,
                    IsEnabled = model.inventoryConfiguration[i].IsEnabled,
                    filter = new InventoryConfiguration.Filter(),
                    destination = new InventoryConfiguration.Destination(),
                    schedule = new InventoryConfiguration.Schedule(),
                    IncludedObjectVersions = model.inventoryConfiguration[i].IncludedObjectVersions,
                    OptionalFields = new InventoryConfiguration.OptionalField()
                };                

                inventoryConfigurationResult.filter.Prefix = model.inventoryConfiguration[i].filter.Prefix;
                inventoryConfigurationResult.destination.OSSBucketDestination = new InventoryConfiguration.OSSBucketDestination();

                inventoryConfigurationResult.destination.OSSBucketDestination.Format = model.inventoryConfiguration[i].destination.OSSBucketDestination.Format;
                inventoryConfigurationResult.destination.OSSBucketDestination.AccountId = model.inventoryConfiguration[i].destination.OSSBucketDestination.AccountId;
                inventoryConfigurationResult.destination.OSSBucketDestination.RoleArn = model.inventoryConfiguration[i].destination.OSSBucketDestination.RoleArn;
                inventoryConfigurationResult.destination.OSSBucketDestination.Bucket = model.inventoryConfiguration[i].destination.OSSBucketDestination.Bucket;
                inventoryConfigurationResult.destination.OSSBucketDestination.Prefix = model.inventoryConfiguration[i].destination.OSSBucketDestination.Prefix;

                inventoryConfigurationResult.schedule.Frequency = model.inventoryConfiguration[i].schedule.Frequency;
                inventoryConfigurationResult.OptionalFields.Field = new InventoryOptionalField[model.inventoryConfiguration[i].OptionalFields.Field.Length];
                inventoryConfigurationResult.destination.OSSBucketDestination.encryption = new InventoryConfiguration.OSSBucketDestination.Encryption();

                if (model.inventoryConfiguration[i].destination.OSSBucketDestination.encryption.SSEKMS != null)
                {
                    inventoryConfigurationResult.destination.OSSBucketDestination.encryption.SSEKMS = new InventoryConfiguration.OSSBucketDestination.SSEKMS();
                    inventoryConfigurationResult.destination.OSSBucketDestination.encryption.SSEKMS.KeyId = model.inventoryConfiguration[i].destination.OSSBucketDestination.encryption.SSEKMS.KeyId;
                }
                else
                {
                    inventoryConfigurationResult.destination.OSSBucketDestination.encryption.EncryptionOSS = "true";
                }

                for (var j = 0; j < model.inventoryConfiguration[i].OptionalFields.Field.Length; j++)
                {
                    inventoryConfigurationResult.OptionalFields.Field[j] = model.inventoryConfiguration[i].OptionalFields.Field[j];
                }

                result.BucketInventory[i] = inventoryConfigurationResult;
            }

            DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}

