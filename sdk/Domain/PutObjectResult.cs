/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 表示上传<see cref="OssObject" />时的请求结果。
    /// </summary>
    public class PutObjectResult
    {
        /// <summary>
        /// 获取一个值表示与Object相关的hex编码的128位MD5摘要。
        /// </summary>
        public string ETag { get; internal set; }

        internal PutObjectResult()
        { }
    }
}
