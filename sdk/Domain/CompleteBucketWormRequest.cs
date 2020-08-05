/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to CompleteBucketWorm.
    /// </summary>
    public class CompleteBucketWormRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the worm id
        /// </summary>
        public string WormId { get; private set; }

        /// <summary>
        /// Creates a instance of <see cref="CompleteBucketWormRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="id">wormId</param>
        public CompleteBucketWormRequest(string bucketName, string id)
        {
            BucketName = bucketName;
            WormId = id;
        }
    }

}

