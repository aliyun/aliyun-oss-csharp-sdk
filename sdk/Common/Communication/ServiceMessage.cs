/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace Aliyun.OSS.Common.Communication
{
    internal class ServiceMessage
    {
        private readonly IDictionary<String, String> _headers =
            new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the dictionary of HTTP headers.
        /// </summary>
        public virtual IDictionary<String, String> Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// Gets or sets the content stream.
        /// </summary>
        public virtual Stream Content { get; set; }
    }
}
