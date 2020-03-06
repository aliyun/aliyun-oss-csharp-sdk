/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to list objects' summary(<see cref="OssObjectSummary" />)
    /// </summary>
    public class ListObjectsRequest
    {
        private string _prefix;
        private string _marker;
        private Int32? _maxKeys;
        private string _delimiter;
        private string _encodingType;

        /// <summary>
        /// Gets or sets bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets or sets the object name prefix. The names of the returned object must be prefixed by this value.
        /// It's optional. If it's not set, then there's no requirement on the object name.
        /// </summary>
        public string Prefix
        {
            get { return _prefix; }
            set
            {
                if (value != null && value.Length > OssUtils.MaxPrefixStringSize)
                    throw new ArgumentException("parameter 'prefix' exceeds max size limit.");
                _prefix = value;
            }
        }

        /// <summary>
        /// Gets or sets the marker value. The name of returned objects must be greater than this value in lexicographic order.
        /// </summary>
        public string Marker
        {
            get { return _marker; }
            set
            {
                if (value != null && value.Length > OssUtils.MaxMarkerStringSize)
                    throw new ArgumentException("parameter 'marker' exceeds max size limit.");
                _marker = value;
            }
        }

        /// <summary>
        /// Gets or sets the max entries to return.
        /// By default it's 100.
        /// </summary>
        public Int32? MaxKeys
        {
            get { return _maxKeys.HasValue ? _maxKeys.Value : 100; }
            set
            {
                if (value > OssUtils.MaxReturnedKeys)
                    throw new ArgumentException("parameter 'maxkeys' exceed max limit.");
                _maxKeys = value;
            }
        }

        /// <summary>
        /// Gets or sets the delimiter for grouping the returned objects based on their keys.
        /// </summary>
        public string Delimiter
        {
            get { return _delimiter; }
            set
            {
                if (value != null && value.Length > OssUtils.MaxDelimiterStringSize)
                    throw new ArgumentException("parameter 'delimiter' exceeds max size limit.");
                _delimiter = value;
            }
        }

        /// <summary>
        /// Gets or sets encoding-type.
        /// </summary>
        public string EncodingType
        {
            get
            {
                return this._encodingType != null ? this._encodingType : HttpUtils.UrlEncodingType;
            }
            set
            {
                this._encodingType = value;
            }
        }

        /// <summary>
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; } 

        /// <summary>
        /// Creates an instance of <see cref="ListObjectsRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public ListObjectsRequest(string bucketName)
        {
            BucketName = bucketName;
        }
    }
}
