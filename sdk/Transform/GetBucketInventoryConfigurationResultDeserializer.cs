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
        : ResponseDeserializer<GetBucketInventoryConfigurationResult, InventoryConfigurationModel>
    {
        public GetBucketInventoryConfigurationResultDeserializer(IDeserializer<Stream, InventoryConfigurationModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override GetBucketInventoryConfigurationResult Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);

            var result = new GetBucketInventoryConfigurationResult
            {
                Configuration = ToInventoryConfiguration(model)
            };

            DeserializeGeneric(xmlStream, result);
            return result;
        }

        internal static string ToInventoryBucketShortName(string name)
        {
            if (!string.IsNullOrEmpty(name) && name.StartsWith("acs:oss:::"))
            {
                return name.Substring(10);
            }
            return name;
        }

        internal static InventoryConfiguration ToInventoryConfiguration(InventoryConfigurationModel model)
        {
            var config = new InventoryConfiguration()
            {
                Id = model.Id,
                IsEnabled = model.IsEnabled,
                IncludedObjectVersions = model.IncludedObjectVersions,
            };

            if (model.Schedule != null)
            {
                config.Schedule = new InventorySchedule(model.Schedule.Frequency);
            }

            if (model.Filter != null)
            {
                config.Filter = new InventoryFilter(model.Filter.Prefix);
            }

            if (model.Destination != null && model.Destination.OSSBucketDestination != null)
            { 
                var ossDst = new InventoryOSSBucketDestination
                {
                    Format = model.Destination.OSSBucketDestination.Format,
                    AccountId = model.Destination.OSSBucketDestination.AccountId,
                    RoleArn = model.Destination.OSSBucketDestination.RoleArn,
                    Bucket = ToInventoryBucketShortName(model.Destination.OSSBucketDestination.Bucket),
                    Prefix = model.Destination.OSSBucketDestination.Prefix
                };

                if (model.Destination.OSSBucketDestination.Encryption != null)
                {
                    if (model.Destination.OSSBucketDestination.Encryption.SSEKMS != null)
                    {
                        ossDst.Encryption = new InventoryEncryption(
                            new InventorySSEKMS(model.Destination.OSSBucketDestination.Encryption.SSEKMS.KeyId));
                    }
                    else if (model.Destination.OSSBucketDestination.Encryption.SSEOSS != null)
                    {
                        ossDst.Encryption = new InventoryEncryption(new InventorySSEOSS());
                    }
                    else
                    {
                        ossDst.Encryption = new InventoryEncryption();
                    }
                }

                config.Destination = new InventoryDestination()
                {
                    OSSBucketDestination = ossDst
                };
            }

            var fields = new List<InventoryOptionalField>();
            if (model.OptionalFields != null && model.OptionalFields.Fields != null)
            {
                foreach (var e in model.OptionalFields.Fields)
                {
                    fields.Add(e);
                }
            }
            config.OptionalFields = fields;

            return config;
        }
    }
}

