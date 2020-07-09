
using System;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to create a vod playlist.
    /// </summary>
    public class PostVodPlaylistRequest
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
        /// Gets or sets the playlist name
        /// </summary>
        public string PlaylistName { get; set; }

        /// <summary>
        /// Gets or sets the start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Sets a new instance of <see cref="PostVodPlaylistRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="channelName">live channel name</param>
        public PostVodPlaylistRequest(string bucketName, string channelName)
        {
            BucketName = bucketName;
            ChannelName = channelName;
        }
    }
}
