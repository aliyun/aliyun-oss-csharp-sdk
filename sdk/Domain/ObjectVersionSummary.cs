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
    /// <see cref="OssObject" /> of a version summary information, no object data.
    /// </summary>
    public class ObjectVersionSummary
    {
        /// <summary>
        /// Gets or sets the bucket name
        /// </summary>
        public string BucketName { get; internal set; }

        /// <summary>
        /// Gets or sets the object key.
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// Gets or sets the version of a object.
        /// </summary>
        public string VersionId { get; internal set; }

        /// <summary>
        /// Gets or sets if it is the latest version of a object.
        /// </summary>
        public bool IsLatest { get; internal set; }

        /// <summary>
        /// Gets or sets the ETag which is the MD5 summry in hex string of the object.
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// Gets or sets the size of the object in bytes.
        /// </summary>
        public long Size { get; internal set; }

        /// <summary>
        /// Gets or sets the last modified time.
        /// </summary>
        public DateTime LastModified { get; internal set; }

        /// <summary>
        /// Gets or sets the object's storage class.
        /// </summary>
        public string StorageClass { get; internal set; }

        /// <summary>
        /// Gets or sets the object's <see cref="Owner" />.
        /// </summary>
        public Owner Owner { get; internal set; }

        /// <summary>
        /// Gets or sets the object's type.
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectVersionSummary" />.
        /// </summary>
        internal ObjectVersionSummary()
        { }

        /// <summary>
        /// Gets the serialization result in string.
        /// </summary>
        /// <returns>serialization result in string</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[ObjectVersionSummary Bucket={0}, Key={1}, VersionId={1}]", 
                BucketName, Key, VersionId);
        }

    }
}
