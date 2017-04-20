/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Internal;

namespace Aliyun.OSS
{
    /// <summary>
    /// 指定上传Object的请求参数。
    /// </summary>
    public class PutObjectRequest
    {
        private Stream _inputStream;

        /// <summary>
        /// 获取或设置<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// 获取或设置要下载<see cref="OssObject" />的Key。
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 获取或设置Object内容的数据流。
        /// </summary>
        public Stream Content
        {
            get { return this._inputStream; }
            set { this._inputStream = value; }
        }

        /// <summary>
        /// 获取或设置进度回调
        /// </summary>
        public EventHandler<StreamTransferProgressArgs> StreamTransferProgress { get; set; }

        /// <summary>
        /// 获取或者设置Object的Metadata信息。
        /// </summary>
        public ObjectMetadata Metadata { get; set; }

        /// <summary>
        /// 获取或设置Object上传后的处理方法，处理结果<see cref="P:PutObjectResult.ResponseStream" />。
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// 构造一个新的<see cref="PutObjectRequest" /> 实例
        /// </summary>
        /// <param name="bucketName">需要上传的<see cref="OssObject" />所在的Bucket</param>
        /// <param name="key">需要上传的<see cref="OssObject" />名称</param>
        /// <param name="content">需要上传的<see cref="OssObject" />内容</param>
        public PutObjectRequest(string bucketName, string key, Stream content) 
            : this(bucketName, key, content, null) { }

        /// <summary>
        /// 构造一个新的<see cref="PutObjectRequest" /> 实例
        /// </summary>
        /// <param name="bucketName">需要上传的<see cref="OssObject" />所在的Bucket</param>
        /// <param name="key">需要上传的<see cref="OssObject" />名称</param>
        /// <param name="content">需要上传的<see cref="OssObject" />内容</param>
        /// <param name="metadata">需要上传的<see cref="ObjectMetadata" />Metadata信息</param>
        public PutObjectRequest(string bucketName, string key, Stream content, ObjectMetadata metadata)
        {
            BucketName = bucketName;
            Key = key;
            Content = content;
            Metadata = metadata;
        }

        internal void Populate(IDictionary<string, string> headers)
        {
            if (Metadata != null) 
            {
                Metadata.Populate(headers);
            }
        }

        /// <summary>
        /// 是否需要返回消息体，删除回调、UDF/Image时返回
        /// </summary>
        internal bool IsNeedResponseStream()
        {
            if ((Process != null) || 
                (Metadata != null && Metadata.HttpMetadata.ContainsKey(HttpHeaders.Callback)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 请求是否携带有上传回调参数
        /// </summary>
        internal bool IsCallbackRequest()
        {
            if (Metadata != null && Metadata.HttpMetadata.ContainsKey(HttpHeaders.Callback))
            {
                return true;
            }
            return false;
        }
    }
}
