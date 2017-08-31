/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Handlers;
using Aliyun.OSS.Commands;

namespace Aliyun.OSS.Common.Communication
{
    internal class ExecutionContext
    {
        /// <summary>
        /// List of HTTP response handlers. 
        /// </summary>
        private readonly IList<IResponseHandler> _responseHandlers = new List<IResponseHandler>();

        /// <summary>
        /// Gets or sets the request signer.
        /// </summary>
        public IRequestSigner Signer { get; set; }
        
        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        public ICredentials Credentials { get; set ;}

        /// <summary>
        /// Gets the list of <see cref="IResponseHandler" />.
        /// </summary>
        public IList<IResponseHandler> ResponseHandlers
        {
            get { return _responseHandlers; }
        }

        /// <summary>
        /// Gets or sets a concrete command associate with this context.
        /// </summary>
        public OssCommand Command { get; set; }

    }
}
