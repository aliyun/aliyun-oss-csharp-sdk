/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get Bucket Worm Result.
    /// </summary>
    public class GetBucketWormResult : GenericResult
    {
        /// <summary>
        /// Set or Gets the wormId
        /// </summary>
        public string WormId { get; set; }

        /// <summary>
        /// Set or Gets the Bucket Worm State
        /// </summary>
        public BucketWormState State { get; set; }

        /// <summary>
        /// Set or Gets the RetentionPeriodInDays
        /// </summary>
        public int RetentionPeriodInDays { get; set; }

        /// <summary>
        /// Set or Gets the CreationDate
        /// </summary>
        public string CreationDate { get; set; }
    }
}
