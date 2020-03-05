/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("CreateBucketConfiguration")]
    public class CreateBucketRequestModel
    {
        [XmlElement("StorageClass")]
        public StorageClass StorageClass { get; set; }

        [XmlElement("DataRedundancyType", IsNullable = true)]
        public DataRedundancyType? DataRedundancyType { get; set; }

        public bool ShouldSerializeDataRedundancyType()
        {
            return DataRedundancyType != null;
        }
    }
}
