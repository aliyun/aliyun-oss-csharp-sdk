/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("ListMultipartUploadsResult")]
    public class ListMultipartUploadsResult
    {
        [XmlElement("Bucket")]
        public string Bucket { get; set; }
        
        [XmlElement("KeyMarker")]
        public string KeyMarker { get; set; }
        
        [XmlElement("UploadIdMarker")]
        public string UploadIdMarker { get; set; }
        
        [XmlElement("NextKeyMarker")]
        public string NextKeyMarker { get; set; }
        
        [XmlElement("NextUploadIdMarker")]
        public string NextUploadIdMarker { get; set; }
        
        [XmlElement("Delimiter")]
        public string Delimiter { get; set; }
        
        [XmlElement("Prefix")]
        public string Prefix { get; set; }

        [XmlElement("EncodingType")]
        public string EncodingType { get; set; } 

        [XmlElement("MaxUploads")]
        public int MaxUploads { get; set; }
        
        [XmlElement("IsTruncated")]
        public bool IsTruncated { get; set; }
        
        [XmlElement("Upload")]
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public Upload[] Uploads { get; set; }
        
        [XmlElement("CommonPrefixes")]
        public CommonPrefixs CommonPrefix { get; set; }

        [XmlRoot("Upload")]
        public class Upload
        {
            [XmlElement("Key")]
            public string Key { get; set; }
            
            [XmlElement("UploadId")]
            public string UploadId { get; set; }
            
            [XmlElement("StorageClass")]
            public string StorageClass { get; set; }
            
            [XmlElement("Initiated")]
            public DateTime Initiated { get; set; }
        }
        
        [XmlRoot("CommonPrefixes")]
        public class CommonPrefixs
        {
            [XmlElement("Prefix")]
            [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
            public string[] Prefixs { get; set; }
        }
    }
}
