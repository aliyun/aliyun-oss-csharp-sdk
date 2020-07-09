
namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set the live channel stauts.
    /// </summary>
    public class SetLiveChannelStatusRequest
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
        /// Gets or sets the status
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Set a new instance of <see cref="SetLiveChannelStatusRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="channelName">live channel name</param>
        /// <param name="status">status</param>
        public SetLiveChannelStatusRequest(string bucketName, string channelName, string status)
        {
            BucketName = bucketName;
            ChannelName = channelName;
            Status = status;
        }
    }
}
