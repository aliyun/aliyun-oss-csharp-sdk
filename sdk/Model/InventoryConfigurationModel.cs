/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("InventoryConfiguration")]
    public class InventoryConfigurationModel
    {
        [XmlElement("Id")]
        public string Id { get; set; }

        [XmlElement("IsEnabled")]
        public bool IsEnabled { get; set; }

        [XmlElement("Filter")]
        public FilterModel Filter { get; set; }

        [XmlElement("Destination")]
        public DestinationModel Destination { get; set; }

        [XmlElement("Schedule")]
        public ScheduleModel Schedule { get; set; }

        [XmlElement("IncludedObjectVersions")]
        public InventoryIncludedObjectVersions IncludedObjectVersions { get; set; }

        [XmlElement("OptionalFields")]
        public OptionalFieldsModel OptionalFields { get; set; }

        [XmlRoot("Filter")]
        public class FilterModel
        {
            [XmlElement("Prefix")]
            public string Prefix { get; set; }
        }

        [XmlRoot("Destination")]
        public class DestinationModel
        {
            [XmlElement("OSSBucketDestination")]
            public OSSBucketDestinationModel OSSBucketDestination { get; set; }
        }

        [XmlRoot("OSSBucketDestination")]
        public class OSSBucketDestinationModel
        {
            [XmlElement("Format")]
            public InventoryFormat Format { get; set; }

            [XmlElement("AccountId")]
            public string AccountId { get; set; }

            [XmlElement("RoleArn")]
            public string RoleArn { get; set; }

            [XmlElement("Bucket")]
            public string Bucket { get; set; }

            [XmlElement("Prefix")]
            public string Prefix { get; set; }

            [XmlElement("Encryption")]
            public EncryptionModel Encryption { get; set; }
        }

        [XmlRoot("Schedule")]
        public class ScheduleModel
        {
            [XmlElement("Frequency")]
            public InventoryFrequency Frequency { get; set; }
        }

        [XmlRoot("OptionalFields")]
        public class OptionalFieldsModel
        {
            [XmlElement("Field")]
            public InventoryOptionalField[] Fields { get; set; }
        }

        [XmlRoot("Encryption")]
        public class EncryptionModel
        {
            [XmlElement("SSE-KMS", IsNullable = true)]
            public SSEKMSModel SSEKMS { get; set; }
            public bool ShouldSerializeSSEKMS()
            {
                return SSEKMS != null;
            }

            [XmlElement("SSE-OSS", IsNullable = true)]
            public SSEOSSModel SSEOSS { get; set; }
            public bool ShouldSerializeSSEOSS()
            {
                return SSEOSS != null;
            }
        }

        [XmlRoot("SSE-KMS")]
        public class SSEKMSModel
        {
            [XmlElement("KeyId")]
            public string KeyId { get; set; }
        }

        [XmlRoot("SSE-OSS")]
        public class SSEOSSModel
        {
        }
    }
}
