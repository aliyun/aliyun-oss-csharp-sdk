/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("LifecycleConfiguration")]
    public class LifecycleConfiguration
    {
        [XmlElement("Rule")]
        public LifecycleRule[] LifecycleRules { get; set; }
    }

    [XmlRoot("Rule")]
    public class LifecycleRule
    {
        [XmlElement("ID")]
        public string ID { get; set; }

        [XmlElement("Prefix")]
        public String Prefix { get; set; }

        [XmlElement("Status")]
        public string Status { get; set; }

        [XmlElement("Expiration")]
        public Expiration Expiration { get; set; }

        [XmlElement("AbortMultipartUpload")]
        public Expiration AbortMultipartUpload { get; set; }

        [XmlElement("Transition")]
        public LifecycleRuleTransition[] Transition { get; set; }
    }

    public class Expiration
    {
        [XmlElement("Days", IsNullable = true)]
        public int? Days { get; set; }

        public bool ShouldSerializeDays()
        {
            return Days.HasValue;
        }

        public bool IsSetDays()
        {
            return Days.HasValue;
        }

        [XmlElement("CreatedBeforeDate", IsNullable = true)]
        public string CreatedBeforeDate
        {
            get;
            set;
        }

        public bool ShouldSerializeCreatedBeforeDate()
        {
            return CreatedBeforeDate != null;
        }

        [XmlElement("Date", IsNullable = true)] 
        public string Date { get; set; }

        public bool ShouldSerializeDate()
        {
            return Date != null;
        }

        public bool IsSetDate()
        {
            return Date != null;
        }
    }

    public class LifecycleRuleTransition : Expiration
    {
        [XmlElement("StorageClass")]
        public StorageClass StorageClass { get; set; }
    }
}
