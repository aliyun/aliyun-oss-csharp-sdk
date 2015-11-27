/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 包含获取<see cref="Bucket" />列表的请求信息。
    /// </summary>
    public class ListBucketsRequest
    {
        /// <summary>
        /// 获取或设置一个值，限定返回的<see cref="Bucket" />的Key必须以该值作为前缀。
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 获取或设置一个值，用户设定结果从该值之后按字母排序的第一个开始返回。
        /// </summary>
        public string Marker { get; set; }

        /// <summary>
        /// 获取或设置一个值，用于限定此次返回bucket的最大数。
        /// 如果不设定，默认为100。
        /// </summary>
        public int? MaxKeys { get; set; }
    }
}
