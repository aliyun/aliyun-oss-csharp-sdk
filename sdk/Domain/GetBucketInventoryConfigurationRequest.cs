using System;
using Aliyun.OSS.Domain;

namespace Aliyun.OSS
{    /// <summary>
     /// The request class of the operation to get the bucket's inventory configuration.
     /// </summary>
    public class GetBucketInventoryConfigurationRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the inventory configuration id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Creates a new intance of <see cref="GetBucketInventoryConfigurationRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="id">inventory configuration id</param>
        public GetBucketInventoryConfigurationRequest(string bucketName, string id)
        {
            BucketName = bucketName;
            Id = id;
        }
    }
}
