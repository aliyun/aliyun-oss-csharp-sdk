/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("InitiateMultipartUploadResult")]
    public class InitiateMultipartResult
    {
        [XmlElement("Bucket")]
        public string Bucket { get; set; }
        
        [XmlElement("Key")]
        public string Key { get; set; }
        
        [XmlElement("UploadId")]
        public string UploadId { get; set; }

        [XmlElement("EncodingType")]
        public string EncodingType { get; set; }
    }
}
