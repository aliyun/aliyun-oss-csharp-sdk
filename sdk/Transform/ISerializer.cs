/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS.Transform
{
    internal interface ISerializer<in TInput, out TOutput>
    {
        TOutput Serialize(TInput input);       
    }
}
