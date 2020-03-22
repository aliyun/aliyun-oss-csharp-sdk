/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
using System;
using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to upload the source object as a part of the target object.
    /// </summary>
    public class UploadPartCopyRequest
    {
        private readonly IList<string> _matchingETagConstraints = new List<string>();
        private readonly IList<string> _nonmatchingETagConstraints = new List<string>();

        /// <summary>
        /// Gets the target bucket name
        /// </summary>
        public string TargetBucket { get; private set; }
        
        /// <summary>
        /// Gets the target key
        /// </summary>
        public string TargetKey { get; private set; }
        
        /// <summary>
        /// Gets the upload Id.
        /// </summary>
        public string UploadId { get; private set; }
        
        /// <summary>
        /// Gets or sets the part number.
        /// Every part upload will have a part number (from 1 to 10000).
        /// For a given upload id, the part number is the Id of the part and determine the position of the part in the whole file.
        /// If the same part number is uploaded with new data, the original data will be overwritten.
        /// </summary>
        public int? PartNumber { get; set; }
        
        /// <summary>
        /// Gets or sets the part size
        /// Except the last part, other parts' size should be at least 5MB.
        /// </summary>
        public long? PartSize { get; set; }
        
        /// <summary>
        /// Gets or sets the MD5 checksum for the part's data.
        /// </summary>
        public string Md5Digest { get; set; }

        /// <summary>
        /// Gets or sets the source object key.
        /// </summary>
        public string SourceKey { get; set; }

        /// <summary>
        /// Gets or sets the source bucket
        /// </summary>
        public string SourceBucket { get; set; }

        /// <summary>
        /// Gets or sets the beginning index of the source object to copy from.
        /// </summary>
        public long? BeginIndex { get; set; }

        /// <summary>
        /// Gets the constraints of matching ETag. If the source object's ETag matches any one in the list, the copy will be proceeded.
        /// Otherwise returns error code 412 (precondition failed).
        /// </summary>        
        public IList<string> MatchingETagConstraints
        {
            get { return _matchingETagConstraints; }
        }

        /// <summary>
        /// Gets the constraints of non-matching ETag. If the source object's ETag does not match any one in the list, the copy will be proceeded.
        /// Otherwise returns error code 412 (precondition failed).
        /// </summary>       
        public IList<string> NonmatchingETagConstraints
        {
            get { return _nonmatchingETagConstraints; }
        }

        /// <summary>
        /// Gets or sets the constraints of unmodified timestamp threshold. If the value is same or greater than the actual last modified time, proceed the copy.
        /// Otherwise returns 412 (precondition failed).
        /// </summary>
        public DateTime? UnmodifiedSinceConstraint { get; set; }

        /// <summary>
        /// Gets or sets the constraints of modified timestamp threshold. If the value is smaller than the actual last modified time,  proceed the copy.
        /// Otherwise returns 412 (precondition failed).
        /// </summary>   
        public DateTime? ModifiedSinceConstraint { get; set; }

        /// <summary>
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; }

        /// <summary>
        /// Gets or sets the traffic limit, the unit is bit/s
        /// </summary>
        public long TrafficLimit { get; set; }

        /// <summary>
        /// Gets or sets the version id
        /// </summary>
        public string VersionId { get; set; }

        public UploadPartCopyRequest(string targetBucket, string targetKey, string sourceBucket, 
            string sourceKey, string uploadId)
        {
            TargetBucket = targetBucket;
            TargetKey = targetKey;
            SourceBucket = sourceBucket;
            SourceKey = sourceKey;
            UploadId = uploadId;
        }
    }
}
