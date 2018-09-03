/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Aliyun.OSS.Common.Authentication
{
    internal class HmacSha256Signature : ServiceSignature
    {
        private static readonly Encoding Encoding = Encoding.UTF8;
        public override string SignatureMethod
        {
            get { return "HmacSHA256"; }
        }

        public override string SignatureVersion
        {
            get { return "v2"; }
        }

        protected override string ComputeSignatureCore(string key, string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));
#if NETCOREAPP2_0
            using (var algorithm = new HMACSHA256())
#else
            using (var algorithm = KeyedHashAlgorithm.Create(SignatureMethod.ToUpperInvariant()))
#endif
            {
                algorithm.Key = Encoding.GetBytes(key.ToCharArray());
                return Convert.ToBase64String(
                    algorithm.ComputeHash(Encoding.GetBytes(data.ToCharArray())));
            }
        }

    }
}
