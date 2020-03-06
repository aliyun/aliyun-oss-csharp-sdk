/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("RequestPaymentConfiguration")]
    public class RequestPaymentConfiguration
    {
        [XmlElement("Payer")]
        public RequestPayer Payer { get; set; }
    }
}
