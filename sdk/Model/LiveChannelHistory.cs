/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("LiveChannelHistory")]
    public class LiveChannelHistory
    {
        [XmlElement("LiveRecord")]
        public LiveRecordModel[] LiveRecords { get; set; }

        [XmlRoot("LiveRecord")]
        public class LiveRecordModel
        {
            [XmlElement("StartTime")]
            public DateTime StartTime { get; set; }

            [XmlElement("EndTime")]
            public DateTime EndTime { get; set; }

            [XmlElement("RemoteAddr")]
            public string RemoteAddr { get; set; }
        }
    }
}
