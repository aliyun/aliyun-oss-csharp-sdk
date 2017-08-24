/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("WebsiteConfiguration")]
    public class SetBucketWebsiteRequestModel
    {
        [XmlElement("IndexDocument")]
        public IndexDocumentModel IndexDocument { get; set; }

        [XmlElement("ErrorDocument")]
        public ErrorDocumentModel ErrorDocument { get; set; }

        [XmlRoot("IndexDocument")]
        public class IndexDocumentModel
        {
            [XmlElement("Suffix")]
            public string Suffix { get; set; }
        }

        [XmlRoot("ErrorDocument")]
        public class ErrorDocumentModel
        {
            [XmlElement("Key")]
            public string Key { get; set; }
        }

    }
}
