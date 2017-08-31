/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operatoin to upload an object
    /// </summary>
    public class PutObjectResult : StreamResult
    {
        /// <summary>
        /// Gets or sets the Etag.
        /// </summary>
        public string ETag { get; internal set; }

        internal PutObjectResult()
        { }
    }
}
