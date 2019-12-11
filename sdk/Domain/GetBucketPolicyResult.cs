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
        public string Policy { get; set; }
    }
}
