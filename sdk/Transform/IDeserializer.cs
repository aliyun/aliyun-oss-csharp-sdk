/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal interface IDeserializer<in TInput, out TOutput>
    {
        TOutput Deserialize(TInput xmlStream);
    }
}
