/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to upload part.
    /// </summary>
    public class UploadPartResult : GenericResult
    {
        /// <summary>
        /// Gets or sets Object ETag
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// Gets or sets Part number
        /// </summary>
        public int PartNumber { get; internal set; }

        /// <summary>
        /// Gets or sets the crc64.
        /// </summary>
        /// <value>The crc64.</value>
        public string Crc64 { get; internal set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public long Length { get; internal set; }

        /// <summary>
        /// Gets the PartEtag instance which consists of a part number and the part's ETag
        /// </summary>
        public PartETag PartETag
        {
            get { return new PartETag(PartNumber, ETag, Crc64, Length); }
        }

        internal UploadPartResult()
        { }
    }
}
