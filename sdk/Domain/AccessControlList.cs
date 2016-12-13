/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Aliyun.OSS
{
    /// <summary>
    /// 表示OSS的访问控制列表（Access Control List， ACL），
    /// 包含了一组为指定被授权者<see cref="IGrantee" />
    /// 分配特定权限<see cref="Permission" />的集合。
    /// </summary>
    public class AccessControlList
    {

        private readonly Dictionary<Grant, bool> _grants = new Dictionary<Grant, bool>();

        /// <summary>
        /// 获取所有<see cref="Grant" />实例的枚举。
        /// </summary>
        [Obsolete("Use ACL instead")]
        public IEnumerable<Grant> Grants
        {
            get { return _grants.Keys; }
        }

        /// <summary>
        /// 获取或设置所有者。
        /// </summary>
        public Owner Owner { get; internal set; }

        /// <summary>
        /// 获取或设置权限。
        /// </summary>
        public CannedAccessControlList ACL { get; internal set; }

        /// <summary>
        /// 构造函数。
        /// </summary>
        internal AccessControlList()
        { }

        /// <summary>
        /// 为指定<see cref="IGrantee" />授予特定的<see cref="Permission" />。
        /// 目前只支持被授权者为<see cref="GroupGrantee.AllUsers" />。
        /// </summary>
        /// <param name="grantee">被授权者。</param>
        /// <param name="permission">被授予的权限。</param>
        internal void GrantPermission(IGrantee grantee, Permission permission)
        {
            if (grantee == null)
                throw new ArgumentNullException("grantee");

            _grants.Add(new Grant(grantee, permission), true);
        }
        
        /**
         * 取消指定{@link Grantee}已分配的所有权限。
         * @param grantee
         *           被授权者。目前只支持被授权者为{@link GroupGrantee#AllUsers}。
         */
        /// <summary>
        /// 取消指定<see cref="IGrantee" />已分配的所有权限。
        /// </summary>
        /// <param name="grantee">被授权者。</param>
        internal void RevokeAllPermissions(IGrantee grantee)
        {
            if (grantee == null)
                throw new ArgumentNullException("grantee");

            foreach (var e in _grants.Keys)
            {
                if (e.Grantee == grantee)
                {
                    _grants.Remove(e);
                }
            }
        }

        /// <summary>
        /// 返回该对象的字符串表示。
        /// </summary>
        /// <returns>对象的字符串表示形式</returns>
        public override String ToString() {
            return string.Format(CultureInfo.InvariantCulture,
                "[AccessControlList: Owner={0}, ACL={1}]", Owner, ACL);
        }
    }
}