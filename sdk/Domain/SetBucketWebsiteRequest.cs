/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 设置存储空间静态网页配置的请求
    /// </summary>
    public class SetBucketWebsiteRequest
    {
        /// <summary>
        /// 获取或者设置<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// 索引页面
        /// </summary>
        public string IndexDocument { get; private set; }

        /// <summary>
        /// 错误页面
        /// </summary>
        public string ErrorDocument { get; private set; }

        /// <summary>
        /// 构造一个新的<see cref="SetBucketWebsiteRequest" />实例。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />所在<see cref="Bucket" />的名称</param>
        /// <param name="indexDocument">存放索引的文件</param>
        /// <param name="errorDocument">产生错误时访问的文件</param>
        public SetBucketWebsiteRequest(string bucketName, string indexDocument, string errorDocument)
        {
            BucketName = bucketName;
            IndexDocument = indexDocument;
            ErrorDocument = errorDocument;
        }
    }
}
