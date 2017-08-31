/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to list objects.
    /// </summary>
    public class ObjectListing : GenericResult
    {
        private readonly IList<OssObjectSummary> _objectSummaries = new List<OssObjectSummary>();

        private readonly IList<string> _commonPrefixes = new List<string>();

        /// <summary>
        /// Gets bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the next maker value for the value of <see cref="P:ListObjectRequest.Marker" /> in the next call.
        /// If the result is not truncated, this value is null.
        /// </summary>
        public string NextMarker { get; internal set; }

        /// <summary>
        /// Obsolete property.
        /// </summary>
        [Obsolete("misspelled, please use IsTruncated instead")]
        public bool IsTrunked
        {
            get { return IsTruncated; }
            internal set { IsTruncated = value; }
        }
        
        /// <summary>
        /// Flag of truncated result.
        /// True: the result is truncated (there's more data to list).
        /// False: no more data in server side to return.
        /// </summary>
        public bool IsTruncated { get; internal set; }

        /// <summary>
        /// The object key's marker. The value comes from <see cref="P:ListObjectRequest.Marker" />.
        /// </summary>
        public string Marker { get; internal set; }

        /// <summary>
        /// The max keys to list. The value comes from <see cref="P:ListObjectRequest.MaxKeys" />.
        /// </summary>
        public int MaxKeys { get; internal set; }

        /// <summary>
        /// The object key's prefix. The value comes from <see cref="P:ListObjectRequest.Prefix" />.
        /// </summary>
        public string Prefix { get; internal set; }

        /// <summary>
        /// The delimiter for grouping object. The value comes from <see cref="P:ListObjectRequest.Delimiter" />.
        /// </summary>
        public string Delimiter { get; internal set; }

        /// <summary>
        /// The iterator of <see cref="OssObjectSummary" /> that meet the requirements in the ListOjectRequest.
        /// </summary>
        public IEnumerable<OssObjectSummary> ObjectSummaries
        {
            get { return _objectSummaries; }
        }

        /// <summary>
        /// The common prefixes in the result. The objects returned do not include the objects under these common prefixes (folders).
        /// </summary>
        public IEnumerable<string> CommonPrefixes
        {
            get { return _commonPrefixes; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectListing" />.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        internal ObjectListing(string bucketName)
        {
            BucketName = bucketName;
        }
        
        internal void AddObjectSummary(OssObjectSummary summary)
        {
            _objectSummaries.Add(summary);
        }
        
        internal void AddCommonPrefix(string prefix)
        {
            _commonPrefixes.Add(prefix);
        }
    }
}
