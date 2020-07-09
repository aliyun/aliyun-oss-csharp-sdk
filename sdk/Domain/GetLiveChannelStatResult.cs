/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
using System;
using System.Collections.Generic;
namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get live channel stat.
    /// </summary>
    public class GetLiveChannelStatResult : GenericResult
    {
        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        public string Status { get; internal set; }

        /// <summary>
        /// Gets or sets the connected time
        /// </summary>
        public DateTime ConnectedTime { get; internal set; }

        /// <summary>
        /// Gets or sets the remote address
        /// </summary>
        public string RemoteAddr { get; internal set; }

        /// <summary>
        /// Gets or sets the video width
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        /// Gets or sets the video height
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        /// Gets or sets the video frame rate
        /// </summary>
        public int FrameRate { get; internal set; }

        /// <summary>
        /// Gets or sets the video bandwidth
        /// </summary>
        public long VideoBandwidth { get; internal set; }

        /// <summary>
        /// Gets or sets the video codec
        /// </summary>
        public string VideoCodec { get; internal set; }

        /// <summary>
        /// Gets or sets the audio sample rate
        /// </summary>
        public int SampleRate { get; internal set; }

        /// <summary>
        /// Gets or sets the audio bandwidth
        /// </summary>
        public long AudioBandwidth { get; internal set; }

        /// <summary>
        /// Gets or sets the audio codec
        /// </summary>
        public string AudioCodec { get; internal set; }
    }
}
