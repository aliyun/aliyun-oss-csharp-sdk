/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to list ongoing multipart uploads.
    /// </summary>
    public class MultipartUploadListing : GenericResult
    {
        private readonly IList<MultipartUpload> _multipartUploads = new List<MultipartUpload>();
        private readonly IList<string> _commonPrefixes = new List<string>();
        
        /// <summary>
        /// bucket name
        /// </summary>
        public string BucketName { get; internal set; }
        
        /// <summary>
        /// The key marker from <see cref="P:ListMultipartUploadsRequest.KeyMarker" />.
        /// </summary>
        public string KeyMarker { get; internal set; }
        
        /// <summary>
        /// The delimiter from <see cref="P:ListMultipartUploadsRequest.Delimiter" />
        /// </summary>
        public string Delimiter { get; internal set; }
        
        /// <summary>
        /// The prefix from <see cref="P:ListMultipartUploadsRequest.Prefix" />
        /// </summary>
        public string Prefix { get; internal set; }
        
        /// <summary>
        /// The upload Id marker from <see cref="P:ListMultipartUploadsRequest.UploadIdMarker" />.
        /// </summary>
        public string UploadIdMarker { get; internal set; }
        
        /// <summary>
        /// The max upload count from <see cref="P:ListMultipartUploadsRequest.MaxUploads" />
        /// </summary>
        public int MaxUploads { get; internal set; }
        
        /// <summary>
        /// The flag which indciates if there's more data to return in OSS server side.
        /// “true” means there's more data to return.
        /// “false” means no more data to return.
        /// </summary>
        public bool IsTruncated { get; internal set; }
        
        /// <summary>
        /// Gets the next key marker value. If the IsTruncated is true, this could be the next list call's KeyMarker value.
        /// </summary>
        public string NextKeyMarker { get; internal set; }
        
        /// <summary>
        /// Gets the next upload id marker value. If the IsTruncated is true, this value could be the next list call's UploadIdMarker value.
        /// </summary>
        public string NextUploadIdMarker { get; internal set; }
        
        /// <summary>
        /// The iterator of all multipart upload returned.
        /// </summary>
        public IEnumerable<MultipartUpload> MultipartUploads
        {
            get { return _multipartUploads; }
        }
        
        /// <summary>
        /// Gets all the common prefixes (which could be thought as virtual 'folder').
        /// </summary>
        public IEnumerable<string> CommonPrefixes
        {
            get { return _commonPrefixes; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="MultipartUploadListing" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public MultipartUploadListing(string bucketName)
        {
            BucketName = bucketName;
        }
        
        /// <summary>
        /// Adds a <see cref="MultipartUpload"/> instance---internal only.
        /// </summary>
        /// <param name="multipartUpload">a multipart upload instance</param>
        internal void AddMultipartUpload(MultipartUpload multipartUpload)
        {
            _multipartUploads.Add(multipartUpload);
        }
        
        /// <summary>
        /// Adds a prefix---internal only.
        /// </summary>
        /// <param name="prefix">The prefix</param>
        internal void AddCommonPrefix(string prefix)
        {
            _commonPrefixes.Add(prefix);
        }
    }
}
