/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS.Common.Authentication
{
    /// <summary>
    /// ICredentialsProvider Interface
    /// </summary>
    public interface ICredentialsProvider
    {
        /// <summary>
        /// Sets the <see cref="ICredentials"/> instance
        /// </summary>
        /// <param name="creds">An instance of <see cref="ICredentials"/></param>
        void SetCredentials(ICredentials creds);

        /// <summary>
        /// Gets an instance of <see cref="ICredentials"/>
        /// </summary>
        /// <returns><see cref="ICredentials"/>ICredential instance</returns>
        ICredentials GetCredentials();
    }
}
