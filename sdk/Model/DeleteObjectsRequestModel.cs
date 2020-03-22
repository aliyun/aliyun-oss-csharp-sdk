/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("Delete")]
    public class DeleteObjectsRequestModel
    {
        [XmlElement("Quiet")]
        public bool Quiet { get; set; }

        [XmlElement("Object")]
        public ObjectToDel[] Keys { get; set; }

        [XmlRoot("Object")]
        public class ObjectToDel
        {
            [XmlElement("Key")]
            public string Key { get; set; }
        }
    }

    [XmlRoot("Delete")]
    public class DeleteObjectVersionsRequestModel
    {
        [XmlElement("Quiet")]
        public bool Quiet { get; set; }

        [XmlElement("Object")]
        public ObjectToDel[] ObjectToDels { get; set; }

        [XmlRoot("Object")]
        public class ObjectToDel
        {
            [XmlElement("Key")]
            public string Key { get; set; }

            [XmlElement("VersionId", IsNullable = true)]
            public string VersionId { get; set; }

            public bool ShouldSerializeVersionId()
            {
                return !string.IsNullOrEmpty(VersionId);
            }
        }
    }
}
