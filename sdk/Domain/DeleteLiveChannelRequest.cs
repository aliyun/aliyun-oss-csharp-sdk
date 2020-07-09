
namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to delete the live channel.
    /// </summary>
    public class DeleteLiveChannelRequest
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
        /// Delete a new instance of <see cref="DeleteLiveChannelRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="channelName">live channel name</param>
        public DeleteLiveChannelRequest(string bucketName, string channelName)
        {
            BucketName = bucketName;
            ChannelName = channelName;
        }
    }
}
