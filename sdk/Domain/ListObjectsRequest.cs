/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// 包含获取<see cref="OssObjectSummary" />列表的请求信息。
    /// </summary>
    public class ListObjectsRequest
    {
        private string _prefix;
        private string _marker;
        private Int32? _maxKeys;
        private string _delimiter;
        private string _encodingType;

        /// <summary>
        /// 获取<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// 获取或设置一个值，限定返回的<see cref="OssObject" />的Key必须以该值作为前缀。
        /// </summary>
        public string Prefix
        {
            get { return _prefix; }
            set
            {
                if (value != null && value.Length > OssUtils.MaxPrefixStringSize)
                    throw new ArgumentException("parameter 'prefix' exceeds max size limit.");
                _prefix = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，用户设定结果从该值之后按字母排序的第一个开始返回。
        /// </summary>
        public string Marker
        {
            get { return _marker; }
            set
            {
                if (value != null && value.Length > OssUtils.MaxMarkerStringSize)
                    throw new ArgumentException("parameter 'marker' exceeds max size limit.");
                _marker = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，用于限定此次返回object的最大数。
        /// 如果不设定，默认为100。
        /// </summary>
        public Int32? MaxKeys
        {
            get { return _maxKeys.HasValue ? _maxKeys.Value : 100; }
            set
            {
                if (value > OssUtils.MaxReturnedKeys)
                    throw new ArgumentException("parameter 'maxkeys' exceed max limit.");
                _maxKeys = value;
            }
        }

        /// <summary>
        /// 获取或设置用于对<see cref="OssObject" />按Key进行分组的字符。
        /// </summary>
        public string Delimiter
        {
            get { return _delimiter; }
            set
            {
                if (value != null && value.Length > OssUtils.MaxDelimiterStringSize)
                    throw new ArgumentException("parameter 'delimiter' exceeds max size limit.");
                _delimiter = value;
            }
        }

        /// <summary>
        /// 获取encoding-type的值
        /// </summary>
        public string EncodingType
        {
            get
            {
                return this._encodingType != null ? this._encodingType : HttpUtils.UrlEncodingType;
            }
            set
            {
                this._encodingType = value;
            }
        }

        /// <summary>
        /// 使用给定的<see cref="Bucket" />名称构造一个新的<see cref="ListObjectsRequest" />实体。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />所在<see cref="Bucket" />的名称。</param>
        public ListObjectsRequest(string bucketName)
        {
            BucketName = bucketName;
        }
    }
}
