/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 指定列出指定Upload ID所属的所有已经上传成功Part的请求参数。
    /// </summary>
    public class ListPartsRequest
    {
        /// <summary>
        /// 获取<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }
        
        /// <summary>
        /// 获取或者设置<see cref="OssObject" />的值。
        /// </summary>
        public string Key { get; private set; }
        
        /// <summary>
        /// 获取或者设置响应中的最大Part数目。
        /// </summary>
        public int? MaxParts { get; set; }
        
        /// <summary>
        /// 获取或者设置List的起始位置，只有Part Number数目大于该参数的Part会被列出。
        /// </summary>
        public int? PartNumberMarker { get; set; }

        /// <summary>
        /// 获取encoding-type的值
        /// </summary>
        public string EncodingType { get; set; }

        /// <summary>
        /// 获取或者设置UploadId。
        /// </summary>
        public string UploadId { get; private set; }

        /// <summary>
        /// 构造一个新的<see cref="ListPartsRequest" />实例。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />所在<see cref="Bucket" />的名称。</param>
        /// <param name="key"><see cref="OssObject" />的<see cref="P:OssObject.Key" />。</param>
        /// <param name="uploadId"><see cref="InitiateMultipartUploadResult.UploadId"/></param>
        public ListPartsRequest(string bucketName, string key, string uploadId)
        {            
            BucketName = bucketName;
            Key = key;
            UploadId = uploadId;
            EncodingType = Util.HttpUtils.UrlEncodingType;
        }
    }
}
