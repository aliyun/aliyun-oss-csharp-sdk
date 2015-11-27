/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("Error")]
    public class ErrorResult
    {
        [XmlElement("Code")]
        public string Code { get; set; }

        [XmlElement("Message")]
        public string Message { get; set; }

        [XmlElement("RequestId")]
        public string RequestId { get; set; }

        [XmlElement("HostId")]
        public string HostId { get; set; }
    }
}
