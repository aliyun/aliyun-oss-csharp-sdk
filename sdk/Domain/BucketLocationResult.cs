/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to get bucket's location.
    /// </summary>
    public class BucketLocationResult : GenericResult
    {
        /// <summary>
        /// The bucket location.
        /// </summary>
        public string Location { get; internal set; }
    }
}
