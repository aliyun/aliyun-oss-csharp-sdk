
namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to get live channel info.
    /// </summary>
    public class GetLiveChannelInfoRequest
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
        /// Gets a new instance of <see cref="GetLiveChannelInfoRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="channelName">live channel name</param>
        public GetLiveChannelInfoRequest(string bucketName, string channelName)
        {
            BucketName = bucketName;
            ChannelName = channelName;
        }
    }
}
