/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("ServerSideEncryptionRule")]
    public class ServerSideEncryptionRule
    {
        public ApplyServerSideEncryptionByDefaultModel ApplyServerSideEncryptionByDefault { get; set; }

        [XmlRoot("ApplyServerSideEncryptionByDefault")]
        public class ApplyServerSideEncryptionByDefaultModel
        {
            [XmlElement("SSEAlgorithm")]
            public string SSEAlgorithm { get; set; }

            [XmlElement("KMSMasterKeyID")]
            public string KMSMasterKeyID { get; set; }
        }
    }
}
