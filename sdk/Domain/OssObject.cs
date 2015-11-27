/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Globalization;
using System.IO;

namespace Aliyun.OSS
{
    /// <summary>
    /// 表示OSS中的Object。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 在 OSS 中，用户的每个文件都是一个 Object，每个文件需小于 5G。
    /// Object包含key、data和user meta。其中，key是Object 的名字；
    /// data是Object 的数据；user meta是用户对该object的描述。
    /// </para>
    /// </remarks>
    public class OssObject : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// 获取或设置Object的Key。
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// 获取或设置Object所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; internal set; }

        /// <summary>
        /// 获取Object的元数据。
        /// </summary>
        public ObjectMetadata Metadata { get; internal set; }

        /// <summary>
        /// 获取或设置Object内容的数据流。
        /// </summary>
        public Stream Content { get; internal set; }

        /// <summary>
        /// 构造一个新的<see cref="OssObject" />实例。
        /// </summary>
        internal OssObject()
        { }

        /// <summary>
        /// 构造一个新的<see cref="OssObject" />实例。
        /// </summary>
        internal OssObject(string key)
        {
            Key = key;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[OSSObject Key={0}, targetBucket={1}]", Key, BucketName ?? string.Empty);
        }

        /// <summary>
        /// 释放<see cref="OssObject.Content" />。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (Content != null)
                    Content.Dispose();
                _disposed = true;
            }
        }
    }
}
