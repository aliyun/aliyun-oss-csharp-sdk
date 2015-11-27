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
    /// 定义了可以被授权的一组OSS用户。
    /// </summary>
    public sealed class GroupGrantee : IGrantee
    {
        private readonly string _identifier;

        /// <summary>
        /// 获取被授权者的标识。
        /// </summary>
        /// <remarks>
        /// 不支持set操作，否则会抛出<see cref="NotSupportedException" />。
        /// </remarks>
        public string Identifier
        {
            get { return _identifier; }
            set { throw new NotSupportedException(); }
        }

        private static readonly GroupGrantee _allUsers =
            new GroupGrantee("http://oss.service.aliyun.com/acl/group/ALL_USERS");

        /// <summary>
        /// 表示为OSS的<see cref="Bucket" />或<see cref="OssObject" />指定匿名访问的权限。
        /// 任何用户都可以根据被授予的权限进行访问。
        /// </summary>
        public static GroupGrantee AllUsers
        {
            get { return _allUsers; }
        }

        /// <summary>
        /// 设置授权者标识
        /// </summary>
        /// <param name="identifier">授权者标识</param>
        private GroupGrantee(string identifier)
        {
            _identifier = identifier;
        }

        /// <summary>
        /// 判断两个<see cref="GroupGrantee"/>是否相等
        /// </summary>
        /// <param name="obj">需要判断的另一个<see cref="GroupGrantee"/></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var grantee = obj as GroupGrantee;
            if (grantee == null)
                return false;

            return grantee.Identifier == Identifier;
        }

        /// <summary>
        /// 获取hash code值
        /// </summary>
        /// <returns>hash code值</returns>
        public override int GetHashCode()
        {
            return ("[GroupGrantee ID=" + Identifier + "]").GetHashCode();
        }

    }
}
