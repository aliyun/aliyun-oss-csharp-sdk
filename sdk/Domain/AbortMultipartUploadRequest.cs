/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 指定中止Multipart Upload事件的请求参数
    /// </summary>
    public class AbortMultipartUploadRequest
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
        /// 获取或者设置需要取消的UploadId。
        /// </summary>
        public string UploadId { get; private set; }

        /// <summary>
        /// 通过bucket name， object key和upload id构造AbortMultipartUploadRequest对象
        /// </summary>
        /// <param name="bucketName">bucket的名字</param>
        /// <param name="key">object的名字</param>
        /// <param name="uploadId">本次要取消的upload的id，由<see cref="InitiateMultipartUploadResult"/>中获取</param>
        public AbortMultipartUploadRequest(string bucketName, string key, string uploadId)
        {
            BucketName = bucketName;
            Key = key;
            UploadId = uploadId;
        }
    }
}
