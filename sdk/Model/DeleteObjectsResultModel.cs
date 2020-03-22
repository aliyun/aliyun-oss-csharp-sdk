/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("DeleteResult")]
    public class DeleteObjectVersionsResultModel
    {
        [XmlElement("EncodingType")]
        public string EncodingType { get; set; }

        [XmlElement("Deleted")]
        public DeletedObject[] DeletedObjects { get; set; }

        [XmlRoot("Deleted")]
        public class DeletedObject
        {
            [XmlElement("Key")]
            public string Key { get; set; }

            [XmlElement("VersionId")]
            public string VersionId { get; set; }

            [XmlElement("DeleteMarker")]
            public bool DeleteMarker { get; set; }

            [XmlElement("DeleteMarkerVersionId")]
            public string DeleteMarkerVersionId { get; set; }
        }
    }
}
