/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// Complete Multipart Upload的请求结果。
    /// </summary>
    public class CompleteMultipartUploadResult
    {
        /// <summary>
        /// 获取<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; internal set; }
        
        /// <summary>
        /// 获取<see cref="OssObject" />的Key。
        /// </summary>
        public string Key { get; internal set; }
        
        /// <summary>
        /// 获取新创建Object的URL。
        /// </summary>
        public string Location { get; internal set; }
        
        /// <summary>
        /// 获取新创建的Object的ETag
        /// </summary>
        public string ETag { get; internal set; }
        
        internal CompleteMultipartUploadResult()
        { }
    }
}
