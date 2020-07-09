/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
using Aliyun.OSS.Model;

namespace Aliyun.OSS
{    /// <summary>
     /// The result class of the operation to get vod's playlist.
     /// </summary>
    public class GetVodPlaylistResult : GenericResult
    {
        /// <summary>
        /// The vod's playlist.
        /// </summary>
        public string Playlist { get; internal set; }
    }
}
