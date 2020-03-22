/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;

using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Internal;

namespace Aliyun.OSS
{
    public class DownloadObjectRequest
    {
        private readonly IList<string> _matchingETagConstraints = new List<string>();
        private readonly IList<string> _nonmatchingEtagConstraints = new List<string>();
        private readonly ResponseHeaderOverrides _responseHeaders = new ResponseHeaderOverrides();
        private int _parallelThreadCount = 3;

        public DownloadObjectRequest(string bucketName, string key, string downloadFile)
        {
            BucketName = bucketName;
            Key = key;
            DownloadFile = downloadFile;
        }

        public DownloadObjectRequest(string bucketName, string key, string downloadFile, string checkpointDir) : this(bucketName, key, downloadFile)
        {
            CheckpointDir = checkpointDir;
        }

        /// <summary>
        /// Gets or sets the name of the bucket.
        /// </summary>
        /// <value>The name of the bucket.</value>
        public string BucketName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the download file.
        /// </summary>
        /// <value>The download file.</value>
        public string DownloadFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the part.
        /// </summary>
        /// <value>The size of the part.</value>
        public long? PartSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parallel thread count.
        /// </summary>
        /// <value>The parallel thread count.</value>
        public int ParallelThreadCount
        {
            get
            {
                return _parallelThreadCount;
            }
            set
            {
                _parallelThreadCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the checkpoint file.
        /// </summary>
        /// <value>The checkpoint file.</value>
        public string CheckpointDir
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets "If-Unmodified-Since" parameter
        /// </summary>
        /// <remarks>
        /// It means if its value is same or later than the actual last modified time, the file will be downloaded. 
        /// Otherwise, return precondition failed (412).
        /// </remarks>
        public DateTime? UnmodifiedSinceConstraint { get; set; }

        /// <summary>
        /// Gets or sets "If-Modified-Since".
        /// </summary>
        /// <remarks>
        /// It means if its value is smaller the actual last modified time, the file will be downloaded. 
        /// Otherwise, return precondition failed (412).
        /// </remarks>
        public DateTime? ModifiedSinceConstraint { get; set; }

        /// <summary>
        /// Gets or sets the stream transfer progress.
        /// </summary>
        /// <value>The stream transfer progress.</value>
        public EventHandler<StreamTransferProgressArgs> StreamTransferProgress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the ETag matching constraint list. If the actual ETag matches any one in the constraint list, the file will be downloaded.
        /// Otherwise, returns precondition failed.
        /// The corresponding http header is "If-Match".
        /// </summary>
        public IList<string> MatchingETagConstraints
        {
            get { return _matchingETagConstraints; }
        }

        /// <summary>
        /// Gets the ETag non-matching constraint list. If the actual ETag does not match any one in the constraint list, the file will be downloaded.
        /// Otherwise, returns precondition failed.
        /// The corresponding http header is "If-None-Match".
        /// </summary>
        public IList<string> NonmatchingETagConstraints
        {
            get { return _nonmatchingEtagConstraints; }
        }

        /// <summary>
        /// Gets the overrided response headers.
        /// </summary>
        public ResponseHeaderOverrides ResponseHeaders
        {
            get { return _responseHeaders; }
        }

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

        internal GetObjectRequest ToGetObjectRequest()
        {
            GetObjectRequest request = new GetObjectRequest(BucketName, Key);

            foreach(var etagCondition in MatchingETagConstraints)
            {
                request.MatchingETagConstraints.Add(etagCondition);
            }
             
            foreach(var etagCondition in NonmatchingETagConstraints)
            {
                request.NonmatchingETagConstraints.Add(etagCondition);
            }

            request.ModifiedSinceConstraint = ModifiedSinceConstraint;
            request.UnmodifiedSinceConstraint = UnmodifiedSinceConstraint;

            request.RequestPayer = RequestPayer;

            request.TrafficLimit = TrafficLimit;

            request.VersionId = VersionId;

            return request;
        }
        
        /// <summary>
        /// Populate the http headers according to the properties of this object.
        /// </summary>
        /// <param name="headers">The generated http headers</param>
        internal void Populate(IDictionary<string, string> headers)
        {
            if (ModifiedSinceConstraint != null)
            {
                headers.Add(OssHeaders.GetObjectIfModifiedSince,
                            DateUtils.FormatRfc822Date(ModifiedSinceConstraint.Value));
            }
            if (UnmodifiedSinceConstraint != null)
            {
                headers.Add(OssHeaders.GetObjectIfUnmodifiedSince,
                            DateUtils.FormatRfc822Date(UnmodifiedSinceConstraint.Value));
            }
            if (_matchingETagConstraints.Count > 0)
            {
                headers.Add(OssHeaders.GetObjectIfMatch,
                    OssUtils.JoinETag(_matchingETagConstraints));
            }
            if (_nonmatchingEtagConstraints.Count > 0)
            {
                headers.Add(OssHeaders.GetObjectIfNoneMatch,
                    OssUtils.JoinETag(_nonmatchingEtagConstraints));
            }
            if (RequestPayer == RequestPayer.Requester)
            {
                headers.Add(OssHeaders.OssRequestPayer,
                    RequestPayer.Requester.ToString().ToLowerInvariant());
            }
        }
    }
}
