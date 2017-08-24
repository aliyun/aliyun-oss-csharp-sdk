﻿/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// The enum of canned access control list.
    /// <para>
    /// This is a quick way to grant permissions to all users
    /// </para>
    /// </summary>
    public enum CannedAccessControlList
    {
        /// <summary>
        /// Private read and write.
        /// </summary>
        [StringValue("private")]
        Private = 0,

        /// <summary>
        /// Public read, private write.
        /// </summary>
        [StringValue("public-read")]
        PublicRead,

        /// <summary>
        /// public read or write---everyone can read and write the data.
        /// </summary>
        [StringValue("public-read-write")]
        PublicReadWrite,

        /// <summary>
        /// Default permission, inherits from the bucket.
        /// </summary>
        [StringValue("default")]
        Default
    }
}
