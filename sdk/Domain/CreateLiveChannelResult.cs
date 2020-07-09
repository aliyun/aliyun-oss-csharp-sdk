/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
using Aliyun.OSS.Model;

namespace Aliyun.OSS
{    /// <summary>
     /// The result class of the operation to create live channel.
     /// </summary>
    public class CreateLiveChannelResult : GenericResult
    {
        /// <summary>
        /// The publish url.
        /// </summary>
        public string PublishUrl { get; internal set; }

        /// <summary>
        /// The play url.
        /// </summary>
        public string PlayUrl { get; internal set; }
    }
}
