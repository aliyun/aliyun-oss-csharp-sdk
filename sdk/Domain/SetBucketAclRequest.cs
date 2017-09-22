/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set the bucket ACL.
    /// </summary>
    public class SetBucketAclRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the ACL
        /// </summary>
        public CannedAccessControlList ACL { get; private set; }

        /// <summary>
        /// Creates a instance of <see cref="SetBucketAclRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="acl">user acl</param>
        public SetBucketAclRequest(string bucketName, CannedAccessControlList acl) 
        {
            BucketName = bucketName;
            ACL = acl;
        }
    }

}
