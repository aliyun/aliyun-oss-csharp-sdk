/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
using System;

namespace Aliyun.OSS
{
#pragma warning disable 618, 3005

    /// <summary>
    /// The request class of the operation to initiate a multipart upload
    /// </summary>
    public class InitiateMultipartUploadRequest
    {
        /// <summary>
        /// Gets or sets the bucket name to upload files to.
        /// </summary>
        public string BucketName { get; set; }
        
        /// <summary>
        /// Gets or sets the target <see cref="OssObject" /> key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the encoding-type value
        /// </summary>
        public string EncodingType { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ObjectMetadata" />
        /// </summary>
        public ObjectMetadata ObjectMetadata { get; set; }

        /// <summary>
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; } 

        /// <summary>
        /// Creates a new instance of <see cref="InitiateMultipartUploadRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        public InitiateMultipartUploadRequest(string bucketName, string key) 
            : this(bucketName, key, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="InitiateMultipartUploadRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        /// <param name="objectMetadata">Object's <see cref="ObjectMetadata"/></param>
        public InitiateMultipartUploadRequest(string bucketName, string key, 
            ObjectMetadata objectMetadata)
        {
            BucketName = bucketName;
            Key = key;
            ObjectMetadata = objectMetadata;
            EncodingType = Util.HttpUtils.UrlEncodingType;
        }
    }

#pragma warning disable 618, 3005
}
