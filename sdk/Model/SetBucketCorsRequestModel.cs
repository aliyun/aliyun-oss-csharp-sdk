/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
   [XmlRoot("CORSConfiguration")]
    public class SetBucketCorsRequestModel
   {
       [XmlElement("CORSRule")]
       public CORSRuleModel[] CORSRuleModels { get; set; }

       [XmlRoot("CORSRule")]
       public class CORSRuleModel 
       {
           [XmlElement("AllowedOrigin")]
           public String[] AllowedOrigins { get; set; }

           [XmlElement("AllowedMethod")]
           public String[] AllowedMethods { get; set; }

           [XmlElement("AllowedHeader")]
           public String[] AllowedHeaders { get; set; }

           [XmlElement("ExposeHeader")]
           public String[] ExposeHeaders { get; set; }

           [XmlElement("MaxAgeSeconds")]
           public Int32 MaxAgeSeconds { get; set; }
       }

    }
}
