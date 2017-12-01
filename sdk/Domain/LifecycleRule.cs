/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;

namespace Aliyun.OSS
{
    /// <summary>
    /// Lifecycle rule status
    /// </summary>
    public enum RuleStatus
    {
        /// <summary>
        /// Enable the rule
        /// </summary>
        Enabled,

        /// <summary>
        /// Disable the rule
        /// </summary>
        Disabled
    };

    /// <summary>
    /// Lifecycle rule definition class, which represents one rule of Lifecycle
    /// </summary>
    public class LifecycleRule : IEquatable<LifecycleRule>
    {
        /// <summary>
        /// Gets or sets the rule Id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the prefix of the files the rule applied to. 
        /// If it's null, then the rule is applied to the whole bucket.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// The rule status
        /// </summary>
        public RuleStatus Status { get; set; }

        /// <summary>
        /// The expiration days.
        /// </summary>
        public int? ExpriationDays { get; set; }

        /// <summary>
        /// The expiration time.
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// Gets or sets the transition.
        /// </summary>
        /// <value>The transition.</value>
        public LifeCycleTransition Transition { get; set; }

        /// <summary>
        /// Gets or sets the abort multipart upload.
        /// </summary>
        /// <value>The abort multipart upload.</value>
        public LifeCycleExpiration AbortMultipartUpload { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="Aliyun.OSS.LifecycleRule"/> is equal to the current <see cref="T:Aliyun.OSS.LifecycleRule"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Aliyun.OSS.LifecycleRule"/> to compare with the current <see cref="T:Aliyun.OSS.LifecycleRule"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Aliyun.OSS.LifecycleRule"/> is equal to the current
        /// <see cref="T:Aliyun.OSS.LifecycleRule"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(LifecycleRule obj)
        {
            if (ReferenceEquals(this, obj)) return true;

            if (obj == null) return false;

            if (!this.ID.Equals(obj.ID)) return false;

            if (!this.Prefix.Equals(obj.Prefix)) return false;

            if (this.ExpriationDays != obj.ExpriationDays) return false;

            if (this.ExpirationTime != obj.ExpirationTime) return false;

            if (this.Status != obj.Status) return false;

            if (this.AbortMultipartUpload == null && obj.AbortMultipartUpload != null) return false;

            if (this.AbortMultipartUpload != null && !this.AbortMultipartUpload.Equals(obj.AbortMultipartUpload))
            {
                return false;
            }

            if (this.Transition == null && obj.Transition != null) return false;
            if (this.Transition != null && !this.Transition.Equals(obj.Transition))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate this instance.
        /// </summary>
        /// <returns>The validate result.</returns>
        internal bool Validate()
        {
            bool ret = true;
            if (Transition != null)
            {
                ret &= Transition.Validate();
            }

            if (AbortMultipartUpload != null)
            {
                ret &= AbortMultipartUpload.Validate();
            }

            ret &= (ExpriationDays != null && ExpirationTime == null || ExpriationDays == null && ExpirationTime != null);

            return ret;
        }

        /// <summary>
        /// Life cycle expiration.
        /// </summary>
        public class LifeCycleExpiration : IEquatable<LifeCycleExpiration>
        {
            /// <summary>
            /// Gets or sets the days.
            /// </summary>
            /// <value>The days.</value>
            public int? Days { get; set; }

            /// <summary>
            /// Gets or sets the expiration time.
            /// </summary>
            /// <value>The expiration time.</value>
            public DateTime? ExpirationTime { get; set; }

            /// <summary>
            /// Validate this instance.
            /// </summary>
            /// <returns>The validate result.</returns>
            public bool Validate()
            {
                return Days != null && ExpirationTime == null || Days == null && ExpirationTime != null;
            }

            public bool Equals(LifeCycleExpiration obj)
            {
                if (ReferenceEquals(this, obj)) return true;

                if (obj == null) return false;

                return this.Days == obj.Days && this.ExpirationTime == obj.ExpirationTime;
            }
        }

        /// <summary>
        /// Life cycle transition.
        /// </summary>
        public class LifeCycleTransition : LifeCycleExpiration, IEquatable<LifeCycleTransition>
        {
            /// <summary>
            /// Gets or sets the storage class.
            /// </summary>
            /// <value>The storage class.</value>
            public StorageClass StorageClass { get; set; }

            public bool Equals(LifeCycleTransition transition)
            {
                return base.Equals(transition) && this.StorageClass == transition.StorageClass;
            }
        }
    }
}
