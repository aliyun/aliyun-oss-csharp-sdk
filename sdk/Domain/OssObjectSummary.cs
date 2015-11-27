/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Globalization;

namespace Aliyun.OSS
{
    /// <summary>
    /// <see cref="OssObject" />的摘要信息。
    /// </summary>
    public class OssObjectSummary
    {
        /// <summary>
        /// 获取Object所在的<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; internal set; }

        /// <summary>
        /// 获取Object的Key。
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// 获取一个值表示与Object相关的hex编码的128位MD5摘要。
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// 获取Object的文件字节数。
        /// </summary>
        public long Size { get; internal set; }

        /// <summary>
        /// 获取最后修改时间。
        /// </summary>
        public DateTime LastModified { get; internal set; }

        /// <summary>
        /// 获取Object的存储类别。
        /// </summary>
        public string StorageClass { get; internal set; }

        /// <summary>
        /// 获取Object的<see cref="Owner" />。
        /// </summary>
        public Owner Owner { get; internal set; }

        /// <summary>
        /// 初始化一个新的<see cref="OssObjectSummary" />实例。
        /// </summary>
        internal OssObjectSummary()
        { }

        /// <summary>
        /// 获取该实例的字符串表示。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[OSSObjectSummary Bucket={0}, Key={1}]", BucketName, Key);
        }

    }
}
