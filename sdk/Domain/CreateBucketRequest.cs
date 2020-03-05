/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set the bucket's lifecycle configuration.
    /// </summary>
    public class CreateBucketRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// Gets the bucket StorageClass
        /// </summary>
        public StorageClass StorageClass { get; set; }

        /// <summary>
        /// Gets the bucket ACL
        /// </summary>
        public CannedAccessControlList ACL { get; set; }

        /// <summary>
        /// Gets the bucket DataRedundancyType
        /// </summary>
        public DataRedundancyType? DataRedundancyType { get; set; }

        /// <summary>
        /// Creates a new intance of <see cref="CreateBucketRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public CreateBucketRequest(string bucketName)
            :this(bucketName, StorageClass.Standard, CannedAccessControlList.Private)
        {
        }

        /// <summary>
        /// Creates a new intance of <see cref="CreateBucketRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="storageClass">the bucket storage class</param>
        /// <param name="acl">the bucket acl</param>
        public CreateBucketRequest(string bucketName, StorageClass storageClass, CannedAccessControlList acl)
        {
            BucketName = bucketName;
            StorageClass = storageClass;
            ACL = acl;
            DataRedundancyType = null;
        }
    }
}
