/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get bucket's storage capacity.
    /// </summary>
    public class GetBucketStorageCapacityResult : GenericResult
    {
        /// <summary>
        /// The bucket storage capacity.
        /// </summary>
        public long StorageCapacity { get; internal set; }
    }
}
