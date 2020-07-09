/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("ListLiveChannelResult")]
    public class ListLiveChannelResultModel
    {
        [XmlElement("Prefix")]
        public string Prefix { get; set; }

        [XmlElement("Marker")]
        public string Marker { get; set; }

        [XmlElement("MaxKeys")]
        public int? MaxKeys { get; set; }

        [XmlElement("IsTruncated")]
        public bool? IsTruncated { get; set; }

        [XmlElement("NextMarker")]
        public string NextMarker { get; set; }

        [XmlElement("LiveChannel")]
        public LiveChannelModel[] LiveChannels { get; set; }

        [XmlRoot("LiveChannel")]
        public class LiveChannelModel
        {
            [XmlElement("Name")]
            public string Name { get; set; }

            [XmlElement("Description")]
            public string Description { get; set; }

            [XmlElement("Status")]
            public string Status { get; set; }

            [XmlElement("LastModified")]
            public DateTime LastModified { get; set; }

            [XmlElement("PublishUrls")]
            public PublishUrlsModel PublishUrls { get; set; }

            [XmlElement("PlayUrls")]
            public PlayUrlsModel PlayUrls { get; set; }
        }

        [XmlRoot("PublishUrls")]
        public class PublishUrlsModel
        {
            [XmlElement("Url")]
            public string Url { get; set; }
        }

        [XmlRoot("PlayUrls")]
        public class PlayUrlsModel
        {
            [XmlElement("Url")]
            public string Url { get; set; }
        }
    }
}