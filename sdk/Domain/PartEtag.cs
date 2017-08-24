/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The class consists of part ETag and Part number. It's used in the request to complete the multipart upload.
    /// </summary>
    public class PartETag
    {
        /// <summary>
        /// Gets or sets the part number.
        /// </summary>
        public int PartNumber { get; set; }
        
        /// <summary>
        /// Gets or sets the ETag, which is the 128 bit MD5 digest in hex string.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="PartETag" />.
        /// </summary>
        /// <param name="partNumber">Part number</param>
        /// <param name="eTag">Etag</param>
        public PartETag(int partNumber, string eTag)
        {
            PartNumber = partNumber;
            ETag = eTag;
        }
    }
}
