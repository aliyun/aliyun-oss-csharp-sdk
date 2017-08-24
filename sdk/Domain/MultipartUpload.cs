/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
using System;
using System.Globalization;

namespace Aliyun.OSS
{
    /// <summary>
    /// The mutipart upload class definition.
    /// </summary>
    public class MultipartUpload
    {
        /// <summary>
        /// Gets or sets the target object's key.
        /// </summary>
        public string Key { get; internal set; }
        
        /// <summary>
        /// Gets or sets the upload Id.
        /// </summary>
        public string UploadId { get; internal set; }
        
        /// <summary>
        /// Gets or sets the target object's storage class.
        /// </summary>
        public string StorageClass {get; internal set; }
        
        /// <summary>
        /// The initiated timestamp of the multipart upload.
        /// </summary>
        public DateTime Initiated {get; internal set;}
        
        internal MultipartUpload()
        { }
        
        /// <summary>
        /// Gets the serialization string
        /// </summary>
        /// <returns>the serilization string</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[MultipartUpload Key={0}, UploadId={1}]", Key, UploadId);
        }
    }
}
