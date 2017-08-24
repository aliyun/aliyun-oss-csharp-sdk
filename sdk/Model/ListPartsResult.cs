/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("ListPartsResult")]
    public class ListPartsResult
    {
        [XmlElement("Bucket")]
        public string Bucket { get; set; }

        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("UploadId")]
        public string UploadId { get; set; }

        [XmlElement("PartNumberMarker")]
        public int PartNumberMarker { get; set; }

        [XmlElement("NextPartNumberMarker")]
        public String NextPartNumberMarker { get; set; }

        [XmlElement("MaxParts")]
        public int MaxParts { get; set; }

        [XmlElement("IsTruncated")]
        public bool IsTruncated { get; set; }

        [XmlElement("Part")]
        public PartResult[] PartResults { get; set; }

        [XmlElement("EncodingType")]
        public string EncodingType { get; set; }


        [XmlRoot("Part")]
        public class PartResult
        {
            [XmlElement("PartNumber")]
            public int PartNumber { get; set; }

            [XmlElement("LastModified")]
            public DateTime LastModified { get; set; }

            [XmlElement("ETag")]
            public string ETag { get; set; }

            [XmlElement("Size")]
            public long Size { get; set; }
        }
    }
}
