/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.OSS.Utilities
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;

namespace Aliyun.OSS.Util
{
    internal sealed class StringValueAttribute : Attribute
    {
        public string Value { get; private set; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }
}
