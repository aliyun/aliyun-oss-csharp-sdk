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
    /// The request class of the operation to delete multiple objects with version id in OSS.
    /// </summary>
    public class DeleteObjectVersionsRequest
    {
        private readonly IList<ObjectIdentifier> _objects = new List<ObjectIdentifier>();
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
        /// Returns the object list where the caller could add or remove key 
        /// </summary>
        public IList<ObjectIdentifier> Objects
        {
            get { return _objects; }
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
        /// <param name="objects">object lists to delete</param>
        public DeleteObjectVersionsRequest(string bucketName, IList<ObjectIdentifier> objects)
            : this(bucketName, objects, true)
        { }

        /// <summary>
        /// Creates an instance with bucket name, objects and quiet flag.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="objects">object keys to delete</param>
        /// <param name="quiet">true: quiet mode; false: detail mode</param>
        public DeleteObjectVersionsRequest(string bucketName, IList<ObjectIdentifier> objects, bool quiet)
        {
            if (objects == null)
                throw new ArgumentException("The list of keys to be deleted should not be null");
            if (objects.Count <= 0)
                throw new ArgumentException("No any keys specified.");
            if (objects.Count > OssUtils.DeleteObjectsUpperLimit)
                throw new ArgumentException("Count of objects to be deleted exceeds upper limit");

            BucketName = bucketName;
            foreach (var o in objects)
            {
                OssUtils.CheckObjectKey(o.Key);
                Objects.Add(o);
            }
            Quiet = quiet;
        }
    }
}
