/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Aliyun.OSS
{
    /// <summary>
    /// The bucket information class
    /// </summary>
    [XmlRoot("BucketInfo")]
    public class BucketInfo
    {
        /// this is to map the XML structure like below: 
        /// <BucketInfo>
        ///     <Bucket>
        ///          ..
        ///     </Bucket>
        /// </BucketInfo>
        [XmlElement("Bucket")]
        public BucketInfoEntry Bucket { get; set; }
    }

    public class BucketInfoEntry
    {
        /// <summary>
        /// Bucket location getter/setter
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Bucket name getter/setter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Bucket <see cref="Owner" /> getter/setter
        /// </summary>
        public Owner Owner { get; set; }

        /// <summary>
        /// Bucket creation time getter/setter
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the extranet endpoint.
        /// </summary>
        /// <value>The extranet endpoint.</value>
        public string ExtranetEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the intranet endpoint.
        /// </summary>
        /// <value>The intranet endpoint.</value>
        public string IntranetEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the storage class.
        /// </summary>
        /// <value>The storage class.</value>
        public StorageClass StorageClass { get; set; }

        /// <summary>
        /// Gets or sets the access control list.
        /// </summary>
        /// <value>The access control list.</value>
        public BucketACL AccessControlList { get; set; }

        /// <summary>
        /// Gets or sets the disaster recovery.
        /// </summary>
        /// <value>The access control list.</value>
        public DataRedundancyType DataRedundancyType { get; set; }

        /// <summary>
        /// Gets or sets server-side encryption rule.
        /// </summary>
        /// <value>The access control list.</value>
        public BucketServerSideEncryptionRule ServerSideEncryptionRule { get; set; }

        /// <summary>
        /// Gets or sets versioning status.
        /// </summary>
        /// <value>bucket versioning status.</value>
        public VersioningStatus Versioning { get; set; }

        /// <summary>
        /// Creats a new <see cref="Bucket" /> instance with the specified name.
        /// </summary>
        /// <param name="name">Bucket name</param>
        internal BucketInfoEntry(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Aliyun.OSS.Bucket"/> class.
        /// </summary>
        internal BucketInfoEntry() {}

        /// <summary>
        /// Returns the bucket's serialization information in string.
        /// </summary>
        /// <returns>The serialization information in string</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                                 "OSS Bucket [Name={0}], [Location={1}] [Owner={2}], [CreationTime={3}]",
                                 Name, Location, Owner, CreationDate);
        }

        public class BucketACL
        {
            public CannedAccessControlList Grant { get; set; }
        }

        [XmlRoot("ServerSideEncryptionRule")]
        public class BucketServerSideEncryptionRule
        {
            [XmlElement("SSEAlgorithm")]
            public string SSEAlgorithm { get; set; }

            [XmlElement("KMSMasterKeyID")]
            public string KMSMasterKeyID { get; set; }
        }
    }
}
