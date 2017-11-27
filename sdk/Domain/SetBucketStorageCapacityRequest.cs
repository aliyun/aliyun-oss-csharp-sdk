/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set the bucket storage capacity
    /// </summary>
    public class SetBucketStorageCapacityRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// The bucket storage capacity
        /// </summary>
        public long StorageCapacity { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="SetBucketStorageCapacityRequest" />.
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />bucket name</param>
        /// <param name="storageCapacity">storage capacity</param>
        public SetBucketStorageCapacityRequest(string bucketName, long storageCapacity)
        {
            BucketName = bucketName;
            StorageCapacity = storageCapacity;
        }
    }
}
