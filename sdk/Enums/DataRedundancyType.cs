/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
using System;
namespace Aliyun.OSS
{
    /// <summary>
    /// Disaster recovery of OSS Bucket
    /// </summary>
    public enum DataRedundancyType
    {
        LRS, //Local Disaster Recovery 
        ZRS  //Zone Disaster Recovery
    }
}
