/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections;
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
        /// Gets or sets the created before date.
        /// </summary>
        /// <value>The created before date.</value>
        public DateTime? CreatedBeforeDate { get; set;}

        /// <summary>
        /// Gets or sets the transition.
        /// </summary>
        /// <value>The transition.</value>
        public LifeCycleTransition[] Transitions { get; set; }

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

            if (this.CreatedBeforeDate != obj.CreatedBeforeDate) return false;

            if (this.Status != obj.Status) return false;

            if (this.AbortMultipartUpload == null && obj.AbortMultipartUpload != null) return false;

            if (this.AbortMultipartUpload != null && !this.AbortMultipartUpload.Equals(obj.AbortMultipartUpload))
            {
                return false;
            }

            if (this.Transitions == null && obj.Transitions != null
                || this.Transitions != null && obj.Transitions == null)
            { 
                return false; 
            }

            if (this.Transitions != null && obj.Transitions != null)
            {
                if (this.Transitions.Length != obj.Transitions.Length) return false;

                for (int i = 0; i < this.Transitions.Length; i++)
                {
                    if (!this.Transitions[i].Equals(obj.Transitions[i]))
                    {
                        return false;
                    }
                }
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
            if (Transitions != null)
            {
                for (int i = 0; i < Transitions.Length; i++)
                {
                    if (Transitions[i].LifeCycleExpiration != null)
                    {
                        ret &= Transitions[i].LifeCycleExpiration.Validate();
                    }
                }
            }

            if (AbortMultipartUpload != null)
            {
                ret &= AbortMultipartUpload.Validate();
            }

            ret &= (ExpriationDays != null && CreatedBeforeDate == null || ExpriationDays == null && CreatedBeforeDate != null);

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
            public DateTime? CreatedBeforeDate { get; set; }

            /// <summary>
            /// Validate this instance.
            /// </summary>
            /// <returns>The validate result.</returns>
            public bool Validate()
            {
                return (Days != null && CreatedBeforeDate == null) || (Days == null && CreatedBeforeDate != null);
            }

            public bool Equals(LifeCycleExpiration obj)
            {
                if (ReferenceEquals(this, obj)) return true;

                if (obj == null) return false;

                return this.Days == obj.Days && this.CreatedBeforeDate == obj.CreatedBeforeDate;
            }
        }

        /// <summary>
        /// Life cycle transition.
        /// </summary>
        public class LifeCycleTransition : IEquatable<LifeCycleTransition>
        {
            private LifeCycleExpiration lifeCycleExpiration = new LifeCycleExpiration();

            /// <summary>
            /// Gets or sets the life cycle expiration.
            /// </summary>
            /// <value>The life cycle expiration.</value>
            public LifeCycleExpiration LifeCycleExpiration
            {
                get
                {
                    return lifeCycleExpiration;
                }
            }

            /// <summary>
            /// Gets or sets the storage class.
            /// </summary>
            /// <value>The storage class.</value>
            public StorageClass StorageClass { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="Aliyun.OSS.LifecycleRule.LifeCycleTransition"/> is equal to
            /// the current <see cref="T:Aliyun.OSS.LifecycleRule.LifeCycleTransition"/>.
            /// </summary>
            /// <param name="transition">The <see cref="Aliyun.OSS.LifecycleRule.LifeCycleTransition"/> to compare with the current <see cref="T:Aliyun.OSS.LifecycleRule.LifeCycleTransition"/>.</param>
            /// <returns><c>true</c> if the specified <see cref="Aliyun.OSS.LifecycleRule.LifeCycleTransition"/> is equal to the
            /// current <see cref="T:Aliyun.OSS.LifecycleRule.LifeCycleTransition"/>; otherwise, <c>false</c>.</returns>
            public bool Equals(LifeCycleTransition transition)
            {
                if (transition == null) return false;

                if (this.StorageClass != transition.StorageClass) return false;
                    
                if (LifeCycleExpiration == null)
                {
                    return transition.LifeCycleExpiration == null;
                }

                return LifeCycleExpiration.Equals(transition.LifeCycleExpiration);
            }
        }
    }
}
