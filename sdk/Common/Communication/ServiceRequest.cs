/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Communication
{
    internal class ServiceRequest : ServiceMessage, IDisposable
    {
        private bool _disposed;
        
        private readonly IDictionary<String, String> _parameters 
            = new Dictionary<String, String>();

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        public Uri Endpoint { get; set; }
        
        /// <summary>
        /// Gets or sets the resource path of the request URI.
        /// </summary>
        public String ResourcePath { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        public HttpMethod Method { get; set; }
        
        /// <summary>
        /// Gets the dictionary of the request parameters.
        /// </summary>
        public IDictionary<String, String> Parameters
        {
            get { return _parameters; }
        }
        
        /// <summary>
        /// Gets whether the request can be repeated.
        /// </summary>
        public bool IsRepeatable
        {
            get { return Content == null || Content.CanSeek; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Aliyun.OSS.Common.Communication.ServiceRequest"/>
        /// use chunked encoding.
        /// </summary>
        /// <value><c>true</c> if use chunked encoding; otherwise, <c>false</c>.</value>
        public bool UseChunkedEncoding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Aliyun.OSS.Common.Communication.ServiceRequest"/>
        /// parameters in URL.
        /// </summary>
        /// <value><c>true</c> if parameters in URL; otherwise, <c>false</c>.</value>
        public bool ParametersInUri
        {
            get;
            set;
        }

        /// <summary>
        /// Build the request URI from the request message.
        /// </summary>
        /// <returns></returns>
        public string BuildRequestUri()
        {
            const string delimiter = "/";
            var uri = Endpoint.ToString();
            if (!uri.EndsWith(delimiter) && 
                (ResourcePath == null || !ResourcePath.StartsWith(delimiter))) 
            {
                uri += delimiter;
            }

            if (ResourcePath != null)
                uri += ResourcePath;
            
            if (IsParameterInUri())
            {
                var paramString = HttpUtils.ConbineQueryString(_parameters);
                if (!string.IsNullOrEmpty(paramString))
                    uri += "?" + paramString;
            }
            
            return uri;
        }
        
        public Stream BuildRequestContent()
        {
            if (!IsParameterInUri())
            {
                var paramString = HttpUtils.ConbineQueryString(_parameters);
                if (!string.IsNullOrEmpty(paramString))
                {
                    var buffer = Encoding.GetEncoding("utf-8").GetBytes(paramString);
                    Stream content = new MemoryStream();
                    content.Write(buffer, 0, buffer.Length);
                    content.Flush();
                    // Move the marker to the beginning for further read.
                    content.Seek(0, SeekOrigin.Begin);
                    return content;
                }
            }

            return Content;
        }
        
        private bool IsParameterInUri()
        {
            var requestHasPayload = Content != null;
            var requestIsPost = Method == HttpMethod.Post;
            var putParamsInUri = !requestIsPost || requestHasPayload || ParametersInUri;
            return putParamsInUri;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            
            if (disposing)
            {
                if (Content != null)
                {
                    Content.Close();
                    Content = null;
                }
                _disposed = true;
            }   
        }
    }
}
