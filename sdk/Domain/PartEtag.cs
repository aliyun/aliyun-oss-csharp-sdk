/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 某块PartNumber和ETag的信息，用于Complete Multipart Upload请求参数的设置。
    /// </summary>
    public class PartETag
    {
        /// <summary>
        /// 获取或者设置一个值表示表示分块的标识
        /// </summary>
        public int PartNumber { get; set; }
        
        /// <summary>
        /// 获取或者设置一个值表示与Object相关的hex编码的128位MD5摘要。
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// 构造一个新的<see cref="PartETag" />实例。
        /// </summary>
        /// <param name="partNumber">分片的id</param>
        /// <param name="eTag">分片的ETag值</param>
        public PartETag(int partNumber, string eTag)
        {
            PartNumber = partNumber;
            ETag = eTag;
        }
    }
}
