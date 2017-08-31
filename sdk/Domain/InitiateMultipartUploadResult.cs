/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to initiate a multipart upload.
    /// </summary>
    public class InitiateMultipartUploadResult : GenericResult
    {
        /// <summary>
        /// Gets or sets bucket name
        /// </summary>
        public string BucketName { get; internal set; }
        
        /// <summary>
        /// Gets or sets the object key
        /// </summary>
        public string Key { get; internal set; }
        
        /// <summary>
        /// Gets or sets the upload Id
        /// </summary>
        public string UploadId { get; internal set; }
    }
}
