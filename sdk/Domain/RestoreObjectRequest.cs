using Aliyun.OSS.Util;
using System.Collections.Generic;
using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to restore an object from OSS.
    /// </summary>
    public class RestoreObjectRequest
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
        /// Gets or sets the Days
        /// </summary>
        public int? Days { get; set; }

        /// <summary>
        /// Gets or sets the TierType
        /// </summary>
        public TierType Tier { get; set; }

        /// <summary>
        /// Delete a new instance of <see cref="RestoreObjectRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        public RestoreObjectRequest(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
        }
    }
}
