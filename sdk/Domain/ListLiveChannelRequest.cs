/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to list the live channel.
    /// </summary>
    public class ListLiveChannelRequest
    {
        /// <summary>
        /// Gets or sets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets or sets the live channel name prefix to list (optional)
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the marker of the live channel name.
        /// </summary>
        public string Marker { get; set; }

        /// <summary>
        /// Gets or sets the max entries to return. By default is 100.
        /// </summary>
        public int? MaxKeys { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="ListLiveChannelRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public ListLiveChannelRequest(string bucketName)
        {
            BucketName = bucketName;
        }
    }
}
