/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// 列举Bucket的请求结果。
    /// </summary>
    public class ListBucketsResult
    {
        /// <summary>
        /// 获取一个值，限定返回的<see cref="Bucket" />的Key必须以该值作为前缀。
        /// </summary>
        public string Prefix { get; internal set; }

        /// <summary>
        /// 获取一个值，用户设定结果从该值之后按字母排序的第一个开始返回。
        /// </summary>
        public string Marker { get; internal set; }

        /// <summary>
        /// 获取一个值，用于限定此次返回bucket的最大数。
        /// 如果不设定，默认为100。
        /// </summary>
        public int? MaxKeys { get; internal set; }

        /// <summary>
        /// 获取一个值，指明是否所有的结果都已经返回。
        /// </summary>
        public bool? IsTruncated { get; internal set; }

        /// <summary>
        /// 获取一个值，指明下一个Marker。
        /// </summary>
        public string NextMaker { get; internal set; }

        /// <summary>
        /// 获取一个值，指明Bucket请求列表。
        /// </summary>
        public IEnumerable<Bucket> Buckets { get; internal set; }
    }
}
