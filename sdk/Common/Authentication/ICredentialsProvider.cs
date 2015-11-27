/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS.Common.Authentication
{
    /// <summary>
    /// 鉴权工厂的接口
    /// </summary>
    public interface ICredentialsProvider
    {
        /// <summary>
        /// 设置一个新的<see cref="ICredentials"/>
        /// </summary>
        /// <param name="creds">新的<see cref="ICredentials"/></param>
        void SetCredentials(ICredentials creds);

        /// <summary>
        /// 获取一个<see cref="ICredentials"/>
        /// </summary>
        /// <returns><see cref="ICredentials"/>实例</returns>
        ICredentials GetCredentials();
    }
}
