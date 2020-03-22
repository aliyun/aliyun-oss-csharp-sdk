/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
using System.Xml.Serialization;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// The enum of versioning status
    /// </summary>
    public enum VersioningStatus
    {
        /// <summary>
        /// OSS bucket versioning status indicating that versioning is off for a
        /// bucket. By default, all buckets start off with versioning off. Once you
        /// enable versioning for a bucket, you can never set the status back to
        /// Off". You can only suspend versioning on a bucket once you've enabled.
        /// </summary>
        Off,

        /// <summary>
        /// OSS bucket versioning status indicating that versioning is enabled for a
        /// bucket.
        /// </summary>
        Enabled,

        /// <summary>
        /// OSS bucket versioning status indicating that versioning is suspended for a
        /// bucket. Use the "Suspended" status when you want to disable versioning on
        /// a bucket that has versioning enabled.
        /// </summary>
        Suspended
    }
}

