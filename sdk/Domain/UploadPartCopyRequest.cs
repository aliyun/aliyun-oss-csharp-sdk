/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */
 
using System;
using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// 指定以某一Object作为上传某分块数据源的请求。
    /// </summary>
    public class UploadPartCopyRequest
    {
        private readonly IList<string> _matchingETagConstraints = new List<string>();
        private readonly IList<string> _nonmatchingETagConstraints = new List<string>();

        /// <summary>
        /// 获取或者设置<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string TargetBucket { get; private set; }
        
        /// <summary>
        /// 获取或者设置<see cref="OssObject" />的值。
        /// </summary>
        public string TargetKey { get; private set; }
        
        /// <summary>
        /// 获取或设置上传Multipart上传事件的Upload ID。
        /// </summary>
        public string UploadId { get; private set; }
        
        /// <summary>
        /// 获取或设置返回上传分块（Part）的标识号码（Part Number）。
        /// 每一个上传分块（Part）都有一个标识它的号码（范围1~10000）。
        /// 对于同一个Upload ID，该号码不但唯一标识这一块数据，也标识了这块数据在整个文件中的
        /// 相对位置。如果你用同一个Part号码上传了新的数据，那么OSS上已有的这个号码的Part数据将被覆盖。
        /// </summary>
        public int? PartNumber { get; set; }
        
        /// <summary>
        /// 获取或设置返回分块（Part）数据的字节数。
        /// 除最后一个Part外，其他Part最小为5MB。
        /// </summary>
        public long? PartSize { get; set; }
        
        /// <summary>
        /// 获取或设置分块（Part）数据的MD5校验值。
        /// </summary>
        public string Md5Digest { get; set; }

        /// <summary>
        /// 获取或设置拷贝源Object。
        /// </summary>
        public string SourceKey { get; set; }

        /// <summary>
        /// 获取或设置拷贝源Bucket。
        /// </summary>
        public string SourceBucket { get; set; }

        /// <summary>
        /// 获取或设置拷贝源Object的起始位置。
        /// </summary>
        public long? BeginIndex { get; set; }

        /// <summary>
        /// 如果源Object的ETAG值和用户提供的ETAG相等，则执行拷贝操作；
        /// 否则返回412 HTTP错误码（预处理失败）。
        /// </summary>        
        public IList<string> MatchingETagConstraints
        {
            get { return _matchingETagConstraints; }
        }

        /// <summary>
        /// 如果源Object的ETAG值和用户提供的ETAG不相等，则执行拷贝操作；
        /// 否则返回412 HTTP错误码（预处理失败）。
        /// </summary>       
        public IList<string> NonmatchingETagConstraints
        {
            get { return _nonmatchingETagConstraints; }
        }

        /// <summary>
        /// 如果传入参数中的时间等于或者晚于文件实际修改时间，则正常传输文件，并返回200 OK；
        /// 否则返回412 precondition failed错误
        /// </summary>
        public DateTime? UnmodifiedSinceConstraint { get; set; }

        /// <summary>
        /// 如果源Object自从用户指定的时间以后被修改过，则执行拷贝操作；
        /// 否则返回412 HTTP错误码（预处理失败）。    
        /// </summary>   
        public DateTime? ModifiedSinceConstraint { get; set; }

        public UploadPartCopyRequest(string targetBucket, string targetKey, string sourceBucket, 
            string sourceKey, string uploadId)
        {
            TargetBucket = targetBucket;
            TargetKey = targetKey;
            SourceBucket = sourceBucket;
            SourceKey = sourceKey;
            UploadId = uploadId;
        }
    }
}
