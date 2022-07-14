/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Handlers;
using Aliyun.OSS.Properties;
using System.Threading.Tasks;
using System.Threading;

namespace Aliyun.OSS.Common.Communication
{
    /// <summary>
    /// The default implementation of <see cref="IServiceClient" />.
    /// </summary>
    internal abstract class ServiceClient : IServiceClient
    {

        #region Fields and Properties

        private readonly ClientConfiguration _configuration;
        
        internal ClientConfiguration Configuration
        {
            get { return _configuration; }
        }

        #endregion

        #region Constructors

        protected ServiceClient(ClientConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public static ServiceClient Create(ClientConfiguration configuration)
        {
#if NETCOREAPP2_0
            //return new ServiceClientImpl(configuration);
            if (configuration.UseNewServiceClient)
            {
                return new ServiceClientNewImpl(configuration);
            }
            else
            {
                return new ServiceClientImpl(configuration);
            }
#else
            return new ServiceClientImpl(configuration);
#endif
        }

#endregion
        
        #region IServiceClient Members

        public ServiceResponse Send(ServiceRequest request, ExecutionContext context)
        {
            SignRequest(request, context);
            var response = SendCore(request, context);
            HandleResponse(response, context.ResponseHandlers);
            return response;
        }

        public IAsyncResult BeginSend(ServiceRequest request, ExecutionContext context, 
            AsyncCallback callback, object state)
        {
            SignRequest(request, context);
            return BeginSendCore(request, context, callback, state);
        }

        public ServiceResponse EndSend(IAsyncResult aysncResult)
        {
            var ar = aysncResult as AsyncResult<ServiceResponse>;
            Debug.Assert(ar != null);
            if (ar == null)
            {
                throw new ArgumentException("ar must be type of AsyncResult<ServiceResponse>");
            }

            try
            {
                // Must dispose the async result instance.
                var result = ar.GetResult();
                ar.Dispose();

                return result;
            }
            catch (ObjectDisposedException)
            {
                throw new InvalidOperationException(Resources.ExceptionEndOperationHasBeenCalled);
            }
        }

        #endregion

        protected abstract ServiceResponse SendCore(ServiceRequest request, ExecutionContext context);
        protected abstract Task<ServiceResponse> SendCoreAsync(ServiceRequest request, ExecutionContext context, CancellationToken cancellationToken=default);

        protected abstract IAsyncResult BeginSendCore(ServiceRequest request, ExecutionContext context, 
            AsyncCallback callback, Object state);
        
        private static void SignRequest(ServiceRequest request, ExecutionContext context)
        {
            if (context.Signer != null)
                context.Signer.Sign(request, context.Credentials);
        }
        
        protected static void HandleResponse(ServiceResponse response, IEnumerable<IResponseHandler> handlers)
        {
            foreach (var handler in handlers)
                handler.Handle(response);
        }

        public async Task<ServiceResponse> SendAsync(ServiceRequest request, ExecutionContext context, System.Threading.CancellationToken cancellationToken = default)
        {
            SignRequest(request, context);
            var response = await SendCoreAsync(request, context);
            HandleResponse(response, context.ResponseHandlers);
            return response;
        }
    }
}
