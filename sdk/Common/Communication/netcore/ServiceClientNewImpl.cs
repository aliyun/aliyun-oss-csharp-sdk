/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Aliyun.OSS.Util;
namespace Aliyun.OSS.Common.Communication
{
    /// <summary>
    /// The new implementation for ServiceClient under dotnet core
    /// </summary>
    internal class ServiceClientNewImpl : ServiceClient
    {
        internal class ResponseImpl : ServiceResponse
        {
            private bool _disposed;
            private HttpResponseMessage _response;
            private readonly Exception _failure;
            private IDictionary<string, string> _headers;
            private Stream _stream;
            private bool _disposeStream;

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
                    return _stream;
                }
            }

            public ResponseImpl(HttpResponseMessage httpWebResponse)
            {
                _response = httpWebResponse;
                try
                {
                    _stream = (_response.Content != null) ? _response.Content.ReadAsStreamAsync().Result : null;
                    if (!_response.IsSuccessStatusCode)
                    {
                        if (_response.Content != null) { // after the EnsureSuccessStatusCode(), the _strema will be closed if the status code is non-successful one.
                            _stream = new MemoryStream();
                            _response.Content.CopyToAsync(_stream).Wait();
                            _stream.Seek(0, SeekOrigin.Begin);
                            _disposeStream = true;
                        }
                    }
                    _response.EnsureSuccessStatusCode();
                }
                catch(HttpRequestException e)
                {
                    _failure = e;
                }
            }

            private static IDictionary<string, string> GetResponseHeaders(HttpResponseMessage response)
            {
                var headers = response.Headers;
                var result = new Dictionary<string, string>();

                foreach(var header in headers)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    string s = "";
                    foreach(var val in header.Value)
                    {
                        sb.Append(s);
                        sb.Append(val);
                        s = "\r\n";
                    }
                    
                    result.Add(header.Key, sb.ToString());
                }

                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var header in response.Content.Headers)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        string s = "";
                        foreach (var val in header.Value)
                        {
                            sb.Append(s);
                            sb.Append(val);
                            s = "\r\n";
                        }

                        result.Add(header.Key, sb.ToString());
                    }
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
                        _response.Dispose();
                        _response = null;
                    }
                    if (_disposeStream)
                    {
                        _stream.Dispose();
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

        public ServiceClientNewImpl(ClientConfiguration configuration) : base(configuration)
        {
        }

        protected override ServiceResponse SendCore(ServiceRequest request, ExecutionContext context)
        { 
            var req = new HttpRequestMessage(Convert(request.Method), request.BuildRequestUri());
            this.SetRequestContent(req, request);
            this.SetHeaders(req, request);
            HttpClient client = GetClient();
            HttpResponseMessage resp = client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).Result;
            return new ResponseImpl(resp);
        }

        private void SetRequestContent(HttpRequestMessage requestMsg,
                                             ServiceRequest serviceRequest)
        {
            var data = serviceRequest.BuildRequestContent();

            if (data == null ||
                (serviceRequest.Method != HttpMethod.Put &&
                 serviceRequest.Method != HttpMethod.Post))
            {
                requestMsg.Content = new System.Net.Http.ByteArrayContent(new byte[0]);
                return;
            }

            // Write data to the request stream.
            requestMsg.Content = new StreamContent(new StreamWeakReferece(data));
        }

        void SetHeaders(HttpRequestMessage req, ServiceRequest request)
        {
            if (req.Content != null)
            {
                if (request.Headers.ContainsKey(HttpHeaders.ContentDisposition) && !string.IsNullOrEmpty(request.Headers[HttpHeaders.ContentDisposition]))
                {
                    req.Content.Headers.ContentDisposition = System.Net.Http.Headers.ContentDispositionHeaderValue.Parse(request.Headers[HttpHeaders.ContentDisposition]);
                }

                if (request.Headers.ContainsKey(HttpHeaders.ContentEncoding) &&!string.IsNullOrEmpty(request.Headers[HttpHeaders.ContentEncoding]))
                {
                    req.Content.Headers.ContentEncoding.Add(request.Headers[HttpHeaders.ContentEncoding]);
                }

                if (request.Headers.ContainsKey(HttpHeaders.ContentType) && !string.IsNullOrEmpty(request.Headers[HttpHeaders.ContentType]))
                {
                    req.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(request.Headers[HttpHeaders.ContentType]);
                }
                if (request.Headers.ContainsKey(HttpHeaders.ContentMd5) && !string.IsNullOrEmpty(request.Headers[HttpHeaders.ContentMd5]))
                {
                    req.Content.Headers.ContentMD5 = System.Convert.FromBase64String(request.Headers[HttpHeaders.ContentMd5]);
                }
            }
            
            foreach (var item in request.Headers)
            {
                if (!item.Key.StartsWith("Content"))
                {
                    req.Headers.TryAddWithoutValidation(item.Key, item.Value);
                }
            }
        }

        void SetProxy(HttpClientHandler httpClientHandler)
        {
            if (String.IsNullOrEmpty(Configuration.ProxyHost))
            {
                httpClientHandler.Proxy = null;
            }
            else
            {
                httpClientHandler.UseProxy = true;
                if (Configuration.ProxyPort < 0)
                    httpClientHandler.Proxy = new WebProxy(Configuration.ProxyHost);
                else
                    httpClientHandler.Proxy = new WebProxy(Configuration.ProxyHost, Configuration.ProxyPort);

                if (!string.IsNullOrEmpty(Configuration.ProxyUserName))
                {
                    httpClientHandler.Proxy.Credentials = String.IsNullOrEmpty(Configuration.ProxyDomain) ?
                        new NetworkCredential(Configuration.ProxyUserName, Configuration.ProxyPassword ?? string.Empty) :
                        new NetworkCredential(Configuration.ProxyUserName, Configuration.ProxyPassword ?? string.Empty,
                                              Configuration.ProxyDomain);
                    httpClientHandler.PreAuthenticate = true;
                }
            }
        }

        protected override IAsyncResult BeginSendCore(ServiceRequest request, ExecutionContext context,
            AsyncCallback callback, Object state)
        {
            var req = new HttpRequestMessage(Convert(request.Method), request.BuildRequestUri());
            this.SetRequestContent(req, request);
            this.SetHeaders(req, request);
            HttpClient client = GetClient();
            var task = client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
            ServiceClientImpl.HttpAsyncResult result = new ServiceClientImpl.HttpAsyncResult(callback, state);
            task.ContinueWith((resp) =>
            {
                ServiceResponse serviceResponse = new ResponseImpl(resp.Result);
                result.Complete(serviceResponse);
            });

            return result;
        }

        private static System.Net.Http.HttpMethod Convert(HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.Delete:
                    return System.Net.Http.HttpMethod.Delete;
                case HttpMethod.Get:
                    return System.Net.Http.HttpMethod.Get;
                case HttpMethod.Head:
                    return System.Net.Http.HttpMethod.Head;
                case HttpMethod.Options:
                    return System.Net.Http.HttpMethod.Options;
                case HttpMethod.Post:
                    return System.Net.Http.HttpMethod.Post;
                case HttpMethod.Put:
                    return System.Net.Http.HttpMethod.Put;
                default:
                    throw new InvalidCastException();
            }
        }

        private HttpClient GetClient()
        {
            if (_httpClientSelf != null)
            {
                return _httpClientSelf;
            }

            bool notUseProxy = string.IsNullOrEmpty(Configuration.ProxyHost);

            if (notUseProxy)
            {
                if (_httpClientNoProxy == null)
                {
                    lock (_clientLock)
                    {
                        if (_httpClientNoProxy == null)
                        {
                            _configurationNoProxy = Configuration.Clone() as ClientConfiguration;
                            _httpClientNoProxy = Create(false);
                        }
                    }
                } 
            }
            else
            {
                if (_httpClient == null)
                {
                    lock (_clientLock)
                    {
                        if (_httpClient == null)
                        {
                            _configuration = Configuration.Clone() as ClientConfiguration;
                            _httpClient = Create(true);
                        }
                    }
                } 
            }

            if (notUseProxy)
            {
                if (CanReuseHttpClient(_configurationNoProxy, Configuration))
                {
                    _httpClientSelf = _httpClientNoProxy;
                }
            }
            else
            {
                if (CanReuseHttpClient(_configuration, Configuration))
                {
                    _httpClientSelf = _httpClient;
                }
            }

            if (_httpClientSelf == null)
            {
                lock (_clientLock)
                {
                    if (_httpClientSelf == null)
                    {
                        _httpClientSelf = Create(!notUseProxy);
                    }
                }
            }
            return _httpClientSelf;
        }

        private HttpClient Create(bool setProxy)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(httpClientHandler);
            if (setProxy)
            {
                this.SetProxy(httpClientHandler);
            }

            client.Timeout = Configuration.ConnectionTimeout < 0 ? TimeSpan.FromDays(1) : TimeSpan.FromMilliseconds(Configuration.ConnectionTimeout);
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Configuration.UserAgent);
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(HttpFactory.CheckValidationResult);
            return client;
        }

        private bool CanReuseHttpClient(ClientConfiguration dst, ClientConfiguration src)
        {
            if (dst == null 
                || src == null
                || (dst.ConnectionTimeout == src.ConnectionTimeout && 
                    dst.ProxyHost == src.ProxyHost &&
                    dst.ProxyPort == src.ProxyPort &&
                    dst.ProxyUserName == src.ProxyUserName &&
                    dst.ProxyPassword == src.ProxyPassword
                ))
            {
                return true;
            }
            return false;
        }

        private static HttpClient _httpClient;
        private static HttpClient _httpClientNoProxy;
        private static object _clientLock = new object();
        private static ClientConfiguration _configuration;
        private static ClientConfiguration _configurationNoProxy;
        private HttpClient _httpClientSelf;

    }
}
