/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get bucket worm.
    /// </summary>
    public class GetBucketWormResult : GenericResult
    {
        /// <summary>
        /// Set or Gets the worm id
        /// </summary>
        public string WormId { get; set; }

        /// <summary>
        /// Set or Gets the bucket worm state
        /// </summary>
        public BucketWormState State { get; set; }

        /// <summary>
        /// Set or Gets the retention period in days
        /// </summary>
        public int RetentionPeriodInDays { get; set; }

        /// <summary>
        /// Set or Gets the creation date
        /// </summary>
        public string CreationDate { get; set; }
    }
}
