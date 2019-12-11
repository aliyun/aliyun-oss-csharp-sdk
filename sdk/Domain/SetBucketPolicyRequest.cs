/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{    /// <summary>
     /// The request class of the operation to set the bucket Policy.
     /// </summary>
    public class SetBucketPolicyRequest
    {
        /// <summary>
        /// Gets the bucket Policy
        /// </summary>
        public string Policy { get; private set; }

        /// <summary>
        /// Gets the bucket Name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Creates a instance of <see cref="SetBucketPolicyRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="policy">user Policy</param>
        public SetBucketPolicyRequest(string bucketName, string policy)
        {
            BucketName = bucketName;
            Policy = policy;
        }

    }
}
