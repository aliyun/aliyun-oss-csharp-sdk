/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request to abort a multipart upload. It specifies all parameters needed for the operation.
    /// </summary>
    public class AbortMultipartUploadRequest
    {
        /// <summary>
        /// Gets <see cref="OssObject" />'s <see cref="Bucket" /> name.
        /// </summary>
        public string BucketName { get; private set; }
        
        /// <summary>
        ///<see cref="OssObject" /> getter
        /// </summary>
        public string Key { get; private set; }
        
        /// <summary>
        /// UploadId getter
        /// </summary>
        public string UploadId { get; private set; }

        /// <summary>
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; } 

        /// <summary>
        /// Creates a new intance <see cref="AbortMultipartUploadRequest" /> with bucket name, object key and upload Id.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object name</param>
        /// <param name="uploadId">Upload Id to cancel. It could be got from<see cref="InitiateMultipartUploadResult"/></param>
        public AbortMultipartUploadRequest(string bucketName, string key, string uploadId)
        {
            BucketName = bucketName;
            Key = key;
            UploadId = uploadId;
        }
    }
}
