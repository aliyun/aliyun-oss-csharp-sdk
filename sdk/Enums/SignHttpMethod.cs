/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 用于预签名URL的请求方法.
    /// </summary>
    public enum SignHttpMethod
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
    }
}
