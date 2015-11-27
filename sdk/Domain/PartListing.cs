/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// 获取List Parts的结果.
    /// </summary>
    public class PartListing
    {
        private readonly IList<Part> _parts = new List<Part>();
        
        /// <summary>
        /// 获取Object所在的<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; internal set; }
               
        /// <summary>
        /// 获取<see cref="OssObject" />的名称。
        /// </summary>
        public string Key { get; internal set; }
        
        /// <summary>
        /// 获取请求参数<see cref="P:ListPartsRequest.UploadId" />的值。
        /// </summary>
        public string UploadId { get; internal set; }
        
        /// <summary>
        /// 获取请求参数<see cref="P:ListPartsRequest.PartNumberMarker" />的值。
        /// </summary>
        public int PartNumberMarker { get; internal set; }
        
        /// <summary>
        /// 如果本次没有返回全部结果，响应请求中将包含NextPartNumberMarker元素，
        /// 用于标明接下来请求的PartNumberMarker值。
        /// </summary>
        public int NextPartNumberMarker { get; internal set; }
        
        /// <summary>
        /// 获取请求参数<see cref="P:ListPartsRequest.MaxParts" />的值。
        /// </summary>
        public int MaxParts { get; internal set; }
        
        /// <summary>
        /// 标明是否本次返回的List Part结果列表被截断。
        /// “true”表示本次没有返回全部结果；“false”表示本次已经返回了全部结果。
        /// </summary>
        public bool IsTruncated { get; internal set; }
        
        /// <summary>
        /// 获取所有的Part
        /// </summary>
        public IEnumerable<Part> Parts 
        {
            get { return _parts; }
        }
        
        /// <summary>
        /// 增加<see cref="Part"/>分片信息
        /// </summary>
        /// <param name="part">分片信息</param>
        internal void AddPart(Part part)
        {
            _parts.Add(part);
        }

        /// <summary>
        /// 构造一个新的<see cref="PartListing" />实例。
        /// </summary>
        internal PartListing()
        { }
        
    }
}
