/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */
 
using System;
using System.Globalization;

namespace Aliyun.OSS
{
    /// <summary>
    /// 获取Multipart Upload事件中某块数据的信息。
    /// </summary>
    public class Part
    {
        internal Part()
        { }
        
        /// <summary>
        /// 获取分块的编号
        /// </summary>
        public int PartNumber { get; internal set; }
        
        /// <summary>
        /// 获取分块上传的时间
        /// </summary>
        public DateTime LastModified { get; internal set; }
        
        /// <summary>
        /// 获取分块内容的ETag
        /// </summary>
        public string ETag { get; internal set; }
        
        /// <summary>
        /// 获取分块的大小，单位字节
        /// </summary>
        public long Size { get; internal set; }
        
        /// <summary>
        /// 获取该实例的字符串表示。
        /// </summary>
        /// <returns>实例的字符串形式</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[Part PartNumber={0}, ETag={1}, LastModified={2}, Size={3}]", 
                PartNumber, ETag, LastModified, Size);
        }
        
        /// <summary>
        /// 获取包含Part标识号码和ETag值的<see cref="PartETag" />对象
        /// </summary>
        public PartETag PartETag
        {
            get { return new PartETag(PartNumber, ETag); }
        }
        
    }
}
