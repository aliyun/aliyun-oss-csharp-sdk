/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Internal;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to upload part
    /// </summary>
    public class UploadPartRequest
    {
        private Stream _inputStream;

        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }
        
        /// <summary>
        /// Gets the object key
        /// </summary>
        public string Key { get; private set; }
        
        /// <summary>
        /// Gets the upload Id
        /// </summary>
        public string UploadId { get; private set; }
        
        /// <summary>
        /// Gets the part number which is between 1 to 10000.
        /// Each part has the Part number as its Id and for a given upload Id, the part number determine the part's position in the whole file.
        /// If there's another part upload with the same part number under the same upload Id, the existing data will be overwritten.
        /// </summary>
        public int? PartNumber { get; set; }
        
        /// <summary>
        /// Gets or sets the part size.
        /// Except the last part, all other parts size are at least 5MB.
        /// </summary>
        public long? PartSize { get; set; }
        
        /// <summary>
        /// Gets or sets the part data's MD5.
        /// </summary>
        public string Md5Digest { get; set; }
        
        /// <summary>
        /// Gets or sets the part's input stream.
        /// </summary>
        public Stream InputStream
        {
            get { return this._inputStream; }
            set { this._inputStream = value; }
        }

        /// <summary>
        /// Gets or sets the progress callback.
        /// </summary>
        public EventHandler<StreamTransferProgressArgs> StreamTransferProgress { get; set; }

        /// <summary>
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; }

        /// <summary>
        /// Gets or sets the traffic limit, the unit is bit/s
        /// </summary>
        public long TrafficLimit { get; set; }

        public UploadPartRequest(string bucketName, string key, string uploadId)
        {            
            BucketName = bucketName;
            Key = key;
            UploadId = uploadId;
        }
    }
}
