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
    /// <summary>
    /// The request class of the operation to delete multiple objects in OSS.
    /// </summary>
    public class DeleteObjectsRequest
    {
        private readonly IList<string> _keys = new List<string>();
        private string _encodingType;

        /// <summary>
        /// Gets or sets the bucket name
        /// </summary>
        public string BucketName { get; private set; }
        
        /// <summary>
        /// Gets quiet mode flag. By default it's true;
        /// </summary>
        public bool Quiet { get; private set; }
        
        /// <summary>
        /// Returns the keys list where the caller could add or remove key 
        /// </summary>
        public IList<string> Keys
        {
            get { return _keys; }
        }
         
        /// <summary>
        /// Gets or sets encoding-type value. By default it's HttpUtils.UrlEncodingType.
        /// </summary>
        public string EncodingType
        {
            get
            {
                return _encodingType ??  HttpUtils.UrlEncodingType;
            }
            set
            {
                _encodingType = value;
            }
        }

        /// <summary>
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; } 

        /// <summary>
        /// Creates an instance with bucket name and keys. Quiet mode is true by default.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="keys">object lists to delete</param>
        public DeleteObjectsRequest(string bucketName, IList<string> keys)
            : this(bucketName, keys, true)
        { }

        /// <summary>
        /// Creates an instance with bucket name, keys and quiet flag.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="keys">object keys to delete</param>
        /// <param name="quiet">true: quiet mode; false: detail mode</param>
        public DeleteObjectsRequest(string bucketName, IList<string> keys, bool quiet)
        {
            if (keys == null)
                throw new ArgumentException("The list of keys to be deleted should not be null");
            if (keys.Count <= 0)
                throw new ArgumentException("No any keys specified.");
            if (keys.Count > OssUtils.DeleteObjectsUpperLimit)
                throw new ArgumentException("Count of objects to be deleted exceeds upper limit");

            BucketName = bucketName;
            foreach (var key in keys)
            {
                OssUtils.CheckObjectKey(key);
                Keys.Add(key);
            }
            Quiet = quiet;
        }
    }
}
