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
    /// The object's identifier.
    /// </summary>
    public class ObjectIdentifier
    {
        /// <summary>
        /// the object key.
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// the object version id.
        /// </summary>
        public string VersionId { get; internal set; }

        /// <summary>
        /// Creates a new intance of <see cref="ObjectIdentifier" />.
        /// </summary>
        /// <param name="key">object name</param>
        public ObjectIdentifier(string key)
            : this(key, "")
        {
        }

        /// <summary>
        /// Creates a new intance of <see cref="ObjectIdentifier" />.
        /// </summary>
        /// <param name="key">object name</param>
        /// <param name="versionId">the object version id</param>
        public ObjectIdentifier(string key, string versionId)
        {
            Key = key;
            VersionId = versionId;
        }
    }
}
