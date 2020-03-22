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
        /// Gets or sets the expired object delete marker.
        /// </summary>
        /// <value>The expired object delete marker.</value>
        public bool? ExpiredObjectDeleteMarker { get; set; }

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
        /// Gets or sets the object tags.
        /// </summary>
        /// <value>The object tags.</value>
        public Tag[] Tags { get; set; }

        /// <summary>
        /// Gets or sets the noncurrent version expiration.
        /// </summary>
        /// <value>The noncurrent version expiration.</value>
        public LifeCycleNoncurrentVersionExpiration NoncurrentVersionExpiration { get; set; }

        /// <summary>
        /// Gets or sets the noncurrent version transition.
        /// </summary>
        /// <value>The noncurrent version transition.</value>
        public LifeCycleNoncurrentVersionTransition[] NoncurrentVersionTransitions { get; set; }

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

            if (this.ExpiredObjectDeleteMarker != obj.ExpiredObjectDeleteMarker) return false;

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

            //tags
            if (this.Tags == null && obj.Tags != null
                || this.Tags != null && obj.Tags == null)
            {
                return false;
            }

            if (this.Tags != null && obj.Tags != null)
            {
                if (this.Tags.Length != obj.Tags.Length) return false;

                for (int i = 0; i < this.Tags.Length; i++)
                {
                    if (!this.Tags[i].Equals(obj.Tags[i]))
                    {
                        return false;
                    }
                }
            }

            //NoncurrentVersionExpiration
            if (this.NoncurrentVersionExpiration == null && obj.NoncurrentVersionExpiration != null) return false;

            if (this.NoncurrentVersionExpiration != null && !this.NoncurrentVersionExpiration.Equals(obj.NoncurrentVersionExpiration))
            {
                return false;
            }

            //NoncurrentVersionTransitions
            if (this.NoncurrentVersionTransitions == null && obj.NoncurrentVersionTransitions != null
                 || this.NoncurrentVersionTransitions != null && obj.NoncurrentVersionTransitions == null)
            {
                return false;
            }

            if (this.NoncurrentVersionTransitions != null && obj.NoncurrentVersionTransitions != null)
            {
                if (this.NoncurrentVersionTransitions.Length != obj.NoncurrentVersionTransitions.Length) return false;

                for (int i = 0; i < this.NoncurrentVersionTransitions.Length; i++)
                {
                    if (!this.NoncurrentVersionTransitions[i].Equals(obj.NoncurrentVersionTransitions[i]))
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

            int flag = 0;
            if (ExpriationDays != null)
            {
                flag++;
            }
            if (CreatedBeforeDate != null)
            {
                flag++;
            }
            if (ExpiredObjectDeleteMarker != null)
            {
                flag++;
            }
            ret &= (flag <= 1);

            return ret;
        }

        internal bool HasExpriation()
        {
            return ExpriationDays.HasValue || CreatedBeforeDate.HasValue || ExpiredObjectDeleteMarker.HasValue;
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

                return this.Days == obj.Days &&  this.CreatedBeforeDate == obj.CreatedBeforeDate;
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

        /// <summary>
        /// Life cycle noncurrent version expiration.
        /// </summary>
        public class LifeCycleNoncurrentVersionExpiration : IEquatable<LifeCycleNoncurrentVersionExpiration>
        {
            /// <summary>
            /// Gets or sets the noncurrent days.
            /// </summary>
            /// <value>The noncurrent days.</value>
            public int NoncurrentDays { get; set; }

            public bool Equals(LifeCycleNoncurrentVersionExpiration obj)
            {
                if (ReferenceEquals(this, obj)) return true;

                if (obj == null) return false;

                return this.NoncurrentDays == obj.NoncurrentDays;
            }
        }

        /// <summary>
        /// Life cycle noncurrent version transition.
        /// </summary>
        public class LifeCycleNoncurrentVersionTransition : IEquatable<LifeCycleNoncurrentVersionTransition>
        {
            /// <summary>
            /// Gets or sets the noncurrent days.
            /// </summary>
            /// <value>The noncurrent days.</value>
            public int NoncurrentDays { get; set; }

            /// <summary>
            /// Gets or sets the storage class.
            /// </summary>
            /// <value>The storage class.</value>
            public StorageClass StorageClass { get; set; }

            public bool Equals(LifeCycleNoncurrentVersionTransition obj)
            {
                if (ReferenceEquals(this, obj)) return true;

                if (obj == null) return false;

                return this.NoncurrentDays == obj.NoncurrentDays && this.StorageClass == obj.StorageClass;
            }
        }
    }
}
