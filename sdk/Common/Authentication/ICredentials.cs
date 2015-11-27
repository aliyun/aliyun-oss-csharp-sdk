/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS.Common.Authentication
{
    /// <summary>
    /// 鉴权的接口
    /// </summary>
    public interface ICredentials
    {
        /// <summary>
        /// OSS的访问ID
        /// </summary>
        string AccessKeyId { get; }

        /// <summary>
        /// OSS的访问密钥
        /// </summary>
        string AccessKeySecret { get; }

        /// <summary>
        /// STS提供的安全令牌
        /// </summary>
        string SecurityToken { get; }

        /// <summary>
        /// 是否使用了STS提供的安全令牌
        /// </summary>
        bool UseToken { get; }
    }
}
