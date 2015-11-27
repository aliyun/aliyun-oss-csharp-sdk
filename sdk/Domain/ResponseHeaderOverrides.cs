/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// 包含了在发送OSS GET请求时可以重载的返回请求头。
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
        /// 获取或设置重载的Content-Type返回请求头。如果未指定，则返回null。
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 获取或设置返回重载的Content-Language返回请求头。如果未指定，则返回null。
        /// </summary>
        public string ContentLanguage { get; set; }

        /// <summary>
        /// 获取或设置返回重载的Expires返回请求头。如果未指定，则返回null。
        /// </summary>
        public string Expires { get; set; }

        /// <summary>
        /// 获取或设置返回重载的Cache-Control返回请求头。如果未指定，则返回null。
        /// </summary>
        public string CacheControl { get; set; }

        /// <summary>
        /// 获取或设置返回重载的Content-Disposition返回请求头。如果未指定，则返回null。
        /// </summary>
        public string ContentDisposition { get; set; }

        /// <summary>
        /// 获取或设置返回重载的Content-Encoding返回请求头。如果未指定，则返回null。
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
