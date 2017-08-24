/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Net;
using System.Threading;
using System.IO;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Communication
{
    /// <summary>
    /// Implementation of <see cref="IServiceClient"/> that will auto-retry HTTP requests 
    /// when encountering some specific exceptions or failures.
    /// </summary>
    internal class RetryableServiceClient : IServiceClient
    {

        #region Fields & Properties

        private const int DefaultMaxRetryTimes = 3;
        private const int DefaultRetryPauseScale = 300; // milliseconds.

        private readonly IServiceClient _innerClient;

        public OssFunc<Exception, bool> ShouldRetryCallback { get; set; }
        public int MaxRetryTimes { get; set; }

        #endregion

        #region Constructors

        public RetryableServiceClient(IServiceClient innerClient)
        {
            _innerClient = innerClient;
            MaxRetryTimes = DefaultMaxRetryTimes;
        }

        #endregion

        #region IServiceClient Members

        internal IServiceClient InnerServiceClient()
        {
            return _innerClient;
        }

        public ServiceResponse Send(ServiceRequest request, ExecutionContext context)
        {
            return SendImpl(request, context, 0);
        }

        private ServiceResponse SendImpl(ServiceRequest request, ExecutionContext context, int retryTimes)
        {
            long originalContentPosition = -1;
            try
            {
                if (request.Content != null && request.Content.CanSeek)
                    originalContentPosition = request.Content.Position;
                return _innerClient.Send(request, context);
            }
            catch (Exception ex)
            {
                if (ShouldRetry(request, ex, retryTimes))
                {
                    if (request.Content != null && (originalContentPosition >= 0 && request.Content.CanSeek))
                        request.Content.Seek(originalContentPosition, SeekOrigin.Begin);
                    
                    Pause(retryTimes);
                    
                    return SendImpl(request, context, ++retryTimes);
                }

                // Rethrow
                throw;
            }
        }

        public IAsyncResult BeginSend(ServiceRequest request, ExecutionContext context, 
            AsyncCallback callback, object state)
        {
            var asyncResult = new RetryableAsyncResult(callback, state, request, context);
            BeginSendImpl(request, context, asyncResult);

            return asyncResult;
        }

        private void BeginSendImpl(ServiceRequest request, ExecutionContext context, 
            RetryableAsyncResult asyncResult)
        {
            if (asyncResult.InnerAsyncResult != null)
                asyncResult.InnerAsyncResult.Dispose();
            asyncResult.InnerAsyncResult =
                _innerClient.BeginSend(request, context, asyncResult.Callback, asyncResult) as AsyncResult;
        }

        public ServiceResponse EndSend(IAsyncResult ar)
        {
            if (ar == null)
                throw new ArgumentNullException("ar");

            var asyncResult = ar as AsyncResult<ServiceResponse>;
            RetryableAsyncResult retryableAsyncResult = ar.AsyncState as RetryableAsyncResult;
            if (asyncResult == null || retryableAsyncResult == null)
                throw new InvalidOperationException("Invalid asynchronous invocation status.");

            try
            {
                var response = asyncResult.GetResult();
                return response;
            }
            catch (Exception ex)
            {
                if (retryableAsyncResult.OriginalContentPosition >= 0)
                {
                    retryableAsyncResult.Request.Content.Seek(retryableAsyncResult.OriginalContentPosition,
                        SeekOrigin.Begin);
                }

                if (ShouldRetry(retryableAsyncResult.Request, ex, retryableAsyncResult.Retries))
                {
                    Pause(retryableAsyncResult.Retries++);
                    BeginSendImpl(retryableAsyncResult.Request, retryableAsyncResult.Context, retryableAsyncResult);
                }

                // Rethrow
                throw;
            }
            finally
            {
                asyncResult.Dispose();
            }
        }

        private bool ShouldRetry(ServiceRequest request, Exception ex, int retryTimes)
        {
            if (retryTimes > MaxRetryTimes || !request.IsRepeatable)
                return false;

            var webException = ex as WebException;
            if (webException != null)
            {
                var httpWebResponse = webException.Response as HttpWebResponse;
                if (httpWebResponse != null &&
                    (httpWebResponse.StatusCode == HttpStatusCode.ServiceUnavailable ||
                     httpWebResponse.StatusCode == HttpStatusCode.InternalServerError))
                {
                    return true;
                }
            }

            if (ShouldRetryCallback != null && ShouldRetryCallback(ex))
                return true;

            return false;
        }

        private static void Pause(int retryTimes)
        {
            // make the pause time increase exponentially based on an assumption 
            // that the more times it retries, the less probability it succeeds.
            var delay = (int)Math.Pow(2, retryTimes) * DefaultRetryPauseScale;
            Thread.Sleep(delay);
        }

        #endregion

    }

    internal class RetryableAsyncResult : AsyncResult<ServiceResponse>
    {
        public ServiceRequest Request { get; private set; }

        public ExecutionContext Context { get; private set; }

        public AsyncResult InnerAsyncResult { get; set; }

        public int Retries { get; set; }

        public long OriginalContentPosition { get; private set; }

        public RetryableAsyncResult(AsyncCallback callback, object state,
                                    ServiceRequest request, ExecutionContext context)
            : base(callback, state)
        {
            Request = request;
            Context = context;
            OriginalContentPosition = (request.Content != null && request.Content.CanSeek)
                ? request.Content.Position : -1;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && InnerAsyncResult != null)
            {
                InnerAsyncResult.Dispose();
                InnerAsyncResult = null;
            }
        }
    }
}


