/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.OSS.Utilities
 * 
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
