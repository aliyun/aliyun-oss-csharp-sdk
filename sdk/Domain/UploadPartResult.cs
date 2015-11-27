/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 获取上传某分块的结果。
    /// </summary>
    public class UploadPartResult
    {
        /// <summary>
        /// 获取一个值表示与Object相关的hex编码的128位MD5摘要。
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// 获取一个值表示分块的标识
        /// </summary>
        public int PartNumber { get; internal set; }

        /// <summary>
        /// 获取包含Part标识号码和ETag值的<see cref="PartETag" />对象
        /// </summary>
        public PartETag PartETag
        {
            get { return new PartETag(PartNumber, ETag); }
        }

        internal UploadPartResult()
        { }
}
}
