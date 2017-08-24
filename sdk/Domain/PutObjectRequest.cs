/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Internal;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to put an object to OSS.
    /// </summary>
    public class PutObjectRequest
    {
        private Stream _inputStream;

        /// <summary>
        /// Gets or sets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets or sets the object key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets object content stream
        /// </summary>
        public Stream Content
        {
            get { return this._inputStream; }
            set { this._inputStream = value; }
        }

        /// <summary>
        /// Gets or sets the transfer progress callback
        /// </summary>
        public EventHandler<StreamTransferProgressArgs> StreamTransferProgress { get; set; }

        /// <summary>
        /// Gets or sets the object metadata.
        /// </summary>
        public ObjectMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the process method.The result will be in <see cref="P:PutObjectResult.ResponseStream" />.
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="PutObjectRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        /// <param name="content">content to upload</param>
        public PutObjectRequest(string bucketName, string key, Stream content) 
            : this(bucketName, key, content, null) { }

        /// <summary>
        /// Creates a new instance of <see cref="PutObjectRequest" />
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        /// <param name="content">content to upload</param>
        /// <param name="metadata">metadata to set</param>
        public PutObjectRequest(string bucketName, string key, Stream content, ObjectMetadata metadata)
        {
            BucketName = bucketName;
            Key = key;
            Content = content;
            Metadata = metadata;
        }

        internal void Populate(IDictionary<string, string> headers)
        {
            if (Metadata != null) 
            {
                Metadata.Populate(headers);
            }
        }

        /// <summary>
        /// Returns true if the request has the Process property or has the callback in metadata.
        /// </summary>
        internal bool IsNeedResponseStream()
        {
            if ((Process != null) || 
                (Metadata != null && Metadata.HttpMetadata.ContainsKey(HttpHeaders.Callback)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the request has the callback in Metadata property.
        /// </summary>
        internal bool IsCallbackRequest()
        {
            if (Metadata != null && Metadata.HttpMetadata.ContainsKey(HttpHeaders.Callback))
            {
                return true;
            }
            return false;
        }
    }
}
