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
    /// 获取MultipartUpload事件的信息。
    /// </summary>
    public class MultipartUpload
    {
        /// <summary>
        /// 获取Object的key。
        /// </summary>
        public string Key { get; internal set; }
        
        /// <summary>
        /// 获取上传Id。
        /// </summary>
        public string UploadId { get; internal set; }
        
        /// <summary>
        /// 获取Object的存储类别。
        /// </summary>
        public string StorageClass {get; internal set; }
        
        /// <summary>
        /// Multipart Upload事件初始化的时间。
        /// </summary>
        public DateTime Initiated {get; internal set;}
        
        internal MultipartUpload()
        { }
        
        /// <summary>
        /// 获取该实例的字符串表示。
        /// </summary>
        /// <returns>对象的字符串表示</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[MultipartUpload Key={0}, UploadId={1}]", Key, UploadId);
        }
    }
}
