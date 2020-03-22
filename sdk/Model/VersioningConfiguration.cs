/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("VersioningConfiguration")]
    public class VersioningConfiguration
    {
        [XmlElement("Status")]
        public VersioningStatus Status { get; set; }
    }
}
