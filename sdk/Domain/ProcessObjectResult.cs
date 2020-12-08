/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
using System.IO;

namespace Aliyun.OSS
{
    /// <summary>
    /// The result class of the operation to process the object.
    /// </summary>
    public class ProcessObjectResult : GenericResult
    {
        /// <summary>
        /// Gets the content of result
        /// </summary>
        public string Content { get; internal set; }
    }
}
