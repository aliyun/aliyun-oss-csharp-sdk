/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Authentication
{
    /// <summary>
    /// 默认鉴权类
    /// </summary>
    public class DefaultCredentials : ICredentials 
    {
        /// <inheritdoc/>
        public string AccessKeyId { get; private set; }

        /// <inheritdoc/>
        public string AccessKeySecret { get; private set; }

        /// <inheritdoc/>
        public string SecurityToken { get; private set; }

        /// <inheritdoc/>
        public bool UseToken { get { return !string.IsNullOrEmpty(SecurityToken); } }

        /// <summary>
        /// 构造一个<see cref="DefaultCredentials"/>的实例
        /// </summary>
        /// <param name="accessKeyId">OSS的访问ID</param>
        /// <param name="accessKeySecret">OSS的访问密钥</param>
        /// <param name="securityToken">STS提供的安全令牌</param>
        public DefaultCredentials(string accessKeyId, string accessKeySecret, string securityToken)
        {
            OssUtils.CheckCredentials(accessKeyId, accessKeySecret);

            AccessKeyId = accessKeyId.Trim();
            AccessKeySecret = accessKeySecret.Trim();
            SecurityToken = securityToken ?? string.Empty;
        }
    }
}
