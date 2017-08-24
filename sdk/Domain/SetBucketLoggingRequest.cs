/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set bucket logging configuration.
    /// </summary>
   public class SetBucketLoggingRequest
    {
        /// <summary>
        /// Gets bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the target bucket name of the logging file
        /// </summary>
        public string TargetBucket { get; private set; }

        /// <summary>
        /// Gets the target prefix.
        /// </summary>
        public string TargetPrefix { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="SetBucketLoggingRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="targetBucket">target bucket</param>
        /// <param name="targetPrefix">target prefix</param>
        public SetBucketLoggingRequest(string bucketName, string targetBucket, string targetPrefix)
        {
            BucketName = bucketName;
            TargetBucket = targetBucket;
            TargetPrefix = targetPrefix;
        }
    }
}
