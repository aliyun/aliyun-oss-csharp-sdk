/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
using System.Collections.Generic;
namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get live channel info.
    /// </summary>
    public class GetLiveChannelInfoResult : GenericResult
    {
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public string Status { get; internal set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// Gets or sets the frag duration
        /// </summary>
        public long FragDuration { get; internal set; }

        /// <summary>
        /// Gets or sets the frag count
        /// </summary>
        public long FragCount { get; internal set; }

        /// <summary>
        /// Gets or sets playlist name
        /// </summary>
        public string PlaylistName { get; internal set; }
    }
}
