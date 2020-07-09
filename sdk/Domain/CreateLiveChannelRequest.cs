
namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to create a live channel.
    /// </summary>
    public class CreateLiveChannelRequest
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
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the frag duration
        /// </summary>
        public long FragDuration { get; set; }

        /// <summary>
        /// Gets or sets the frag count
        /// </summary>
        public long FragCount { get; set; }

        /// <summary>
        /// Gets or sets playlist name
        /// </summary>
        public string PlaylistName { get; set; }

        /// <summary>
        /// Gets or sets role name of snapshot
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets destination bucket of snapshot
        /// </summary>
        public string DestBucket { get; set; }

        /// <summary>
        /// Gets or sets notify topic of snapshot
        /// </summary>
        public string NotifyTopic { get; set; }

        /// <summary>
        /// Gets or sets interval of snapshot
        /// </summary>
        public long Interval { get; set; }

        /// <summary>
        /// Set a new instance of <see cref="CreateLiveChannelRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="channelName">live channel name</param>
        public CreateLiveChannelRequest(string bucketName, string channelName)
        {
            BucketName = bucketName;
            ChannelName = channelName;
            Type = "HLS";
            FragDuration = 5;
            FragCount = 3;
            PlaylistName = "playlist.m3u8";
        }
    }
}
