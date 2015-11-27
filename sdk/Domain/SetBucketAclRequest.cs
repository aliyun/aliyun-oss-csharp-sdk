/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 存储空间ACL权限的请求
    /// </summary>
    public class SetBucketAclRequest
    {
        /// <summary>
        /// 获取<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// 获取用户访问权限。
        /// </summary>
        public CannedAccessControlList ACL { get; private set; }

        /// <summary>
        /// 构造一个新的<see cref="SetBucketAclRequest" />实例。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />所在<see cref="Bucket" />的名称。</param>
        /// <param name="acl">用户访问权限</param>
        public SetBucketAclRequest(string bucketName, CannedAccessControlList acl) 
        {
            BucketName = bucketName;
            ACL = acl;
        }
    }

}
