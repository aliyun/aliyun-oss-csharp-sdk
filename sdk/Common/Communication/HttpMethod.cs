/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS.Common.Communication
{
    /// <summary>
    /// Represents a HTTP method.
    /// </summary>
    internal enum HttpMethod
    {
        /// <summary>
        /// Represents HTTP GET. Default value.
        /// </summary>
        Get = 0,

        /// <summary>
        /// Represents HTTP DELETE.
        /// </summary>
        Delete,

        /// <summary>
        /// Represents HTTP HEAD.
        /// </summary>
        Head,

        /// <summary>
        /// Represents HTTP POST.
        /// </summary>
        Post,

        /// <summary>
        /// Represents HTTP PUT.
        /// </summary>
        Put,

        /// <summary>
        /// Represents HTTP OPTIONS.
        /// </summary>
        Options,
    }
}
