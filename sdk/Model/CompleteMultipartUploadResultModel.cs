/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("CompleteMultipartUploadResult")]
    public class CompleteMultipartUploadResultModel
    {
        [XmlElement("Location")]
        public string Location { get; set; }
        
        [XmlElement("Bucket")]
        public string Bucket { get; set; }
        
        [XmlElement("Key")]
        public string Key { get; set; }
        
        [XmlElement("ETag")]
        public string ETag { get; set; }
    }
}
