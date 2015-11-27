/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
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
}
