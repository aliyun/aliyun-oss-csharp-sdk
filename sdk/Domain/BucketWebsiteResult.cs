/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// Get Bucket Website 的请求结果。
    /// </summary>
   public class BucketWebsiteResult
    {
        /// <summary>
        /// 索引页面
        /// </summary>
       public string IndexDocument { get; internal set; }

        /// <summary>
        /// 错误页面
        /// </summary>
       public string ErrorDocument { get; internal set; }
    }
}
