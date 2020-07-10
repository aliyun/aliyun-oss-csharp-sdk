/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS.Model
{
    /// <summary>
    /// The result class of the operation to get bucket's inventory configuration.
    /// </summary>
    public class GetBucketInventoryConfigurationResult : GenericResult
    {
        /// <summary>
        /// The bucket inventory configuration.
        /// </summary>
        public InventoryConfiguration Configuration { get; set; }
    }
}
