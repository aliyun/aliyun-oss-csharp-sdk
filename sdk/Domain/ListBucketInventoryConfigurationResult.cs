/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    /// <summary>
    /// The result class of the operation to list bucket's InventoryConfiguration.
    /// </summary>
    public class ListBucketInventoryConfigurationResult : GenericResult
    {
        /// <summary>
        /// The bucket ListBucketInventoryConfigurationResult .
        /// </summary>

        public InventoryConfiguration[] BucketInventory { get; set; }

        public bool IsTruncated { get; set; }

        public string NextContinuationToken { get; set; }
    }
}

