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
    /// The result object of GetSymlink
    /// </summary>
    public class OssSymlink
    {
        /// <summary>
        /// Gets or sets the symlink's metadata.
        /// </summary>
        /// <value>The symlink's metadata.</value>
        public ObjectMetadata ObjectMetadata { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the symlink.
        /// </summary>
        /// <value>The symlink.</value>
        public string Symlink { get; set; }
    }
}
