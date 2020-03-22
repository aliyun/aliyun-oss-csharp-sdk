/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// Result class for the copy object operation.
    /// </summary>
    public class CopyObjectResult : GenericResult
    {
        /// <summary>
        /// Last modified timestamp getter/setter
        /// </summary>
        public DateTime LastModified { get; internal set; }
        
        /// <summary>
        /// New object's ETag
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// Gets or sets the version id.
        /// </summary>
        public string VersionId { get; internal set; }

        /// <summary>
        /// Gets or sets the copy source version id.
        /// </summary>
        public string CopySourceVersionId { get; internal set; }

        internal CopyObjectResult()
        { }
    }
}
