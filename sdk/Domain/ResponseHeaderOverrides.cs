/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// The class to contains the headers the caller hopes to get from the OSS response.
    /// </summary>
    public class ResponseHeaderOverrides
    {
        internal const string ResponseHeaderContentType = "response-content-type";
        internal const string ResponseHeaderContentLanguage = "response-content-language";
        internal const string ResponseHeaderExpires = "response-expires";
        internal const string ResponseCacheControl = "response-cache-control";
        internal const string ResponseContentDisposition = "response-content-disposition";
        internal const string ResponseContentEncoding = "response-content-encoding";

        /// <summary>
        /// Gets or sets content-type. If it's not specified, returns null.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets content-language.If it's not specified, returns null.
        /// </summary>
        public string ContentLanguage { get; set; }

        /// <summary>
        /// Gets or sets the expires header. If it's not specified, returns null.
        /// </summary>
        public string Expires { get; set; }

        /// <summary>
        /// Gets or sets the cache-control header.If it's not specified, returns null.
        /// </summary>
        public string CacheControl { get; set; }

        /// <summary>
        /// Gets or sets the Content-Disposition header.
        /// </summary>
        public string ContentDisposition { get; set; }

        /// <summary>
        /// Gets or sets the Content-Encoding header.
        /// </summary>
        public string ContentEncoding { get; set; }

        internal void Populate(IDictionary<string, string> parameters)
        {
            if (CacheControl != null)
                parameters.Add(ResponseCacheControl, CacheControl);
            
            if (ContentDisposition != null)
                parameters.Add(ResponseContentDisposition, ContentDisposition);
           
            if (ContentEncoding != null)
                parameters.Add(ResponseContentEncoding, ContentEncoding);
            
            if (ContentLanguage != null)
                parameters.Add(ResponseHeaderContentLanguage, ContentLanguage);
            
            if (ContentType != null)
                parameters.Add(ResponseHeaderContentType, ContentType);
            
            if (Expires != null)
                parameters.Add(ResponseHeaderExpires, Expires);
        }
    }
}
