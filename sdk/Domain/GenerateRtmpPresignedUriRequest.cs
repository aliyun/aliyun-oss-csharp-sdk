/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
 
using System;
using System.Collections.Generic;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to sign the rtmp URL
    /// </summary>
    public class GenerateRtmpPresignedUriRequest
    {
        private IDictionary<string, string> _queryParams = new Dictionary<string, string>();

        /// <summary>
        /// Bucket name getter/setter
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Channel name getter/setter
        /// </summary>
        public string ChannelName { get; private set; }

        /// <summary>
        /// Playlist name getter/setter
        /// </summary>
        public string PlaylistName { get; private set; }

        /// <summary>
        /// Getter/setter of the expiration time of the signed URL.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Gets or sets query parameters
        /// </summary>
        public IDictionary<string, string> QueryParams
        {
            get 
            {
                return _queryParams; 
            }
            set
            {
                if (value == null)
                    throw new ArgumentException("QueryParams should not be null");
                _queryParams = value;
            }
        }

        /// <summary>
        /// Add a query parameter
        /// </summary>
        /// <param name="param">param name</param>
        /// <param name="value">param value</param>
        public void AddQueryParam(string param, string value)
        {
            _queryParams.Add(param, value);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GenerateRtmpPresignedUriRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="channelName">object key</param>
        public GenerateRtmpPresignedUriRequest(string bucketName, string channelName, string playlistName)
        {
            OssUtils.CheckBucketName(bucketName);
            BucketName = bucketName;
            ChannelName = channelName;
            PlaylistName = playlistName;
            Expiration = DateTime.Now.AddMinutes(15);
        }
    }
}
