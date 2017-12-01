/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aliyun.OSS
{
    /// <summary>
    /// The bucket information class
    /// </summary>
    [XmlRoot("BucketInfo")]
    public class BucketInfo
    {
        [XmlElement("Bucket")]
        public Bucket Bucket { get; set; }
    }
}
