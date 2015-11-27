/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// Get Bucket Logging 的请求结果。
    /// </summary>
   public class BucketLoggingResult
    {
       /// <summary>
       /// 访问日志记录要存入的bucket。
       /// </summary>
       public string TargetBucket { get; internal set; }

       /// <summary>
       /// 存储访问日志记录的object名字前缀，可以为空。
       /// </summary>
       public string TargetPrefix { get; internal set; }
    }
}
