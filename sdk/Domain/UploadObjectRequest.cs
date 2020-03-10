/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;

using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Internal;
namespace Aliyun.OSS
{
    /// <summary>
    /// Upload object request.
    /// </summary>
    public class UploadObjectRequest
    {
        private int _parallelThreadCount = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Aliyun.OSS.UploadObjectRequest"/> class.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="key">Key.</param>
        /// <param name="uploadFile">Upload file.</param>
        public UploadObjectRequest(string bucketName, string key, string uploadFile)
        {
            BucketName = bucketName;
            Key = key;
            UploadFile = uploadFile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Aliyun.OSS.UploadObjectRequest"/> class.
        /// </summary>
        /// <param name="bucketName">Bucket name.</param>
        /// <param name="key">Key.</param>
        /// <param name="uploadStream">Upload stream.</param>
        public UploadObjectRequest(string bucketName, string key, Stream uploadStream)
        {
            BucketName = bucketName;
            Key = key;
            UploadStream = uploadStream;
        }

        /// <summary>
        /// Gets or sets the name of the bucket.
        /// </summary>
        /// <value>The name of the bucket.</value>
        public string BucketName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the upload file.
        /// </summary>
        /// <value>The upload file.</value>
        public string UploadFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the upload stream.
        /// Note: when both UploadStream and UploadFile properties are set, the UploadStream will be used.
        /// It will be disposed once the ResumableUploadFile finishes.
        /// </summary>
        /// <value>The upload stream.</value>
        public Stream UploadStream
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the part.
        /// </summary>
        /// <value>The size of the part.</value>
        public long? PartSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parallel thread count.
        /// </summary>
        /// <value>The parallel thread count.</value>
        public int ParallelThreadCount
        {
            get
            {
                return _parallelThreadCount;
            }
            set
            {
                _parallelThreadCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the checkpoint dir.
        /// </summary>
        /// <value>The checkpoint dir.</value>
        public string CheckpointDir
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the stream transfer progress.
        /// </summary>
        /// <value>The stream transfer progress.</value>
        public EventHandler<StreamTransferProgressArgs> StreamTransferProgress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public ObjectMetadata Metadata
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reqeust payer
        /// </summary>
        public RequestPayer RequestPayer { get; set; }

        /// <summary>
        /// Gets or sets the traffic limit, the unit is bit/s
        /// </summary>
        public long TrafficLimit { get; set; }
    }
}
