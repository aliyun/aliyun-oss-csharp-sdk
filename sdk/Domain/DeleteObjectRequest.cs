using Aliyun.OSS.Util;
using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to delete an object from OSS.
    /// </summary>
    public class DeleteObjectRequest
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
        /// Delete a new instance of <see cref="DeleteObjectRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        public DeleteObjectRequest(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
        }
    }
}
