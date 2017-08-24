/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("CopyObjectResult")]
    public class CopyObjectResultModel
    {
        [XmlElement("LastModified")]
        public DateTime LastModified { get; set; }
        
        [XmlElement("ETag")]
        public string ETag { get; set; }
    }
}
