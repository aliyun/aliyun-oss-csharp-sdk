/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS.Transform
{
    internal interface ISerializer<in TInput, out TOutput>
    {
        TOutput Serialize(TInput input);       
    }
}
