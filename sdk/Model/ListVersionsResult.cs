/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("ListVersionsResult")]
    public class ListVersionsResult
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Prefix")]
        public string Prefix { get; set; }

        [XmlElement("EncodingType")]
        public string EncodingType { get; set; }

        [XmlElement("Delimiter")]
        public string Delimiter { get; set; }

        [XmlElement("KeyMarker")]
        public string KeyMarker { get; set; }

        [XmlElement("VersionIdMarker")]
        public string VersionIdMarker { get; set; }

        [XmlElement("MaxKeys")]
        public int? MaxKeys { get; set; }

        [XmlElement("IsTruncated")]
        public bool? IsTruncated { get; set; }

        [XmlElement("NextKeyMarker")]
        public string NextKeyMarker { get; set; }

        [XmlElement("NextVersionIdMarker")]
        public string NextVersionIdMarker { get; set; }

        [XmlElement("Version")]
        public ObjectVersion[] ObjectVersions { get; set; }

        [XmlElement("DeleteMarker")]
        public ObjectDeleteMarker[] ObjectDeleteMarkers { get; set; }

        [XmlElement("CommonPrefixes")]
        public CommonPrefixesModel[] CommonPrefixes { get; set; }

        [XmlRoot("Version")]
        public class ObjectVersion
        {
            [XmlElement("Key")]
            public string Key { get; set; }

            [XmlElement("VersionId")]
            public string VersionId { get; set; }

            [XmlElement("IsLatest")]
            public bool IsLatest { get; set; }

            [XmlElement("LastModified")]
            public DateTime LastModified { get; set; }

            [XmlElement("ETag")]
            public string ETag { get; set; }

            [XmlElement("Type")]
            public string Type { get; set; }

            [XmlElement("Size")]
            public long Size { get; set; }

            [XmlElement("StorageClass")]
            public string StorageClass { get; set; }

            [XmlElement("Owner")]
            public Owner Owner { get; set; }
        }

        [XmlRoot("DeleteMarker")]
        public class ObjectDeleteMarker
        {
            [XmlElement("Key")]
            public string Key { get; set; }

            [XmlElement("VersionId")]
            public string VersionId { get; set; }

            [XmlElement("IsLatest")]
            public bool IsLatest { get; set; }

            [XmlElement("LastModified")]
            public DateTime LastModified { get; set; }

            [XmlElement("Owner")]
            public Owner Owner { get; set; }
        }

        [XmlRoot("CommonPrefixes")]
        public class CommonPrefixesModel
        {
            [XmlElement("Prefix")]
            public string Prefix { get; set; }
        }
    }
}
