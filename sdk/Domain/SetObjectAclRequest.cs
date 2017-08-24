/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set the object ACL.
    /// </summary>
    public class SetObjectAclRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the object key.
        /// </summary>
        public string Key { get; private set; }   

        /// <summary>
        /// Gets the ACL.
        /// </summary>
        public CannedAccessControlList ACL { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="SetObjectAclRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">object key</param>
        /// <param name="acl">access control list</param>
        public SetObjectAclRequest(string bucketName, string key, CannedAccessControlList acl) 
        {
            BucketName = bucketName;
            Key = key;
            ACL = acl;
        }
    }

}
