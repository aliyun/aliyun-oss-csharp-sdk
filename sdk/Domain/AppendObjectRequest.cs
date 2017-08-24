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
#pragma warning disable 618, 3005

    /// <summary>
    /// The request which is used to append data into an object (existing or non-existing)
    /// </summary>
    public class AppendObjectRequest
    {
        private Stream _inputStream;

        /// <summary>
        /// Bucket name getter/setter.
        /// </summary>
        public string BucketName { get; set; }
        
        /// <summary>
        /// Object key getter/setter
        /// </summary>
        public string Key { get; set; }   

        /// <summary>
        /// Object metadata getter/setter
        /// </summary>
        public ObjectMetadata ObjectMetadata { get; set; }

        /// <summary>
        /// Position getter/setter. The position is the start index for the appending. 
        /// Initially it could be the length of the object (length could be got from the GetObjectmeta). Then it could be got from the previous result of AppendObjectRequest.
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// The content to append
        /// </summary>
        public Stream Content
        {
            get { return this._inputStream; }
            set { this._inputStream = value; }
        }

        /// <summary>
        /// Progress callback getter and setter
        /// </summary>
        public EventHandler<StreamTransferProgressArgs> StreamTransferProgress { get; set; }
        
        /// <summary>
        /// Creates a new instance of <see cref="AppendObjectRequest" />
        /// </summary>
        /// <param name="bucketName"> bucket name</param>
        /// <param name="key">object key</param>
        public AppendObjectRequest(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
        }
        
        internal void Populate(IDictionary<string, string> headers)
        {
            ObjectMetadata.Populate(headers);
        }
    }

#pragma warning restore 618, 3005
}
