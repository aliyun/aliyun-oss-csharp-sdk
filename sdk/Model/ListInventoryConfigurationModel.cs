/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("ListInventoryConfigurationsResult")]
    public class ListInventoryConfigurationModel
    {
        [XmlElement("InventoryConfiguration")]
        public InventoryConfigurationModel[] Configurations { get; set; }

        [XmlElement("IsTruncated")]
        public bool? IsTruncated { get; set; }

        [XmlElement("NextContinuationToken")]
        public string NextContinuationToken { get; set; }
    }
}
