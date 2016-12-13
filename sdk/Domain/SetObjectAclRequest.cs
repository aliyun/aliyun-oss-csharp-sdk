/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// 设置文件ACL权限的请求
    /// </summary>
    public class SetObjectAclRequest
    {
        /// <summary>
        /// 获取或获取<see cref="OssObject" />所在的<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// 获取或者设置Object的Key。
        /// </summary>
        public string Key { get; private set; }   

        /// <summary>
        /// 获取用户访问权限。
        /// </summary>
        public CannedAccessControlList ACL { get; private set; }

        /// <summary>
        /// 构造一个新的<see cref="SetObjectAclRequest" />实例。
        /// </summary>
        /// <param name="bucketName">bucket的名字</param>
        /// <param name="key">object的名字</param>
        /// <param name="acl">用户访问权限</param>
        public SetObjectAclRequest(string bucketName, string key, CannedAccessControlList acl) 
        {
            BucketName = bucketName;
            Key = key;
            ACL = acl;
        }
    }

}
