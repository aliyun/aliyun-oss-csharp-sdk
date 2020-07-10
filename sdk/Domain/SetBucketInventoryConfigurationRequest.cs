/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set the bucket inventory configuration.
    /// </summary>
    public class SetBucketInventoryConfigurationRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the inventory configuration
        /// </summary>
        public InventoryConfiguration Configuration { get; private set; }

        /// <summary>
        /// Creates a instance of <see cref="SetBucketInventoryConfigurationRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="configuration">inventory configuration</param>
        public SetBucketInventoryConfigurationRequest(string bucketName, InventoryConfiguration configuration)
        {
            BucketName = bucketName;
            Configuration = configuration;
        }
    }
}
