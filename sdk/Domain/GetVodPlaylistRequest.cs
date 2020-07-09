
using System;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to get a vod playlist.
    /// </summary>
    public class GetVodPlaylistRequest
    {
        /// <summary>
        /// Gets or sets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets or sets the channel name
        /// </summary>
        public string ChannelName { get; private set; }

        /// <summary>
        /// Gets or sets the start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Sets a new instance of <see cref="GetVodPlaylistRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="channelName">live channel name</param>
        public GetVodPlaylistRequest(string bucketName, string channelName)
        {
            BucketName = bucketName;
            ChannelName = channelName;
        }
    }
}
