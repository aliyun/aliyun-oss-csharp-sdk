/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("LiveChannelConfiguration")]
    public class LiveChannelConfiguration
    {
        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("Status")]
        public string Status { get; set; }

        [XmlElement("Target")]
        public TargetModel Target { get; set; }

        [XmlElement("Snapshot")]
        public SnapshotModel Snapshot { get; set; }

        [XmlRoot("Target")]
        public class TargetModel
        {
            [XmlElement("Type")]
            public string Type { get; set; }

            [XmlElement("FragDuration")]
            public long FragDuration { get; set; }

            [XmlElement("FragCount")]
            public long FragCount { get; set; }

            [XmlElement("PlaylistName")]
            public string PlaylistName { get; set; }
        }

        [XmlRoot("Snapshot")]
        public class SnapshotModel
        {
            [XmlElement("RoleName")]
            public string RoleName { get; set; }

            [XmlElement("DestBucket")]
            public string DestBucket { get; set; }

            [XmlElement("NotifyTopic")]
            public string NotifyTopic { get; set; }

            [XmlElement("Interval")]
            public long Interval { get; set; }
        }
    }
}
