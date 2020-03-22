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
    /// The result class of the operation to list object versions.
    /// </summary>
    public class ObjectVersionList : GenericResult
    {
        private readonly IList<ObjectVersionSummary> _objectVersionSummaries = new List<ObjectVersionSummary>();
        private readonly IList<DeleteMarkerSummary> _deleteMarkerSummaries = new List<DeleteMarkerSummary>();
        private readonly IList<string> _commonPrefixes = new List<string>();

        /// <summary>
        /// Gets bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the next key maker value for the value of <see cref="P:ListObjectVersionsRequest.KeyMarker" /> in the next call.
        /// If the result is not truncated, this value is null.
        /// </summary>
        public string NextKeyMarker { get; internal set; }

        /// <summary>
        /// Gets the next version id maker value for the value of <see cref="P:ListObjectVersionsRequest.VersionIdMarker" /> in the next call.
        /// If the result is not truncated, this value is null.
        /// </summary>
        public string NextVersionIdMarker { get; internal set; }

        /// <summary>
        /// Flag of truncated result.
        /// True: the result is truncated (there's more data to list).
        /// False: no more data in server side to return.
        /// </summary>
        public bool IsTruncated { get; internal set; }

        /// <summary>
        /// The object key's marker. The value comes from <see cref="P:ListObjectVersionsRequest.KeyMarker" />.
        /// </summary>
        public string KeyMarker { get; internal set; }

        /// <summary>
        /// The version id's marker. The value comes from <see cref="P:ListObjectVersionsRequest.VersionIdMarker" />.
        /// </summary>
        public string VersionIdMarker { get; internal set; }

        /// <summary>
        /// The max keys to list. The value comes from <see cref="P:ListObjectVersionsRequest.MaxKeys" />.
        /// </summary>
        public int MaxKeys { get; internal set; }

        /// <summary>
        /// The object key's prefix. The value comes from <see cref="P:ListObjectVersionsRequest.Prefix" />.
        /// </summary>
        public string Prefix { get; internal set; }

        /// <summary>
        /// The delimiter for grouping object. The value comes from <see cref="P:ListObjectVersionsRequest.Delimiter" />.
        /// </summary>
        public string Delimiter { get; internal set; }

        /// <summary>
        /// The iterator of <see cref="ObjectVersionSummary" /> that meet the requirements in the ListObjectVersionsRequest.
        /// </summary>
        public IEnumerable<ObjectVersionSummary> ObjectVersionSummaries
        {
            get { return _objectVersionSummaries; }
        }

        /// <summary>
        /// The iterator of <see cref="DeleteMarkerSummary" /> that meet the requirements in the ListObjectVersionsRequest.
        /// </summary>
        public IEnumerable<DeleteMarkerSummary> DeleteMarkerSummaries
        {
            get { return _deleteMarkerSummaries; }
        }

        /// <summary>
        /// The common prefixes in the result. The objects returned do not include the objects under these common prefixes (folders).
        /// </summary>
        public IEnumerable<string> CommonPrefixes
        {
            get { return _commonPrefixes; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectVersionList" />.
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" /> name</param>
        internal ObjectVersionList(string bucketName)
        {
            BucketName = bucketName;
        }

        internal void AddObjectVersionSummary(ObjectVersionSummary summary)
        {
            _objectVersionSummaries.Add(summary);
        }

        internal void AddDeleteMarkerSummary(DeleteMarkerSummary summary)
        {
            _deleteMarkerSummaries.Add(summary);
        }

        internal void AddCommonPrefix(string prefix)
        {
            _commonPrefixes.Add(prefix);
        }
    }
}
