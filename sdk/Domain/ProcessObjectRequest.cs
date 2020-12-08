
namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to process the object.
    /// </summary>
    public class ProcessObjectRequest
    {
        /// <summary>
        /// Gets or sets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets or sets the object key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets the process
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ProcessObjectRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        public ProcessObjectRequest(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
        }
    }
}
