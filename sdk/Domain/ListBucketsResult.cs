/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to list buckets.
    /// </summary>
    public class ListBucketsResult : GenericResult
    {
        /// <summary>
        /// Gets or sets the bucket name prefix(optional).
        /// </summary>
        public string Prefix { get; internal set; }

        /// <summary>
        /// Gets or sets the bucket name marker.Its value should be same as the ListBucketsRequest.Marker.
        /// </summary>
        public string Marker { get; internal set; }

        /// <summary>
        /// Gets or sets the max entries to return.
        /// By default it's 100.
        /// </summary>
        public int? MaxKeys { get; internal set; }

        /// <summary>
        /// Gets or sets the flag of truncated. If it's true, means not all buckets have been returned.
        /// </summary>
        public bool? IsTruncated { get; internal set; }

        /// <summary>
        /// Gets the next marker's value. Assign this value to the next call's ListBucketRequest.marker.
        /// </summary>
        public string NextMaker { get; internal set; }

        /// <summary>
        /// Gets the bucket iterator.
        /// </summary>
        public IEnumerable<Bucket> Buckets { get; internal set; }
    }
}
