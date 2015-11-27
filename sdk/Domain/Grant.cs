/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;

namespace Aliyun.OSS
{
    /// <summary>
    /// 访问控制的授权信息。
    /// </summary>
    public class Grant {

        /// <summary>
        /// 获取被授权者信息。
        /// </summary>
        public IGrantee Grantee { get; private set; }

        /// <summary>
        /// 获取被授权的权限。
        /// </summary>
        public Permission Permission { get; private set; }

        /// <summary>
        /// 构造一个新的<see cref="Grant" />实体。
        /// </summary>
        /// <param name="grantee">被授权者。目前只支持<see cref="GroupGrantee.AllUsers" />。</param>
        /// <param name="permission">权限。</param>
        public Grant(IGrantee grantee, Permission permission)
        {
            if (grantee == null)
                throw new ArgumentNullException("grantee");

            Grantee = grantee;
            Permission = permission;
        }

        /// <summary>
        /// 判断两个<see cref="Grant" />是否相等
        /// </summary>
        /// <param name="obj">需要比较的<see cref="Grant" /></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            var g = obj as Grant;
            if (g == null)
                return false;

            return Grantee.Identifier== g.Grantee.Identifier && 
                Permission == g.Permission;
        }

        /// <summary>
        /// 获取HashCode值
        /// </summary>
        /// <returns>hash code值</returns>
        public override int GetHashCode()
        {
            return (Grantee.Identifier + ":" + Permission).GetHashCode();
        }

    }
}
