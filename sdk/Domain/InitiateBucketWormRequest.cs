/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to initiate bucket worm.
    /// </summary>
    public class InitiateBucketWormRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the retention period in days
        /// </summary>
        public int RetentionPeriodInDays { get; private set; }

        /// <summary>
        /// Creates a instance of <see cref="InitiateBucketWormRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="days">RetentionPeriodInDays</param>
        public InitiateBucketWormRequest(string bucketName, int days)
        {
            BucketName = bucketName;
            RetentionPeriodInDays = days;
        }
    }

}

