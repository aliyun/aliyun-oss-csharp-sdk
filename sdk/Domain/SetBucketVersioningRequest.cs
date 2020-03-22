/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set the bucket versioning configuration.
    /// </summary>
    public class SetBucketVersioningRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the versioning status
        /// </summary>
        public VersioningStatus Status { get; private set; }

        /// <summary>
        /// Creates a instance of <see cref="SetBucketVersioningRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="status">versioning status</param>
        public SetBucketVersioningRequest(string bucketName, VersioningStatus status)
        {
            BucketName = bucketName;
            Status = status;
        }
    }

}

