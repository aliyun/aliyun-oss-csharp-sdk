/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;

namespace Aliyun.OSS.Common.Authentication
{
    internal abstract class ServiceSignature
    {
        public abstract string SignatureMethod { get; }

        public abstract string SignatureVersion { get; }

        public string ComputeSignature(String key, String data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The parameter is empty or null.", "key");
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("The parameter is empty or null.", "data");

            return ComputeSignatureCore(key, data);
        }

        protected abstract string ComputeSignatureCore(string key, string data);

        public static ServiceSignature Create()
        {
            return new HmacSha1Signature();
        }
    }
}
