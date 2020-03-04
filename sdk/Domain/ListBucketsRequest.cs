/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to list <see cref="Bucket" /> of the current account.
    /// </summary>
    public class ListBucketsRequest
    {
        private Tag _tag;

        /// <summary>
        /// Gets or sets the bucket name prefix to list (optional)
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the marker of the bucket name. The buckets to return whose names are greater than this value in lexicographic order.
        /// </summary>
        public string Marker { get; set; }

        /// <summary>
        /// Gets or sets the max entries to return. By default is 100.
        /// </summary>
        public int? MaxKeys { get; set; }

        /// <summary>
        /// Gets or sets the bucket tag
        /// </summary>
        public Tag Tag { get => _tag; set => _tag = value; }
    }
}
