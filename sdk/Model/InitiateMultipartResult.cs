/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
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
