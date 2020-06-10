/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
namespace Aliyun.OSS
{
    /// <summary>
    /// The mode of restoring an object
    /// </summary>
    public enum TierType
    {
        Expedited,  //The object is restored within one hour
        Standard,   //The object is restored within two to five hours
        Bulk        //The object is restored within five to eleven hours
    }
}
