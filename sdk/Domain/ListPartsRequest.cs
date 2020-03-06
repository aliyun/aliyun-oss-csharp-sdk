/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of operation to list parts of a ongoing multipart upload.
    /// </summary>
    public class ListPartsRequest
    {
        /// <summary>
        /// Gets or sets the bucket name
        /// </summary>
        public string BucketName { get; private set; }
        
        /// <summary>
        /// Gets or sets the target object key
        /// </summary>
        public string Key { get; private set; }
        
        /// <summary>
        /// Gets or sets the max parts to return.
        /// </summary>
        public int? MaxParts { get; set; }
        
        /// <summary>
        /// Gets or sets the part number marker. It will only list the parts whose numbers are greater than the property.
        /// </summary>
        public int? PartNumberMarker { get; set; }

        /// <summary>
        /// Gets encoding-type.
        /// </summary>
        public string EncodingType { get; set; }

        /// <summary>
        /// Gets UploadId.
        /// </summary>
        public string UploadId { get; private set; }

        /// <summary>
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; } 

        /// <summary>
        /// Creates an instance of <see cref="ListPartsRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">target object key</param>
        /// <param name="uploadId">upload Id</param>
        public ListPartsRequest(string bucketName, string key, string uploadId)
        {            
            BucketName = bucketName;
            Key = key;
            UploadId = uploadId;
            EncodingType = Util.HttpUtils.UrlEncodingType;
        }
    }
}
