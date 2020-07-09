/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("CreateLiveChannelResult")]
    public class CreateLiveChannelResultModel
    {
        public PublishUrlsModel PublishUrls { get; set; }

        public PlayUrlsModel PlayUrls { get; set; }

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
