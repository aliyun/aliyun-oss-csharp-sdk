/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class for getting object from OSS.
    /// </summary>
    public class GetObjectRequest
    {
        private readonly IList<string> _matchingETagConstraints = new List<string>();
        private readonly IList<string> _nonmatchingEtagConstraints = new List<string>();
        private readonly ResponseHeaderOverrides _responseHeaders = new ResponseHeaderOverrides();

        /// <summary>
        /// Gets or sets <see cref="Bucket" /> name.
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="OssObject" /> key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets <see cref="OssObject" /> range to read
        /// </summary>
        /// <remarks>
        /// Calls <see cref="SetRange" /> to set. If it's not set, returns null.
        /// </remarks>
        public long[] Range { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="OssObject" />'s process method (such as resize, sharpen, etc)
        /// </summary>
        public string Process { get; set; }

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
        /// Gets or sets the progress callback
        /// </summary>
        public EventHandler<StreamTransferProgressArgs> StreamTransferProgress { get; set; }

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

        /// <summary>
        /// Creates a new instance of <see cref="GetObjectRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        public GetObjectRequest(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
        }

        /// <summary>
        /// Creates a new instance of <see cref="GetObjectRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key name</param>
        /// <param name="process">The process method for image file in OSS</param>
        public GetObjectRequest(string bucketName, string key, string process)
        {
            BucketName = bucketName;
            Key = key;
            Process = process;
        }

        /// <summary>
        /// Sets the read range of the target object (optional).
        /// It follows the HTTP header "Range"'s semantic 
        /// </summary>
        /// <param name="start">
        /// The start value in the range.
        /// <para>
        /// If the value is non-negative, it means the start index of the object to read. 
        /// If the value is -1, it means the start index is determined by end parameter and thus the end parameter must not be -1.
        /// For example, if the end is 100, then the start is bytes=-100 (bytes is the total length of the object). It means to read the last 100 bytes of the object.
        /// </para>
        /// </param>
        /// <param name="end">
        /// The end value of the range. And it must be smaller than the total length of the object.
        /// <para>
        /// If the value is non-negative, it means the end index of the object to read.
        /// If the value is -1, it means the end is the object's last byte and start must not be -1.
        /// For example, if the start is 99 and end is -1, it means to read the whole object except the first 99 bytes.
        /// </para>
        /// </param>
        public void SetRange(long start, long end)
        {
            Range = new[] { start, end };
        }

        /// <summary>
        /// Populate the http headers according to the properties of this object.
        /// </summary>
        /// <param name="headers">The generated http headers</param>
        internal void Populate(IDictionary<string, string> headers)
        {
            if (Range != null && (Range[0] >= 0 || Range[1] >= 0))
            {
                var rangeHeaderValue = new StringBuilder();
                rangeHeaderValue.Append("bytes=");
                if (Range[0] >= 0)
                    rangeHeaderValue.Append(Range[0].ToString(CultureInfo.InvariantCulture));
                rangeHeaderValue.Append("-");
                if (Range[1] >= 0)
                    rangeHeaderValue.Append(Range[1].ToString(CultureInfo.InvariantCulture));

                headers.Add(HttpHeaders.Range, rangeHeaderValue.ToString());
            }
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
                headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
            }
            if (TrafficLimit > 0)
            {
                headers.Add(OssHeaders.OssTrafficLimit, TrafficLimit.ToString());
            }
        }
    }
}
