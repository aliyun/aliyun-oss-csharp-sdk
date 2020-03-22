/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    public class RestoreObjectResult : GenericResult
    {
        /// <summary>
        /// Gets or sets the version id.
        /// </summary>
        public string VersionId { get; internal set; }
    }
}
