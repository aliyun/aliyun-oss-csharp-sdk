/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{    
    /// <summary>
    /// The result class of the operation to upload a source file as the target object's one part.
    /// </summary>
    public class UploadPartCopyResult : GenericResult
    {
        /// <summary>
        /// The ETag of the source object
        /// </summary>
        public string ETag { get; internal set; }
        
        /// <summary>
        /// The part number of the target object
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
        /// Gets the wrapper class of the part number and ETag.
        /// </summary>
        public PartETag PartETag
        {
            get { return new PartETag(PartNumber, ETag, Crc64, Length); }
        }

        /// <summary>
        /// Gets or sets the copy source version id.
        /// </summary>
        public string CopySourceVersionId { get; internal set; }

        internal UploadPartCopyResult()
        { }
    }
}
