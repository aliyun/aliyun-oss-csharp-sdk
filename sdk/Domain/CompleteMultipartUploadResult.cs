/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of operation to complete a multipart upload.
    /// </summary>
    public class CompleteMultipartUploadResult : PutObjectResult
    {
        /// <summary>
        /// Bucket name getter/setter
        /// </summary>
        public string BucketName { get; internal set; }
        
        /// <summary>
        /// Object key's getter/setter.
        /// </summary>
        public string Key { get; internal set; }
        
        /// <summary>
        /// The new object' URL
        /// </summary>
        public string Location { get; internal set; }

        internal CompleteMultipartUploadResult()
        { }
    }
}
