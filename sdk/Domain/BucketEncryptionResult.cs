/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get bucket encryption config
    /// </summary>
    public class BucketEncryptionResult : GenericResult
    {
        /// <summary>
        /// Server-side encryption method.
        /// </summary>
        public string SSEAlgorithm { get; internal set; }

        /// <summary>
        /// The CMK id.
        /// </summary>
        public string KMSMasterKeyID { get; internal set; }
    }
}
