/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("BucketLoggingStatus")]
    public class SetBucketLoggingRequestModel
    {
        [XmlElement("LoggingEnabled")]
        public SetBucketLoggingEnabled LoggingEnabled { get; set; }

        [XmlRoot("LoggingEnabled")]
        public class SetBucketLoggingEnabled 
        {
            [XmlElement("TargetBucket")]
            public string TargetBucket { get; set; }

            [XmlElement("TargetPrefix")]
            public string TargetPrefix { get; set; }
        }
    }
}
