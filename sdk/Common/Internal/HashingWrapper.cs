/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Aliyun.OSS.Common.ThirdParty;

namespace Aliyun.OSS.Common.Internal
{
    public class HashingWrapper : IHashingWrapper
    {
        private static string MD5ManagedName = typeof(MD5Managed).FullName;

        private HashAlgorithm _algorithm = null;
        private void Init(string algorithmName)
        {
            if (string.Equals(MD5ManagedName, algorithmName, StringComparison.Ordinal))
                _algorithm = new MD5Managed();
            else
                throw new ArgumentOutOfRangeException(algorithmName, "Unsupported hashing algorithm");
        }

        public HashingWrapper(string algorithmName)
        {
            if (string.IsNullOrEmpty(algorithmName))
                throw new ArgumentNullException("algorithmName");

            Init(algorithmName);
        }

        #region IHashingWrapper Members

        public void Clear()
        {
            _algorithm.Initialize();
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            return _algorithm.ComputeHash(buffer);
        }

        public byte[] ComputeHash(Stream stream)
        {
            return _algorithm.ComputeHash(stream);
        }

        public void AppendBlock(byte[] buffer, int offset, int count)
        {
            _algorithm.TransformBlock(buffer, offset, count, null, 0);
        }

        public byte[] AppendLastBlock(byte[] buffer, int offset, int count)
        {
            _algorithm.TransformFinalBlock(buffer, offset, count);
            return _algorithm.Hash;
        }

        public void AppendBlock(byte[] buffer)
        {
            AppendBlock(buffer, 0, buffer.Length);
        }

        public byte[] AppendLastBlock(byte[] buffer)
        {
            return AppendLastBlock(buffer, 0, buffer.Length);
        }

        #endregion

        #region Dispose Pattern Implementation

        /// <summary>
        /// Implements the Dispose pattern
        /// </summary>
        /// <param name="disposing">Whether this object is being disposed via a call to Dispose
        /// or garbage collected.</param>
        protected virtual void Dispose(bool disposing)
        {
            var disposable = _algorithm as IDisposable;
            if (disposing && disposable != null)
            {
                disposable.Dispose();
                _algorithm = null;
            }
        }

        /// <summary>
        /// Disposes of all managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public class HashingWrapperMD5 : HashingWrapper
    {
        public HashingWrapperMD5()
            : base(typeof(MD5Managed).FullName)
        { }
    }
}
