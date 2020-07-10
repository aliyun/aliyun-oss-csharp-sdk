/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class SetBucketInventoryConfigurationRequestSerializer : RequestSerializer<SetBucketInventoryConfigurationRequest, InventoryConfigurationModel>
    {
        public SetBucketInventoryConfigurationRequestSerializer(ISerializer<InventoryConfigurationModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(SetBucketInventoryConfigurationRequest request)
        {
            var config = request.Configuration;
            var model = new InventoryConfigurationModel
            {
                Id = config.Id,
                IsEnabled = config.IsEnabled,
                IncludedObjectVersions = config.IncludedObjectVersions,
            };

            if (config.Schedule != null)
            {
                model.Schedule = new InventoryConfigurationModel.ScheduleModel
                {
                    Frequency = config.Schedule.Frequency
                };
            }

            if (config.Filter != null)
            {
                model.Filter = new InventoryConfigurationModel.FilterModel { Prefix = config.Filter.Prefix };
            }

            //Destination
            var ossDest = new InventoryConfigurationModel.OSSBucketDestinationModel
            {
                Format = config.Destination.OSSBucketDestination.Format,
                AccountId = config.Destination.OSSBucketDestination.AccountId,
                RoleArn = config.Destination.OSSBucketDestination.RoleArn,
                Bucket = "acs:oss:::" + config.Destination.OSSBucketDestination.Bucket,
                Prefix = config.Destination.OSSBucketDestination.Prefix,
            };

            if (config.Destination.OSSBucketDestination.Encryption != null)
            {
                ossDest.Encryption = new InventoryConfigurationModel.EncryptionModel();

                if (config.Destination.OSSBucketDestination.Encryption.SSEKMS != null)
                {
                    ossDest.Encryption.SSEKMS = new InventoryConfigurationModel.SSEKMSModel()
                    {
                        KeyId = config.Destination.OSSBucketDestination.Encryption.SSEKMS.KeyId
                    };
                }

                if (config.Destination.OSSBucketDestination.Encryption.SSEOSS != null)
                {
                    ossDest.Encryption.SSEOSS = new InventoryConfigurationModel.SSEOSSModel();
                }
            }

            model.Destination = new InventoryConfigurationModel.DestinationModel
            {
                OSSBucketDestination = ossDest
            };

            //Fields
            if (config.OptionalFields.Count > 0)
            {
                model.OptionalFields = new InventoryConfigurationModel.OptionalFieldsModel();
                model.OptionalFields.Fields = new InventoryOptionalField[config.OptionalFields.Count];
                for (var i = 0; i < config.OptionalFields.Count; i++)
                {
                    model.OptionalFields.Fields[i] = config.OptionalFields[i];
                }
            }

            return ContentSerializer.Serialize(model);
        }
    }
}
