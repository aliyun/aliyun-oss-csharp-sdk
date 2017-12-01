/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aliyun.OSS
{
    /// <summary>
    /// Bucket state.
    /// </summary>
    public class BucketStat
    {
        public ulong Storage { get; set; }

        public ulong ObjectCount { get; set; }

        public ulong MultipartUploadCount { get; set; }
    }
}
