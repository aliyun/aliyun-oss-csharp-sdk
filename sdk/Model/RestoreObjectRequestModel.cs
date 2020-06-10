/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("RestoreRequest")]
    public class RestoreRequestModel
    {
        [XmlElement("Days")]
        public int Days { get; set; }

        [XmlElement("JobParameters")]
        public JobParameters JobParameter { get; set; }

        public class JobParameters
        {
            [XmlElement("Tier")]
            public TierType Tier { get; set; }
        }
    }
}
