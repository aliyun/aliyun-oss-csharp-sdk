/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;

namespace Aliyun.OSS.Common.Internal
{
    public interface IHashingWrapper : IDisposable
    {
        void Clear();
        byte[] ComputeHash(byte[] buffer);
        byte[] ComputeHash(Stream stream);
        void AppendBlock(byte[] buffer);
        void AppendBlock(byte[] buffer, int offset, int count);
        byte[] AppendLastBlock(byte[] buffer);
        byte[] AppendLastBlock(byte[] buffer, int offset, int count);
    }
}
