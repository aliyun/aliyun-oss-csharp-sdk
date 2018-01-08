/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Communication
{
    /// <summary>
    /// The default  implementation of <see cref="ServiceClient"/> that
    /// used to communicate with Aliyun OSS via HTTP protocol.
    /// </summary>
    internal class ServiceClientImpl : ServiceClient
    {

        #region Embeded Classes

        /// <summary>
        /// Represents the async operation of requests in <see cref="ServiceClientImpl"/>.
        /// </summary>
        public class HttpAsyncResult : AsyncResult<ServiceResponse>
        {
            public HttpWebRequest WebRequest { get; set; }
            
            public ExecutionContext Context { get; set; }

            internal ServiceRequest Request { get; set; }

            public HttpAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            { }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing && Request != null)
                {
                    Request.Dispose();
                    Request = null;
                }
            }
        }

        /// <summary>
        /// Represents the response data of <see cref="ServiceClientImpl"/> requests.
        /// </summary>
        internal class ResponseImpl : ServiceResponse
        {
            private bool _disposed;
            private HttpWebResponse _response;
            private readonly Exception _failure;
            private IDictionary<string, string> _headers;
            
            public override HttpStatusCode StatusCode
            {
                get { return _response.StatusCode; }
            }
            
            public override Exception Failure 
            {
                get { return _failure; }
            }

            public override IDictionary<string, string> Headers
            {
                get
                {
                    ThrowIfObjectDisposed();
                    return _headers ?? (_headers = GetResponseHeaders(_response));
                }
            }

            public override Stream Content
            {
                get
                {
                    ThrowIfObjectDisposed();

                    try
                    {
                        return (_response != null) ? _response.GetResponseStream() : null;
                    }
                    catch (ProtocolViolationException ex)
                    {
                        throw new InvalidOperationException(ex.Message, ex);
                    }
                }
            }

            public ResponseImpl(HttpWebResponse httpWebResponse)
            {
                _response = httpWebResponse;
            }

            public ResponseImpl(WebException failure)
            {
                var httpWebResponse = failure.Response as HttpWebResponse;
                _failure = failure;
                _response = httpWebResponse;
            }

            private static IDictionary<string, string> GetResponseHeaders(HttpWebResponse response)
            {
                var headers = response.Headers;
                var result = new Dictionary<string, string>(headers.Count);

                for (var i = 0; i < headers.Count; i++)
                {
                    var key = headers.Keys[i];
                    var value = headers.Get(key);
                    result.Add(key, HttpUtils.Reencode(value, HttpUtils.Iso88591Charset, HttpUtils.Utf8Charset));
                }

                return result;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (_disposed)
                    return;


                if (disposing)
                {
                    if (_response != null)
                    {
                        _response.Close();
                        _response = null;
                    }
                    _disposed = true;
                }
            }

            private void ThrowIfObjectDisposed()
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion

        #region Constructors

        public ServiceClientImpl(ClientConfiguration configuration)
            : base(configuration)
        {
        }

        #endregion

        #region Implementations

        protected override ServiceResponse SendCore(ServiceRequest serviceRequest,
                                                    ExecutionContext context)
        {
            var request = HttpFactory.CreateWebRequest(serviceRequest, Configuration);
            SetRequestContent(request, serviceRequest, Configuration);
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                return new ResponseImpl(response);
            }
            catch (WebException ex)
            {
                return HandleException(ex);
            }
        }

        protected override IAsyncResult BeginSendCore(ServiceRequest serviceRequest,
                                                      ExecutionContext context,
                                                      AsyncCallback callback, object state)
        {
            var request = HttpFactory.CreateWebRequest(serviceRequest, Configuration);

            var asyncResult = new HttpAsyncResult(callback, state)
            {
                WebRequest = request, 
                Context = context,
                Request = serviceRequest
            };

            BeginSetRequestContent(request, serviceRequest,
                                   () => request.BeginGetResponse(OnGetResponseCompleted, asyncResult), Configuration,
                                   asyncResult);

            return asyncResult;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
                                                         Justification = "Catch the exception and set it to async result.")]
        private void OnGetResponseCompleted(IAsyncResult ar)
        {
            var asyncResult = ar.AsyncState as HttpAsyncResult;
            Debug.Assert(asyncResult != null && asyncResult.WebRequest != null);

            try
            {
                var response = asyncResult.WebRequest.EndGetResponse(ar) as HttpWebResponse;
                ServiceResponse res = new ResponseImpl(response);
                HandleResponse(res, asyncResult.Context.ResponseHandlers);
                asyncResult.Complete(res);
            }
            catch (WebException we)
            {
                try
                {
                    var res = HandleException(we);
                    HandleResponse(res, asyncResult.Context.ResponseHandlers);
                    asyncResult.WebRequest.Abort();
                    asyncResult.Complete(res);
                }
                catch (Exception ie)
                {
                    asyncResult.WebRequest.Abort();
                    asyncResult.Complete(ie);
                }
            }
            catch (Exception oe)
            {
                asyncResult.WebRequest.Abort();
                asyncResult.Complete(oe);
            }
        }

        private static void SetRequestContent(HttpWebRequest webRequest, 
                                              ServiceRequest serviceRequest,
                                              ClientConfiguration clientConfiguration)
        {
            var data = serviceRequest.BuildRequestContent();

            if (data == null ||
                (serviceRequest.Method != HttpMethod.Put &&
                 serviceRequest.Method != HttpMethod.Post))
            {
                return;
            }

            // Write data to the request stream.
            long userSetContentLength = -1;
            if (serviceRequest.Headers.ContainsKey(HttpHeaders.ContentLength))
                userSetContentLength = long.Parse(serviceRequest.Headers[HttpHeaders.ContentLength]);
            
            if (serviceRequest.UseChunkedEncoding || !data.CanSeek) // when data cannot seek, we have to use chunked encoding as there's no way to set the length
            {
                webRequest.SendChunked = true;
                webRequest.AllowWriteStreamBuffering = false; // when using chunked encoding, the data is likely big and thus not use write buffer;
            }
            else
            {
                long streamLength = data.Length - data.Position;
                webRequest.ContentLength = (userSetContentLength >= 0 &&
                    userSetContentLength <= streamLength) ? userSetContentLength : streamLength;
                if (webRequest.ContentLength > clientConfiguration.DirectWriteStreamThreshold)
                {
                    webRequest.AllowWriteStreamBuffering = false;
                }
            }

            using (var requestStream = webRequest.GetRequestStream())
            {
                if (!webRequest.SendChunked)
                {
                    IoUtils.WriteTo(data, requestStream, webRequest.ContentLength);
                }
                else
                {
                    IoUtils.WriteTo(data, requestStream);
                }
            }
        }

        private static void BeginSetRequestContent(HttpWebRequest webRequest, ServiceRequest serviceRequest,
                                            OssAction asyncCallback, ClientConfiguration clientConfiguration, HttpAsyncResult result)
        {
            var data = serviceRequest.BuildRequestContent();

            if (data == null ||
                (serviceRequest.Method != HttpMethod.Put &&
                 serviceRequest.Method != HttpMethod.Post))
            {
                // Skip setting content body in this case.
                try
                {
                    asyncCallback();
                }
                catch(Exception e)
                {
                    result.WebRequest.Abort();
                    result.Complete(e);
                }

                return;
            }

            // Write data to the request stream.
            long userSetContentLength = -1;
            if (serviceRequest.Headers.ContainsKey(HttpHeaders.ContentLength))
                userSetContentLength = long.Parse(serviceRequest.Headers[HttpHeaders.ContentLength]);

            if (serviceRequest.UseChunkedEncoding || !data.CanSeek) // when data cannot seek, we have to use chunked encoding as there's no way to set the length
            {
                webRequest.SendChunked = true;
                webRequest.AllowWriteStreamBuffering = false; // when using chunked encoding, the data is likely big and thus not use write buffer;
            }
            else
            {
                long streamLength = data.Length - data.Position;
                webRequest.ContentLength = (userSetContentLength >= 0 &&
                    userSetContentLength <= streamLength) ? userSetContentLength : streamLength;
                if (webRequest.ContentLength > clientConfiguration.DirectWriteStreamThreshold)
                {
                    webRequest.AllowWriteStreamBuffering = false;
                }
            }

            webRequest.BeginGetRequestStream(
                    (ar) =>
                    {
                        try
                        {
                            using (var requestStream = webRequest.EndGetRequestStream(ar))
                            {
                                if (!webRequest.SendChunked)
                                {
                                    IoUtils.WriteTo(data, requestStream, webRequest.ContentLength);
                                }
                                else
                                {
                                    IoUtils.WriteTo(data, requestStream);
                                }
                            }
                            asyncCallback();
                        }
                        catch(Exception e)
                        {
                            result.WebRequest.Abort();
                            result.Complete(e);
                        }
                    }, null);
        }

        private static ServiceResponse HandleException(WebException ex)
        {
            var response = ex.Response as HttpWebResponse;
            if (response == null)
                throw ex;
            else
                return new ResponseImpl(ex);
        }

        #endregion

    }


    internal static class HttpFactory
    {

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        internal static HttpWebRequest CreateWebRequest(ServiceRequest serviceRequest, ClientConfiguration configuration)
        {
            var webRequest = WebRequest.Create(serviceRequest.BuildRequestUri()) as HttpWebRequest;

            SetRequestHeaders(webRequest, serviceRequest, configuration);
            SetRequestProxy(webRequest, configuration);

            if (webRequest.RequestUri.Scheme == "https")
            {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            }


            return webRequest;
        }

        // Set request headers
        private static void SetRequestHeaders(HttpWebRequest webRequest, ServiceRequest serviceRequest,
                                              ClientConfiguration configuration)
        {
            webRequest.Timeout = configuration.ConnectionTimeout;
            webRequest.ReadWriteTimeout = configuration.ConnectionTimeout;
            webRequest.Method = serviceRequest.Method.ToString().ToUpperInvariant();

            // Because it is not allowed to set common headers
            // with the WebHeaderCollection.Add method,
            // we have to call an internal method to skip validation.
            foreach (var h in serviceRequest.Headers)
            {
                // Nginx does not accept a chunked encoding request with Content-Length, as detailed in #OSS-2848
                if (h.Key.Equals(HttpHeaders.ContentLength) && (serviceRequest.UseChunkedEncoding || 
                    (serviceRequest.Content != null && !serviceRequest.Content.CanSeek) || serviceRequest.Content == null))
                {
                    continue;
                }

                HttpExtensions.AddInternal(webRequest.Headers, h.Key, h.Value);
            }

            // Set user-agent
            if (!string.IsNullOrEmpty(configuration.UserAgent))
                webRequest.UserAgent = configuration.UserAgent;
        }

        // Set proxy
        private static void SetRequestProxy(HttpWebRequest webRequest, ClientConfiguration configuration)
        {
            // Perf Improvement:
            // If HttpWebRequest.Proxy is not set to null explicitly,
            // it will try to load the IE proxy settings including auto proxy detection,
            // which is quite time consuming.
            webRequest.Proxy = null;
            

            // Set proxy if proxy settings are specified.
            if (!string.IsNullOrEmpty(configuration.ProxyHost))
            {
                if (configuration.ProxyPort < 0)
                    webRequest.Proxy = new WebProxy(configuration.ProxyHost);
                else
                    webRequest.Proxy = new WebProxy(configuration.ProxyHost, configuration.ProxyPort);

                if (!string.IsNullOrEmpty(configuration.ProxyUserName))
                {
                    webRequest.Proxy.Credentials = String.IsNullOrEmpty(configuration.ProxyDomain) ?
                        new NetworkCredential(configuration.ProxyUserName, configuration.ProxyPassword ?? string.Empty) :
                        new NetworkCredential(configuration.ProxyUserName, configuration.ProxyPassword ?? string.Empty,
                                              configuration.ProxyDomain);
                }
                webRequest.PreAuthenticate = true;
            }
        }

    }

    internal static class HttpExtensions
    {
        private static MethodInfo _addInternalMethod;
        private static readonly ICollection<PlatformID> MonoPlatforms = 
            new List<PlatformID> { PlatformID.MacOSX, PlatformID.Unix };
        private static bool? _isMonoPlatform;

        internal static void AddInternal(WebHeaderCollection headers, string key, string value)
        {
            if (_isMonoPlatform == null)
                _isMonoPlatform = MonoPlatforms.Contains(Environment.OSVersion.Platform);

            // HTTP headers should be encoded to iso-8859-1,
            // however it will be encoded automatically by HttpWebRequest in mono.
            if (_isMonoPlatform == false)
                // Encode headers for win platforms.
                value = HttpUtils.Reencode(value, HttpUtils.Utf8Charset, HttpUtils.Iso88591Charset);

            if (_addInternalMethod == null)
            {
                // Specify the internal method name for adding headers
                // mono: AddWithoutValidate
                // win: AddInternal
                var internalMethodName = (_isMonoPlatform == true) ? "AddWithoutValidate" : "AddInternal";

                var mi = typeof(WebHeaderCollection).GetMethod(
                    internalMethodName,
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(string), typeof(string) },
                    null);
                _addInternalMethod = mi;
            }

            _addInternalMethod.Invoke(headers, new object[] { key, value });
        }
    }
}
