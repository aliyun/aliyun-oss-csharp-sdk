/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class SetBucketInventoryConfigurationRequestSerializer : RequestSerializer<SetBucketInventoryConfigurationRequest, InventoryConfiguration>
    {
        public SetBucketInventoryConfigurationRequestSerializer(ISerializer<InventoryConfiguration, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(SetBucketInventoryConfigurationRequest request)
        {
            var model = new InventoryConfiguration
            {
                Id = request.Id,
                IsEnabled = request.IsEnabled,
                filter = new InventoryConfiguration.Filter { Prefix = request.Prefix },
                destination = new InventoryConfiguration.Destination { OSSBucketDestination = new InventoryConfiguration.OSSBucketDestination()},
                schedule = new InventoryConfiguration.Schedule { },
                IncludedObjectVersions = request.IncludedObjectVersions,
                OptionalFields = new InventoryConfiguration.OptionalField()
            };
            model.destination.OSSBucketDestination.Format = request.Destination.Format;
            model.destination.OSSBucketDestination.AccountId = request.Destination.AccountId;
            model.destination.OSSBucketDestination.RoleArn = request.Destination.RoleArn;
            model.destination.OSSBucketDestination.Bucket = "acs:oss:::"+ request.Destination.Bucket;
            model.destination.OSSBucketDestination.Prefix = request.Destination.Prefix;
            model.destination.OSSBucketDestination.encryption = new InventoryConfiguration.OSSBucketDestination.Encryption();

            if (request.Destination.Encryption != null)
            {
                model.destination.OSSBucketDestination.encryption.SSEKMS = new InventoryConfiguration.OSSBucketDestination.SSEKMS();
                model.destination.OSSBucketDestination.encryption.SSEKMS.KeyId = request.Destination.Encryption;
            }

            if (request.Destination.IsEncryptionOSS)
            {
                model.destination.OSSBucketDestination.encryption.EncryptionOSS = "true";
            }

            model.schedule = new InventoryConfiguration.Schedule();
            model.schedule.Frequency = request.Schedule;

            model.OptionalFields.Field = new InventoryOptionalField[request.OptionalFields.Count];
            for (var i = 0; i < request.OptionalFields.Count; i++)
            {
                model.OptionalFields.Field[i] = request.OptionalFields[i];
            }

            return ContentSerializer.Serialize(model);
        }
    }
}
