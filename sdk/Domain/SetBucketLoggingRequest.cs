/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 设置存储空间日志配置的请求
    /// </summary>
   public class SetBucketLoggingRequest
    {
        /// <summary>
        /// 获取或设置<see cref="Bucket"/>名称。
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// 获取或设置存放访问日志的Bucket。
        /// </summary>
        public string TargetBucket { get; private set; }

        /// <summary>
        /// 获取或设置存放访问日志的文件名前缀。
        /// </summary>
        public string TargetPrefix { get; private set; }

        /// <summary>
        /// 构造一个新的<see cref="SetBucketLoggingRequest" />实例。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />所在<see cref="Bucket" />的名称</param>
        /// <param name="targetBucket">存放日志的存储空间名称</param>
        /// <param name="targetPrefix">产生的日子的文件名前缀</param>
        public SetBucketLoggingRequest(string bucketName, string targetBucket, string targetPrefix)
        {
            BucketName = bucketName;
            TargetBucket = targetBucket;
            TargetPrefix = targetPrefix;
        }
    }
}
