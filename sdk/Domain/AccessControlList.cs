/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// The class defines "Access control list"(ACL).
    /// It contains a list of <see cref="Grant"/> instances, each specifies a <see cref="IGrantee" /> and
    /// a <see cref="Permission" />.
    /// </summary>
    public class AccessControlList : GenericResult
    {

        private readonly Dictionary<Grant, bool> _grants = new Dictionary<Grant, bool>();

        /// <summary>
        /// Gets the iterator of <see cref="Grant" /> list.
        /// </summary>
        [Obsolete("Use ACL instead")]
        public IEnumerable<Grant> Grants
        {
            get { return _grants.Keys; }
        }

        /// <summary>
        /// Owner getter and setter
        /// </summary>
        public Owner Owner { get; internal set; }

        /// <summary>
        /// ACL getter or setter
        /// </summary>
        public CannedAccessControlList ACL { get; internal set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal AccessControlList()
        { }

        /// <summary>
        /// Grants permission to a <see cref="IGrantee" /> instance with specified <see cref="Permission" />.
        /// Currently the supported grantee is <see cref="GroupGrantee.AllUsers" />.
        /// </summary>
        /// <param name="grantee">The grantee</param>
        /// <param name="permission">The permission</param>
        internal void GrantPermission(IGrantee grantee, Permission permission)
        {
            if (grantee == null)
                throw new ArgumentNullException("grantee");

            _grants.Add(new Grant(grantee, permission), true);
        }
        
        /**
         * Revoke all permissions on a specific grantee.
         * @param grantee
         *           The grantee, currently only <see cref="GroupGrantee.AllUsers" /> is supported.
         */
        /// <summary>
        /// Invoke the <see cref="IGrantee" /> instance's all permissions.
        /// </summary>
        /// <param name="grantee">The grantee instanc</param>
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
        /// Return the string that has the owner and ACL list information.
        /// </summary>
        /// <returns>The serialized information in a string</returns>
        public override String ToString() {
            return string.Format(CultureInfo.InvariantCulture,
                "[AccessControlList: Owner={0}, ACL={1}]", Owner, ACL);
        }
    }
}