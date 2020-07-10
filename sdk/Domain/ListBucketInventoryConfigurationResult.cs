/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;

namespace Aliyun.OSS.Model
{
    /// <summary>
    /// The result class of the operation to list bucket's inventory configuration.
    /// </summary>
    public class ListBucketInventoryConfigurationResult : GenericResult
    {
        /// <summary>
        /// Gets or sets the flag of truncated.
        /// If it's true, means not all configurations have been returned.
        /// </summary>
        public bool IsTruncated { get; internal set; }

        /// <summary>
        /// Gets the next continuation token.
        /// Assign this value to the next call's ListBucketInventoryConfigurationRequest.ContinuationToken.
        /// </summary>
        public string NextContinuationToken { get; internal set; }

        /// <summary>
        /// Gets the inventory configuration iterator.
        /// </summary>
        public IEnumerable<InventoryConfiguration> Configurations { get; internal set; }
    }
}

