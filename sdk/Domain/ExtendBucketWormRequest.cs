/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to ExtendBucketWorm.
    /// </summary>
    public class ExtendBucketWormRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the RetentionPeriodInDays
        /// </summary>
        public int RetentionPeriodInDays { get; private set; }

        /// <summary>
        /// Gets the worm id
        /// </summary>
        public string WormId { get; private set; }

        /// <summary>
        /// Creates a instance of <see cref="ExtendBucketWormRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="days">RetentionPeriodInDays</param>
        /// <param name="id">wormId</param>
        public ExtendBucketWormRequest(string bucketName, int days, string id)
        {
            BucketName = bucketName;
            RetentionPeriodInDays = days;
            WormId = id;
        }
    }

}

