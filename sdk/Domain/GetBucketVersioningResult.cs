/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get bucket's versioning configuration.
    /// </summary>
    public class GetBucketVersioningResult : GenericResult
    {
        /// <summary>
        /// Gets the versioning status
        /// </summary>
        public VersioningStatus Status { get; set; }
    }
}
