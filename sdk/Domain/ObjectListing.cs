/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// 包含获取OSS的<see cref="Bucket" />中<see cref="OssObjectSummary" />列表的信息。
    /// </summary>
    public class ObjectListing
    {
        private readonly IList<OssObjectSummary> _objectSummaries = new List<OssObjectSummary>();

        private readonly IList<string> _commonPrefixes = new List<string>();

        /// <summary>
        /// 获取<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// 获取一个值表示用于下一个<see cref="P:ListObjectRequest.Marker" />以读取
        /// 结果列表的下一页。
        /// 如果结果列表没有被截取掉，则该属性返回null。
        /// </summary>
        public string NextMarker { get; internal set; }

        /// <summary>
        /// 是否结果被截取掉了
        /// </summary>
        [Obsolete("misspelled, please use IsTruncated instead")]
        public bool IsTrunked
        {
            get { return IsTruncated; }
            internal set { IsTruncated = value; }
        }
        
        /// <summary>
        /// 是否结果被截取掉了
        /// </summary>
        public bool IsTruncated { get; internal set; }

        /// <summary>
        /// 获取请求参数<see cref="P:ListObjectRequest.Marker" />的值。
        /// </summary>
        public string Marker { get; internal set; }

        /// <summary>
        /// 获取请求参数<see cref="P:ListObjectRequest.MaxKeys" />的值。
        /// </summary>
        public int MaxKeys { get; internal set; }

        /// <summary>
        /// 获取请求参数<see cref="P:ListObjectRequest.Prefix" />的值。
        /// </summary>
        public string Prefix { get; internal set; }

        /// <summary>
        /// 获取请求参数<see cref="P:ListObjectRequest.Delimiter" />的值。
        /// </summary>
        public string Delimiter { get; internal set; }

        /// <summary>
        /// 枚举满足查询条件的<see cref="OssObjectSummary" />。
        /// </summary>
        public IEnumerable<OssObjectSummary> ObjectSummaries
        {
            get { return _objectSummaries; }
        }

        /// <summary>
        /// 获取返回结果中的CommonPrefixes部分。
        /// </summary>
        public IEnumerable<string> CommonPrefixes
        {
            get { return _commonPrefixes; }
        }

        /// <summary>
        /// 构造一个新的<see cref="ObjectListing" />实例。
        /// </summary>
        /// <param name="bucketName"><see cref="Bucket" />的名称。</param>
        internal ObjectListing(string bucketName)
        {
            BucketName = bucketName;
        }
        
        internal void AddObjectSummary(OssObjectSummary summary)
        {
            _objectSummaries.Add(summary);
        }
        
        internal void AddCommonPrefix(string prefix)
        {
            _commonPrefixes.Add(prefix);
        }
    }
}
