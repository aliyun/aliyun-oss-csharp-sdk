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

        [XmlElement("Tag")]
        public LifecycleRuleTag[] Tags { get; set; }

        [XmlElement("NoncurrentVersionExpiration")]
        public LifecycleRuleNoncurrentVersionExpiration NoncurrentVersionExpiration { get; set; }

        [XmlElement("NoncurrentVersionTransition")]
        public LifecycleRuleNoncurrentVersionTransition[] NoncurrentVersionTransition { get; set; }

        [XmlElement("Filter")]
        public LifecycleRuleFilter Filter { get; set; }
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

        [XmlElement("ExpiredObjectDeleteMarker", IsNullable = true)]
        public bool? ExpiredObjectDeleteMarker { get; set; }

        public bool ShouldSerializeExpiredObjectDeleteMarker()
        {
            return ExpiredObjectDeleteMarker != null;
        }

        public bool IsSetExpiredObjectDeleteMarker()
        {
            return ExpiredObjectDeleteMarker != null;
        }
    }

    public class LifecycleRuleTransition : Expiration
    {
        [XmlElement("StorageClass")]
        public StorageClass StorageClass { get; set; }

        [XmlElement("IsAccessTime", IsNullable = true)]
        public bool? IsAccessTime { get; set; }

        public bool ShouldSerializeIsAccessTime()
        {
            return IsAccessTime != null;
        }
        public bool IsSetIsAccessTime()
        {
            return IsAccessTime != null;
        }

        [XmlElement("ReturnToStdWhenVisit", IsNullable = true)]
        public bool? ReturnToStdWhenVisit { get; set; }
        public bool ShouldSerializeReturnToStdWhenVisit()
        {
            return ReturnToStdWhenVisit != null;
        }
        public bool IsSetReturnToStdWhenVisit()
        {
            return ReturnToStdWhenVisit != null;
        }

        [XmlElement("AllowSmallFile", IsNullable = true)]
        public bool? AllowSmallFile { get; set; }
        public bool ShouldSerializeAllowSmallFile()
        {
            return AllowSmallFile != null;
        }
        public bool IsSetAllowSmallFile()
        {
            return AllowSmallFile != null;
        }

    }

    public class LifecycleRuleTag
    {
        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }
    }

    public class LifecycleRuleNoncurrentVersionExpiration
    {
        [XmlElement("NoncurrentDays")]
        public int NoncurrentDays { get; set; }
    }

    public class LifecycleRuleNoncurrentVersionTransition
    {
        [XmlElement("NoncurrentDays")]
        public int NoncurrentDays { get; set; }

        [XmlElement("StorageClass")]
        public StorageClass StorageClass { get; set; }

        [XmlElement("IsAccessTime", IsNullable = true)]
        public bool? IsAccessTime { get; set; }

        public bool ShouldSerializeIsAccessTime()
        {
            return IsAccessTime != null;
        }
        public bool IsSetIsAccessTime()
        {
            return IsAccessTime != null;
        }

        [XmlElement("ReturnToStdWhenVisit", IsNullable = true)]
        public bool? ReturnToStdWhenVisit { get; set; }

        public bool ShouldSerializeReturnToStdWhenVisit()
        {
            return ReturnToStdWhenVisit != null;
        }
        public bool IsSetReturnToStdWhenVisit()
        {
            return ReturnToStdWhenVisit != null;
        }

        [XmlElement("AllowSmallFile", IsNullable = true)]
        public bool? AllowSmallFile { get; set; }
        public bool ShouldSerializeAllowSmallFile()
        {
            return AllowSmallFile != null;
        }
        public bool IsSetAllowSmallFile()
        {
            return AllowSmallFile != null;
        }
    }

    public class LifecycleRuleFilter
    {
        [XmlElement("Not")]
        public LifecycleNot Not { get; set; }

        [XmlElement("ObjectSizeGreaterThan", IsNullable = true)]
        public long? ObjectSizeGreaterThan { get; set; }

        public bool ShouldSerializeObjectSizeGreaterThan()
        {
            return ObjectSizeGreaterThan != null;
        }
        public bool IsSetObjectSizeGreaterThan()
        {
            return ObjectSizeGreaterThan != null;
        }

        [XmlElement("ObjectSizeLessThan", IsNullable = true)]
        public long? ObjectSizeLessThan { get; set; }

        public bool ShouldSerializeObjectSizeLessThan()
        {
            return ObjectSizeLessThan != null;
        }
        public bool IsSetObjectSizeLessThan()
        {
            return ObjectSizeLessThan != null;
        }
    }

    public class LifecycleNot
    {
        [XmlElement("Prefix")]
        public string Prefix { get; set; }

        [XmlElement("Tag")]
        public LifecycleRuleTag Tag { get; set; }
    }

}
