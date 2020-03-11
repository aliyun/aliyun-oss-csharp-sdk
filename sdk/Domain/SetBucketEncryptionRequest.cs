/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set bucket encryption configuration.
    /// </summary>
    public class SetBucketEncryptionRequest
    {
        /// <summary>
        /// Gets bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets server-side encryption method.
        /// </summary>
        public string SSEAlgorithm { get; private set; }

        /// <summary>
        /// Gets the CMK id.
        /// </summary>
        public string KMSMasterKeyID { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="SetBucketEncryptionRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="algorithm">server-side encryption method</param>
        public SetBucketEncryptionRequest(string bucketName, string algorithm)
        {
            BucketName = bucketName;
            SSEAlgorithm = algorithm;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SetBucketEncryptionRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="algorithm">server-side encryption method</param>
        /// <param name="id">the CMK id</param>
        public SetBucketEncryptionRequest(string bucketName, string algorithm, string id)
        {
            BucketName = bucketName;
            SSEAlgorithm = algorithm;
            KMSMasterKeyID = id;
        }
    }
}
