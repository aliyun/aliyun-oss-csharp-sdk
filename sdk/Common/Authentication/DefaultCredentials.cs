/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Authentication
{
    /// <summary>
    /// Default Credential class
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
        /// creates a instance of <see cref="DefaultCredentials"/>
        /// </summary>
        /// <param name="accessKeyId">OSS access key Id</param>
        /// <param name="accessKeySecret">OSS access secret</param>
        /// <param name="securityToken">STS security token</param>
        public DefaultCredentials(string accessKeyId, string accessKeySecret, string securityToken)
        {
            OssUtils.CheckCredentials(accessKeyId, accessKeySecret);

            AccessKeyId = accessKeyId.Trim();
            AccessKeySecret = accessKeySecret.Trim();
            SecurityToken = securityToken ?? string.Empty;
        }
    }
}
