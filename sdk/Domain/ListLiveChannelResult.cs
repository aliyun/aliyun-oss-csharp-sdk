/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to list live channel.
    /// </summary>
    public class ListLiveChannelResult : GenericResult
    {
        /// <summary>
        /// Gets or sets the live channel name prefix(optional).
        /// </summary>
        public string Prefix { get; internal set; }

        /// <summary>
        /// Gets or sets the live channel name marker.Its value should be same as the ListLiveChannelRequest.Marker.
        /// </summary>
        public string Marker { get; internal set; }

        /// <summary>
        /// Gets or sets the max entries to return.
        /// By default it's 100.
        /// </summary>
        public int? MaxKeys { get; internal set; }

        /// <summary>
        /// Gets or sets the flag of truncated. If it's true, means not all live channels have been returned.
        /// </summary>
        public bool? IsTruncated { get; internal set; }

        /// <summary>
        /// Gets the next marker's value. Assign this value to the next call's ListLiveChannelRequest.marker.
        /// </summary>
        public string NextMarker { get; internal set; }

        /// <summary>
        /// Gets the live channel iterator.
        /// </summary>
        public IEnumerable<LiveChannel> LiveChannels { get; internal set; }
    }
}
