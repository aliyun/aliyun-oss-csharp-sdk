/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class for appending operation.
    /// </summary>
    public class AppendObjectResult : GenericResult
    {
        /// <summary>
        /// ETag getter/setter. ETag is calculated in the OSS server side by using the 128bit MD5 result on the object content. It's the hex string.
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// The next append position
        /// </summary>
        public long NextAppendPosition { get; internal set; }

        /// <summary>
        /// The CRC value of the object. It's calculated by ECMA-182.
        /// </summary>
        public ulong HashCrc64Ecma { get; internal set; }

        internal AppendObjectResult()
        { }
    }
}
