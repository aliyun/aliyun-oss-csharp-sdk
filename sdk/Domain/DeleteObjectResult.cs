/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class for delete object operation.
    /// </summary>
    public class DeleteObjectResult : GenericResult
    {
        /// <summary>
        /// Gets or sets the version id.
        /// </summary>
        public string VersionId { get; internal set; }

        /// <summary>
        /// Gets or sets the delete marker.
        /// </summary>
        public bool DeleteMarker { get; internal set; }

        internal DeleteObjectResult()
        { }
    }
}
