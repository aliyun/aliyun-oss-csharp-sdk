/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("LiveChannelStat")]
    public class LiveChannelStat
    {
        [XmlElement("Status")]
        public string Status { get; set; }

        [XmlElement("ConnectedTime")]
        public DateTime ConnectedTime { get; set; }

        [XmlElement("RemoteAddr")]
        public string RemoteAddr { get; set; }

        [XmlElement("Video")]
        public VideoModel Video { get; set; }

        [XmlElement("Audio")]
        public AudioModel Audio { get; set; }

        [XmlRoot("Video")]
        public class VideoModel
        {
            [XmlElement("Width")]
            public int Width { get; set; }

            [XmlElement("Height")]
            public int Height { get; set; }

            [XmlElement("FrameRate")]
            public int FrameRate { get; set; }

            [XmlElement("Bandwidth")]
            public long Bandwidth { get; set; }

            [XmlElement("Codec")]
            public string Codec { get; set; }
        }

        [XmlRoot("Audio")]
        public class AudioModel
        {
            [XmlElement("SampleRate")]
            public int SampleRate { get; set; }

            [XmlElement("Bandwidth")]
            public long Bandwidth { get; set; }

            [XmlElement("Codec")]
            public string Codec { get; set; }
        }
    }
}
