/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using Aliyun.OSS.Properties;

namespace Aliyun.OSS.Common.Authentication
{
    internal abstract class ServiceSignature
    {
        public abstract string SignatureMethod { get; }

        public abstract string SignatureVersion { get; }

        public string ComputeSignature(String key, String data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "key");
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "data");

            return ComputeSignatureCore(key, data);
        }

        protected abstract string ComputeSignatureCore(string key, string data);

        public static ServiceSignature Create()
        {
            return new HmacSha1Signature();
        }
    }
}
