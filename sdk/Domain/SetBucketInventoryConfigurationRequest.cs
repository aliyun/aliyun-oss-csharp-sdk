/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
using System.Collections.Generic;
namespace Aliyun.OSS
{
    public class OSSBucketDestination
    {
        public InventoryFormat Format { get; set; }

        public string AccountId { get; set; }

        public string RoleArn { get; set; }

        public string Bucket { get; set; }

        public string Prefix { get; set; }

        public string Encryption { get; set; }

        public bool IsEncryptionOSS { get; set; }

        public OSSBucketDestination(InventoryFormat format)
        {
            Format = format;
            Encryption = null;
            IsEncryptionOSS = false;
        }
    }
    /// <summary>
    /// The request class of the operation to set the Bucket InventoryConfiguration.
    /// </summary>
    public class SetBucketInventoryConfigurationRequest
    {
        /// <summary>
        /// Gets the bucket InventoryConfiguration
        /// </summary>
        public string Id { get; set; }

        public bool IsEnabled { get; set; }

        public string Prefix { get; set; }

        public OSSBucketDestination Destination { get; set; }

        public InventoryFrequency Schedule { get; set; }

        public InventoryIncludedObjectVersions IncludedObjectVersions { get; set; }

        private readonly IList<InventoryOptionalField> _optionalFields = new List<InventoryOptionalField>();

        public IList<InventoryOptionalField> OptionalFields
        {
            get { return _optionalFields; }
        }

        /// <summary>
        /// Gets the bucket Name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Creates a instance of <see cref="SetBucketInventoryConfigurationRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="policy">user Policy</param>
        public SetBucketInventoryConfigurationRequest(string bucketName, IList<InventoryOptionalField> optionalFields)
        {
            BucketName = bucketName;
            _optionalFields = optionalFields;
        }

    }
}
