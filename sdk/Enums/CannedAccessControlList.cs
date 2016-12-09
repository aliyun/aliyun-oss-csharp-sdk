/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// 表示一组常用的用户访问权限。
    /// <para>
    /// 这一组常用权限相当于给所有用户指定权限的快捷方法。
    /// </para>
    /// </summary>
    public enum CannedAccessControlList
    {
        /// <summary>
        /// 私有权限，有权限才可以读和写
        /// </summary>
        [StringValue("private")]
        Private = 0,

        /// <summary>
        /// 公共读，任何人都可以读，有权限才可以写
        /// </summary>
        [StringValue("public-read")]
        PublicRead,

        /// <summary>
        /// 公共读写，任何人都可以读和写
        /// </summary>
        [StringValue("public-read-write")]
        PublicReadWrite,

        /// <summary>
        /// 默认权限，仅用于文件，与存储空间的权限相同
        /// </summary>
        [StringValue("default")]
        Default
    }
}
