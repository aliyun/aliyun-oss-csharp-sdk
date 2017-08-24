/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;

namespace Aliyun.OSS
{
    /// <summary>
    /// It defines a group of user that could be granted with permission.
    /// </summary>
    public sealed class GroupGrantee : IGrantee
    {
        private readonly string _identifier;

        /// <summary>
        /// The grantee's identifier.
        /// </summary>
        /// <remarks>
        /// Only supports gets operation. Calling the setter will trigger <see cref="NotSupportedException" />.
        /// </remarks>
        public string Identifier
        {
            get { return _identifier; }
            set { throw new NotSupportedException(); }
        }

        private static readonly GroupGrantee _allUsers =
            new GroupGrantee("http://oss.service.aliyun.com/acl/group/ALL_USERS");

        /// <summary>
        /// AllUsers means the <see cref="Bucket" /> or <see cref="OssObject" /> could be accessed by anonymous users.
        /// That is all users could access the resource.
        /// </summary>
        public static GroupGrantee AllUsers
        {
            get { return _allUsers; }
        }

        /// <summary>
        /// Sets the identifier.
        /// </summary>
        /// <param name="identifier">the grantee's Id</param>
        private GroupGrantee(string identifier)
        {
            _identifier = identifier;
        }

        /// <summary>
        /// Checks if two <see cref="GroupGrantee"/> instances equal
        /// </summary>
        /// <param name="obj">The other instance to compare with</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var grantee = obj as GroupGrantee;
            if (grantee == null)
                return false;

            return grantee.Identifier == Identifier;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return ("[GroupGrantee ID=" + Identifier + "]").GetHashCode();
        }

    }
}
