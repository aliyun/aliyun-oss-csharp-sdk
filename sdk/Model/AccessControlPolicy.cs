/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("AccessControlPolicy")]
    public class AccessControlPolicy
    {
        [XmlElement("Owner")]
        public Owner Owner { get; set; }

        [XmlArray("AccessControlList")]
        [XmlArrayItem("Grant")]
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<string> Grants { get; set; }
    }
}
