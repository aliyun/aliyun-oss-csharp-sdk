/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.IO;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// 指定上传某分块的请求。
    /// </summary>
    public class UploadPartRequest
    {
        /// <summary>
        /// 获取或者设置<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }
        
        /// <summary>
        /// 获取或者设置<see cref="OssObject" />的值。
        /// </summary>
        public string Key { get; private set; }
        
        /// <summary>
        /// 获取或设置上传Multipart上传事件的Upload ID。
        /// </summary>
        public string UploadId { get; private set; }
        
        /// <summary>
        /// 获取或设置返回上传分块（Part）的标识号码（Part Number）。
        /// 每一个上传分块（Part）都有一个标识它的号码（范围1~10000）。
        /// 对于同一个Upload ID，该号码不但唯一标识这一块数据，也标识了这块数据在整个文件中的相对位置。
        /// 如果你用同一个Part号码上传了新的数据，那么OSS上已有的这个号码的Part数据将被覆盖。
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
        /// 获取或设置包含上传分块内容的数据流。
        /// </summary>
        public Stream InputStream { get; set; }
        
        public UploadPartRequest(string bucketName, string key, string uploadId)
        {            
            BucketName = bucketName;
            Key = key;
            UploadId = uploadId;
        }
    }
}
