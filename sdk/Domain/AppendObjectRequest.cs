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

namespace Aliyun.OSS
{
#pragma warning disable 618, 3005

    /// <summary>
    /// 指定追加Object的请求参数
    /// </summary>
    public class AppendObjectRequest
    {
        /// <summary>
        /// 获取或者设置Object所在的Bucket的名称。
        /// </summary>
        public string BucketName { get; set; }
        
        /// <summary>
        /// 获取或者设置Object的Key。
        /// </summary>
        public string Key { get; set; }   

        /// <summary>
        /// 获取或者设置目标Object的Metadata信息。
        /// </summary>
        public ObjectMetadata ObjectMetadata { get; set; }

        /// <summary>
        /// 设置或获取追加的位置。第一次可以通过GetObjectMeta获取，后续可以从前一次的AppendObjectResult中获取
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// 需要追加的内容
        /// </summary>
        public Stream Content { get; set; }
        
        /// <summary>
        /// 构造一个新的<see cref="AppendObjectRequest" /> 实例
        /// </summary>
        /// <param name="bucketName">需要追加的<see cref="OssObject" />所在的Bucket</param>
        /// <param name="key">需要追加的<see cref="OssObject" />名称</param>
        public AppendObjectRequest(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
        }
        
        internal void Populate(IDictionary<string, string> headers)
        {
            ObjectMetadata.Populate(headers);
        }
    }

#pragma warning restore 618, 3005
}
