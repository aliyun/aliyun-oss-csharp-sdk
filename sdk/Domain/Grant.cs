/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;

namespace Aliyun.OSS
{
    /// <summary>
    /// The access control grant class definition
    /// </summary>
    public class Grant {

        /// <summary>
        /// The grantee instance
        /// </summary>
        public IGrantee Grantee { get; private set; }

        /// <summary>
        /// The granted permission
        /// </summary>
        public Permission Permission { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Grant" />.
        /// </summary>
        /// <param name="grantee">the grantee instance----cannot be null</param>
        /// <param name="permission">the permission instance</param>
        public Grant(IGrantee grantee, Permission permission)
        {
            if (grantee == null)
                throw new ArgumentNullException("grantee");

            Grantee = grantee;
            Permission = permission;
        }

        /// <summary>
        /// Checks if two <see cref="Grant" /> instances equal.
        /// </summary>
        /// <param name="obj">The other grant instance to compare with</param>
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
        /// Gets the hash code
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return (Grantee.Identifier + ":" + Permission).GetHashCode();
        }

    }
}
