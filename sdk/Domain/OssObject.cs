/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Globalization;
using System.IO;
using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// Base class for OSS's object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In OSS, every file is an object, which should be less than 5G. 在 OSS 中，用户的每个文件都是一个 Object，每个文件需小于 5G。
    /// Object consists of key, data and metadata. Key is the object name which must be unique under the bucket.
    /// Data is the object's content. And user metadata is the key-value pair collection that has the object's additional description.
    /// </para>
    /// </remarks>
    public class OssObject : StreamResult
    {
        /// <summary>
        /// Gets or sets object key.
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// Gets or sets object's bucket name
        /// </summary>
        public string BucketName { get; internal set; }

        /// <summary>
        /// Gets or sets object's metadata.
        /// </summary>
        public ObjectMetadata Metadata { get; internal set; }

        /// <summary>
        /// Gets or sets object's content stream.
        /// </summary>
        public Stream Content 
        { 
            get { return this.ResponseStream; } 
        }

        /// <summary>
        /// Creates a new instance of <see cref="OssObject" />---internal only.
        /// </summary>
        internal OssObject()
        { }

        /// <summary>
        /// Creates a new instance of <see cref="OssObject" /> with the key name.
        /// </summary>
        internal OssObject(string key)
        {
            Key = key;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[OSSObject Key={0}, targetBucket={1}]", Key, BucketName ?? string.Empty);
        }
    }
}
