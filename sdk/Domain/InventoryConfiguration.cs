
using System.Collections.Generic;

namespace Aliyun.OSS
{
    /// <summary>
    /// The inventory filter class definition
    /// </summary>
    public class InventoryFilter
    {
        /// <summary>
        /// Gets or sets the prefix value
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="InventoryFilter" />.
        /// </summary>
        /// <param name="prefix">the prefix that an object must have to be included in the inventory results.</param>
        public InventoryFilter(string prefix)
        {
            Prefix = prefix;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InventoryFilter" />.
        /// </summary>
        public InventoryFilter()
        {
        }
    }

    /// <summary>
    /// The inventory schedule class definition
    /// </summary>
    public class InventorySchedule
    {
        /// <summary>
        /// Gets or sets the frequency value.
        /// </summary>
        public InventoryFrequency Frequency { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="InventorySchedule" />.
        /// </summary>
        /// <param name="frequency"> how frequently inventory results are produced.</param>
        public InventorySchedule(InventoryFrequency frequency)
        {
            Frequency = frequency;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InventorySchedule" />.
        /// </summary>
        public InventorySchedule()
        {
        }
    }

    /// <summary>
    /// The inventory SSE-OSS class definition
    /// </summary>
    public class InventorySSEOSS
    {
        /// <summary>
        /// Creates a new instance of <see cref="InventorySSEOSS" />.
        /// </summary>
        public InventorySSEOSS()
        {
        }
    }

    /// <summary>
    /// The inventory SSE-KMS class definition
    /// </summary>
    public class InventorySSEKMS
    {
        /// <summary>
        /// Gets or sets the KMS key id
        /// </summary>
        public string KeyId { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="InventorySSEKMS" />.
        /// </summary>
        /// <param name="keyId">the KMS key id used to encrypt the inventory contents.</param>
        public InventorySSEKMS(string keyId)
        {
            KeyId = keyId;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InventorySSEKMS" />.
        /// </summary>
        public InventorySSEKMS()
        {
        }
    }

    /// <summary>
    /// The inventory encryption class definition
    /// </summary>
    public class InventoryEncryption
    {
        private InventorySSEOSS _sSEOSS;
        private InventorySSEKMS _sSEKMS;

        /// <summary>
        /// Gets or sets the SSE-OSS encryption.
        /// </summary>
        public InventorySSEOSS SSEOSS
        {
            set { _sSEOSS = value; }
            get { return _sSEOSS; }
        }

        /// <summary>
        /// Gets or sets the SSE-KMS encryption.
        /// </summary>
        public InventorySSEKMS SSEKMS
        {
            set { _sSEKMS = value; }
            get { return _sSEKMS; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="InventoryEncryption" />.
        /// </summary>
        /// <param name="sSEOSS">specifies the use of SSE-OSS to encrypt delivered inventory results.</param>
        public InventoryEncryption(InventorySSEOSS sSEOSS)
        {
            SSEOSS = sSEOSS;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InventoryEncryption" />.
        /// </summary>
        /// <param name="sSEKMS">specifies the use of SSE-KMS to encrypt delivered inventory results.</param>
        public InventoryEncryption(InventorySSEKMS sSEKMS)
        {
            SSEKMS = sSEKMS;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InventoryEncryption" />.
        /// </summary>
        public InventoryEncryption()
        {
        }
    }

    /// <summary>
    /// The inventory destination for OSS bucket class definition
    /// </summary>
    public class InventoryOSSBucketDestination
    {
        /// <summary>
        /// Gets or sets the output format of the inventory results.
        /// </summary>
        public InventoryFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the account ID that owns the destination bucket.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the name of the role arn.
        /// </summary>
        public string RoleArn { get; set; }

        /// <summary>
        /// Gets or sets the bucket where inventory results will be published.
        /// </summary>
        public string Bucket { get; set; }

        /// <summary>
        /// Gets or sets the prefix that is prepended to all inventory results.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the type of server-side encryption used to encrypt the inventory results.
        /// </summary>
        public InventoryEncryption Encryption { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="InventoryOSSBucketDestination" />.
        /// </summary>
        public InventoryOSSBucketDestination()
        {
        }
    }

    /// <summary>
    /// The inventory destination class definition
    /// </summary>
    public class InventoryDestination
    {
        /// <summary>
        /// Gets or sets the OSS bucket information.
        /// </summary>
        public InventoryOSSBucketDestination OSSBucketDestination { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="InventoryDestination" />.
        /// </summary>
        public InventoryDestination()
        {
        }
    }

    /// <summary>
    /// The inventory configuration class definition
    /// </summary>
    public class InventoryConfiguration
    {
        private IList<InventoryOptionalField> _optionalFields = new List<InventoryOptionalField>();

        /// <summary>
        /// Gets or sets the ID used to identify the inventory configuration.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the status of the inventory.
        /// If set to true, an inventory list is generated.
        /// If set to false, no inventory list is generated.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the inventory filter.
        /// The inventory only includes objects that meet the filter's criteria.
        /// </summary>
        public InventoryFilter Filter { get; set; }

        /// <summary>
        /// Gets or sets information about where to publish the inventory results.
        /// </summary>
        public InventoryDestination Destination { get; set; }

        /// <summary>
        /// Gets or sets the schedule for generating inventory results.
        /// </summary>
        public InventorySchedule Schedule { get; set; }

        /// <summary>
        /// Gets or sets object versions to include in the inventory list.
        /// </summary>
        public InventoryIncludedObjectVersions IncludedObjectVersions { get; set; }

        /// <summary>
        /// Gets or sets the optional fields that are included in the inventory result.
        /// </summary>
        public IList<InventoryOptionalField>  OptionalFields
        {
            get { return _optionalFields; }
            set { _optionalFields = value; }
        }
    }
}
