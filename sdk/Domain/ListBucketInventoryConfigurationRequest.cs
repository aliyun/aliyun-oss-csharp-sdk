using System;
using Aliyun.OSS.Domain;

namespace Aliyun.OSS
{    /// <summary>
     /// The request class of the operation to list the bucket's inventory configuration.
     /// </summary>
    public class ListBucketInventoryConfigurationRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the inventory continuation token
        /// </summary>
        public string ContinuationToken { get; private set; }

        /// <summary>
        /// Creates a new intance of <see cref="ListBucketInventoryConfigurationRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="token">continuation token</param>
        public ListBucketInventoryConfigurationRequest(string bucketName, string token)
        {
            BucketName = bucketName;
            ContinuationToken = token;
        }
    }
}
