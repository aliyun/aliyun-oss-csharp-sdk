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
    /// The result class of the operation to get bucket's InventoryConfiguration.
    /// </summary>
    public class BucketInventoryConfigurationResult : GenericResult
    {
        /// <summary>
        /// The bucket InventoryConfiguration.
        /// </summary>
        public InventoryConfiguration InventoryConfiguration { get; set; }
    }
}
