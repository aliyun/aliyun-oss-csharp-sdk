/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
#pragma warning disable 618, 3005

    /// <summary>
    /// The request class of the operation to copy an existing object to another one. The destination object could be a non-existing or existing object.
    /// </summary>
    public class CopyObjectRequest
    {
        private readonly IList<string> _matchingETagConstraints = new List<string>();
        private readonly IList<string> _nonmatchingETagConstraints = new List<string>();

        /// <summary>
        /// Source bucket name's getter/setter.
        /// </summary>
        public string SourceBucketName { get; set; }
        
        /// <summary>
        /// Source object key's getter/setter.
        /// </summary>
        public string SourceKey { get; set; }
        
        /// <summary>
        /// Destination bucket name's getter/setter.
        /// </summary>
        public string DestinationBucketName { get; set; }
        
        /// <summary>
        /// Destination object key's getter/setter.
        /// </summary>
        public string DestinationKey { get; set; } 

        /// <summary>
        /// Destination object's metadata getter/setter
        /// </summary>
        public ObjectMetadata NewObjectMetadata { get; set; }

        /// <summary>
        /// ETag maching contraints---that is for the copy operation to execute, the source object's ETag must match one of the ETags in this property. 
        /// If not, return 412 as HTTP code (precondition failed)
        /// </summary>        
        public IList<string> MatchingETagConstraints
        {
            get { return _matchingETagConstraints; }
        }

        /// <summary>
        /// ETag non-matching contraints---that is for the copy operation to execute, the source object's ETag must not match any of the ETags in this property. 
        /// If matches any, return 412 as HTTP code (precondition failed)
        /// </summary>       
        public IList<string> NonmatchingETagConstraints
        {
            get { return _nonmatchingETagConstraints; }
        }

        /// <summary>
        /// Unmodified timestamp threshold----that is for the copy operation to execute, the file's last modified time must be smaller than this property;
        /// Otherwise return 412 as HTTP code (precondition failed)
        /// </summary>
        public DateTime? UnmodifiedSinceConstraint { get; set; }

        /// <summary>
        /// Modified timestamp threshold----that is for the copy operation to execute, the file's last modified time must be same or greater than this property;
        /// Otherwise return 412 as HTTP code (precondition failed)
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
        /// Gets or sets the source key version id
        /// </summary>
        public string SourceVersionId { get; set; }

        /// <summary>
        /// Creates a new <see cref="CopyObjectRequest" /> instance
        /// </summary>
        /// <param name="sourceBucketName">source object's bucket name</param>
        /// <param name="sourceKey">source object key</param>
        /// <param name="destinationBucketName">destination object's bucket name</param>
        /// <param name="destinationKey">destination object key</param>
        public CopyObjectRequest(string sourceBucketName, string sourceKey,
            string destinationBucketName, string destinationKey)
        {
            OssUtils.CheckBucketName(destinationBucketName);
            OssUtils.CheckObjectKey(destinationKey);

            SourceBucketName = sourceBucketName;
            SourceKey = sourceKey;
            DestinationBucketName = destinationBucketName;
            DestinationKey = destinationKey;
        }
        
        internal void Populate(IDictionary<string, string> headers)
        {
            var copyHeaderValue = OssUtils.BuildCopyObjectSource(SourceBucketName, SourceKey);
            if (!string.IsNullOrEmpty(SourceVersionId))
            {
                copyHeaderValue = copyHeaderValue + "?versionId=" + SourceVersionId;
            }
            headers.Add(OssHeaders.CopyObjectSource, copyHeaderValue);

            if (ModifiedSinceConstraint != null)
            {
                headers.Add(OssHeaders.CopySourceIfModifedSince, 
                    DateUtils.FormatRfc822Date(ModifiedSinceConstraint.Value));
            }

            if (UnmodifiedSinceConstraint != null)
            {
                headers.Add(OssHeaders.CopySourceIfUnmodifiedSince, 
                    DateUtils.FormatRfc822Date(UnmodifiedSinceConstraint.Value));
            }

            if (_matchingETagConstraints.Count > 0)
            {
                headers.Add(OssHeaders.CopySourceIfMatch, OssUtils.JoinETag(_matchingETagConstraints));
            }

            if (_nonmatchingETagConstraints.Count > 0)
            {
                headers.Add(OssHeaders.CopySourceIfNoneMatch, 
                    OssUtils.JoinETag(_nonmatchingETagConstraints));
            }
            
            if (NewObjectMetadata != null)
            {
                headers.Add(OssHeaders.CopyObjectMetaDataDirective, "REPLACE");
                NewObjectMetadata.Populate(headers);
            }

            // Remove Content-Length header, ObjectMeta#Populate will create 
            // ContentLength header, but we do not need it for the request body is empty.
            headers.Remove(HttpHeaders.ContentLength);

            if (RequestPayer == RequestPayer.Requester)
            {
                headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
            }

            if (TrafficLimit > 0)
            {
                headers.Add(OssHeaders.OssTrafficLimit, TrafficLimit.ToString());
            }
        }
    }

#pragma warning restore 618, 3005
}
