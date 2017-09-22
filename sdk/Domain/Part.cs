/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
using System;
using System.Globalization;

namespace Aliyun.OSS
{
    /// <summary>
    /// The part's summary information in a multipart upload. It does not have the actual content data.
    /// </summary>
    public class Part
    {
        internal Part()
        { }
        
        /// <summary>
        /// Parts number.
        /// </summary>
        public int PartNumber { get; internal set; }
        
        /// <summary>
        /// Part's last updated time (typically it's just the upload time)
        /// </summary>
        public DateTime LastModified { get; internal set; }
        
        /// <summary>
        /// The Etag of the part content.
        /// </summary>
        public string ETag { get; internal set; }
        
        /// <summary>
        /// Size of the part content, in bytes.
        /// </summary>
        public long Size { get; internal set; }
        
        /// <summary>
        /// The serialization string
        /// </summary>
        /// <returns>the serialization string</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[Part PartNumber={0}, ETag={1}, LastModified={2}, Size={3}]", 
                PartNumber, ETag, LastModified, Size);
        }
        
        /// <summary>
        /// Gets the <see cref="PartETag" /> instance which consists of the part number and the ETag.
        /// </summary>
        public PartETag PartETag
        {
            get { return new PartETag(PartNumber, ETag); }
        }
        
    }
}
