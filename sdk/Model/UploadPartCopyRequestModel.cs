/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("CopyPartResult")]
    public class UploadPartCopyRequestModel
    {
        [XmlElement("LastModified")]
        public DateTime LastModified { get; set; }
        
        [XmlElement("ETag")]
        public string ETag { get; set; }
    }
}
