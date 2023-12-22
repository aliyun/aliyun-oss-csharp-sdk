/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// supported signature version definition. V1 is the default one.
    /// </summary>
    public enum SignatureVersion
    {
        /// <summary>
        /// V1
        /// </summary>
        [StringValue("v1")]
        V1 = 0,

        /// <summary>
        /// V4
        /// </summary>
        [StringValue("v4")]
        V4
    }
}
