/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 表示初始化MultipartUpload的结果。
    /// </summary>
    public class InitiateMultipartUploadResult
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
        /// 获取上传Id
        /// </summary>
        public string UploadId { get; internal set; }
    }
}
