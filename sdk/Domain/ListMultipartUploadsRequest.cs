/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to list ongoing multipart uploads.
    /// </summary>
    public class ListMultipartUploadsRequest 
    {
        /// <summary>
        /// Gets the bucket name that these multipart uploads belong to.
        /// </summary>
        public string BucketName { get; private set; }
        
        /// <summary>
        /// Gets or sets the delimiter for grouping the result.
        /// </summary>
        public string Delimiter { get; set; }
        
        /// <summary>
        /// Gets or sets the max entries to list.
        /// By default it's 1000. The max value is 1000.
        /// </summary>
        public int? MaxUploads { get; set; }
        
        /// <summary>
        /// Gets or sets the key marker.
        /// The key marker and upload id marker filter the multipart uploads to return.
        /// If the upload-id-marker is not set, then the returned uploads whose target object name are greater than key-marker.
        /// If the uploader-id-marker is set, then beside the target object's requirement above, the returned uploads Ids must be greater than the upliad-id-marker.
        /// </summary>
        public string KeyMarker { get; set; }
        
        /// <summary>
        /// Gets or sets the target object's prefix of these multipart uploads.
        /// </summary>
        public string Prefix { get; set;}
        
        /// <summary>
        /// Gets or sets upload-id-marker.
        /// The key marker and upload id marker filter the multipart uploads to return.
        /// If the key-marker is not set, the upload-id-marker is ignored by OSS.
        /// If the key marker is set, then:
        ///     All target objects' name must be greater than key-marker value in lexicographic order.
        ///     And all the Upload IDs returned must be greater than upload-id-marker.
        /// </summary>
        public string UploadIdMarker { get; set;}

        /// <summary>
        /// Gets or sets encoding-type value.
        /// </summary>
        public string EncodingType { get; set; }

        /// <summary>
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; } 

        /// <summary>
        /// Creates an instance of <see cref="ListMultipartUploadsRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public ListMultipartUploadsRequest(string bucketName)
        {
            BucketName = bucketName;
            EncodingType = Util.HttpUtils.UrlEncodingType;
        }
    }
}
