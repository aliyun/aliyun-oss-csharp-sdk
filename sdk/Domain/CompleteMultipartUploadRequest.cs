/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;

using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of operation to complete a multipart upload
    /// </summary>
    public class CompleteMultipartUploadRequest
    {
        private readonly IList<PartETag> _partETags = new List<PartETag>();  
        
        /// <summary>
        /// Bucket name getter/setter
        /// </summary>
        public string BucketName { get; private set; }
        
        /// <summary>
        /// Object key getter/setter
        /// </summary>
        public string Key { get; private set; }
        
        /// <summary>
        /// Upload Id's getter/setter. 
        /// </summary>
        public string UploadId { get; private set; }
        
        /// <summary>
        /// <see cref="PartETag" /> list getter. 
        /// </summary>
        public IList<PartETag> PartETags
        {
            get { return _partETags; }
        }

        /// <summary>
        /// <see cref="ObjectMetadata" /> getter/setter
        /// </summary>
        public ObjectMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; } 

        /// <summary>
        /// Creates a <see cref="CompleteMultipartUploadRequest" /> instance by bucket name, object key and upload Id.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        /// <param name="uploadId">Upload Id, which is got from <see cref="InitiateMultipartUploadResult"/></param>
        public CompleteMultipartUploadRequest(string bucketName, string key, string uploadId)
        {
            BucketName = bucketName;
            Key = key;
            UploadId = uploadId;
        }

        /// <summary>
        /// Flag of containing the http body in the response.
        /// </summary>
        internal bool IsNeedResponseStream()
        {
            if (Metadata != null && Metadata.HttpMetadata.ContainsKey(HttpHeaders.Callback))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Flag of containing the callback parameters in the request.
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
