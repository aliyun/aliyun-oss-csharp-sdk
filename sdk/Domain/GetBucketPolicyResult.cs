/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
using Aliyun.OSS.Model;

namespace Aliyun.OSS
{    /// <summary>
     /// The result class of the operation to get bucket's policy.
     /// </summary>
    public class GetBucketPolicyResult :GenericResult
    {
        /// <summary>
        /// The bucket's policy.
        /// </summary>
        public string Policy { get; internal set; }
    }
}
