/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;

namespace Aliyun.OSS.Common.Communication
{
    /// <summary>
    /// Represent the channel that communicates with an Aliyun Open Service.
    /// </summary>
    internal interface IServiceClient
    {
        /// <summary>
        /// Sends a request to the service.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>The response data.</returns>
        ServiceResponse Send(ServiceRequest request, ExecutionContext context);

        /// <summary>
        /// Begins to send a request to the service asynchronously.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="callback">User callback.</param>
        /// <param name="state">User state.</param>
        /// <returns>An instance of <see cref="IAsyncResult"/>.</returns>
        IAsyncResult BeginSend(ServiceRequest request, ExecutionContext context, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the asynchronous operation.
        /// </summary>
        /// <param name="asyncResult">An instance of <see cref="IAsyncResult"/>.</param>
        /// <returns>The response data.</returns>
        ServiceResponse EndSend(IAsyncResult asyncResult);
    }
}
