/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("Tagging")]
    public class Tagging
    {
        [XmlElement("TagSet")]
        public TagSetModel TagSet { get; set; }

        [XmlRoot("TagSet")]
        public class TagSetModel
        {
            [XmlElement("Tag")]
            public Tag[] Tags { get; set; }

            [XmlRoot("Tag")]
            public class Tag
            {
                [XmlElement("Key")]
                public string Key { get; set; }

                [XmlElement("Value")]
                public string Value { get; set; }
            }
        }
    }
}
