
namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to get the object meta.
    /// </summary>
    public class GetObjectMetadataRequest
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
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; }

        /// <summary>
        /// Gets or sets the version id
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        /// Delete a new instance of <see cref="GetObjectAclRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        public GetObjectMetadataRequest(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
        }
    }
}
