/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class for create symlink operation.
    /// </summary>
    public class CreateSymlinkResult : GenericResult
    {
        /// <summary>
        /// ETag getter/setter. ETag is calculated in the OSS server side by using the 128bit MD5 result on the object content. It's the hex string.
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// Gets or sets the version id.
        /// </summary>
        public string VersionId { get; internal set; }

        internal CreateSymlinkResult()
        { }
    }
}
