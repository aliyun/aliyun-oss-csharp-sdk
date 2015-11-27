/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Authentication
{
    /// <summary>
    /// 默认鉴权工厂类
    /// </summary>
    public class DefaultCredentialsProvider : ICredentialsProvider
    {
        private volatile ICredentials _creds;

        /// <summary>
        /// 构造一个<see cref="DefaultCredentialsProvider"/>的实例
        /// </summary>
        /// <param name="creds"><see cref="ICredentials"/>接口的实例</param>
        public DefaultCredentialsProvider(ICredentials creds)
        {
            SetCredentials(creds);
        }

        /// <inheritdoc/>
        public void SetCredentials(ICredentials creds)
        {
            if (creds == null)
                throw new ArgumentNullException("creds");

            OssUtils.CheckCredentials(creds.AccessKeyId, creds.AccessKeySecret);
            _creds = creds;
        }

        /// <inheritdoc/>
        public ICredentials GetCredentials()
        {
            return _creds;
        }
    }
}
