/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */
 
using System;

namespace Aliyun.OSS
{
#pragma warning disable 618, 3005

    /// <summary>
    /// 指定初始化Multipart Upload的请求参数。
    /// </summary>
    public class InitiateMultipartUploadRequest
    {
        /// <summary>
        /// 获取或者设置<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; set; }
        
        /// <summary>
        /// 获取或者设置<see cref="OssObject" />的值。
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 获取encoding-type的值
        /// </summary>
        public string EncodingType { get; set; }

        /// <summary>
        /// 获取或设置<see cref="ObjectMetadata" />
        /// </summary>
        public ObjectMetadata ObjectMetadata { get; set; }

        /// <summary>
        /// 构造一个新的<see cref="InitiateMultipartUploadRequest" />实例。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />所在<see cref="Bucket" />的名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="P:OssObject.Key" />。</param>
        public InitiateMultipartUploadRequest(string bucketName, string key) 
            : this(bucketName, key, null)
        { }

        /// <summary>
        /// 构造一个新的<see cref="InitiateMultipartUploadRequest" />实例。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />所在<see cref="Bucket" />的名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="P:OssObject.Key" />。</param>
        /// <param name="objectMetadata">文件的元数据. <see cref="ObjectMetadata"/></param>
        public InitiateMultipartUploadRequest(string bucketName, string key, 
            ObjectMetadata objectMetadata)
        {
            BucketName = bucketName;
            Key = key;
            ObjectMetadata = objectMetadata;
            EncodingType = Util.HttpUtils.UrlEncodingType;
        }
    }

#pragma warning disable 618, 3005
}
