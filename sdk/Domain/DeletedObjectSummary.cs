/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Globalization;

namespace Aliyun.OSS
{
    /// <summary>
    /// a deleted object summary information.
    /// </summary>
    public class DeletedObjectSummary
    {
        /// <summary>
        /// Gets or sets the object key.
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// Gets or sets the version of a object.
        /// </summary>
        public string VersionId { get; internal set; }

        /// <summary>
        /// Gets or sets if it is a delete marker of a object.
        /// </summary>
        public bool DeleteMarker { get; internal set; }

        /// <summary>
        /// Gets or sets the version of a delete marker.
        /// </summary>
        public string DeleteMarkerVersionId { get; internal set; }
    }
}
