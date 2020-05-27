/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("InventoryConfiguration")]
    public class InventoryConfiguration
    {
        [XmlElement("Id")]
        public string Id { get; set; }

        [XmlElement("IsEnabled")]
        public bool IsEnabled { get; set; }

        [XmlElement("Filter")]
        public Filter filter { get; set; }

        public class Filter 
        {
            [XmlElement("Prefix")]
            public string Prefix { get; set; }
        }

        [XmlElement("Destination")]
        public Destination destination { get; set; }

        public class Destination
        {
            public OSSBucketDestination OSSBucketDestination { get; set; }
        }

        [XmlRoot("OSSBucketDestination")]
        public class OSSBucketDestination
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
            public Encryption encryption { get; set; }

            public class Encryption
            {
                [XmlElement("SSE-KMS", IsNullable = true)]
                public SSEKMS SSEKMS { get; set; }
                public bool ShouldSerializeSSEKMS()
                {
                    return SSEKMS != null;
                }

                [XmlElement("SSE-OSS", IsNullable = true)]
                public string EncryptionOSS { get; set; }
                public bool ShouldSerializeEncryptionOSS()
                {
                    return !string.IsNullOrEmpty(EncryptionOSS);
                }
            }

            public class SSEKMS
            {
                [XmlElement("KeyId")]
                public string KeyId { get; set; }
            }
        }

        [XmlElement("Schedule")]
        public Schedule schedule { get; set; }

        public class Schedule
        {
            [XmlElement("Frequency")]
            public InventoryFrequency Frequency { get; set; }
        }

        [XmlElement("IncludedObjectVersions")]
        public InventoryIncludedObjectVersions IncludedObjectVersions { get; set; }

        [XmlElement("OptionalFields")]
        public OptionalField OptionalFields { get; set; }

        public class OptionalField
        {
            [XmlElement("Field")]
            public InventoryOptionalField[] Field { get; set; }
        }
    }
}
