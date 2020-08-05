/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to initiate bucket worm.
    /// </summary>
    public class InitiateBucketWormResult : GenericResult
    {
        /// <summary>
        /// Set or Gets the worm id
        /// </summary>
        public string WormId { get; set; }
    }
}
