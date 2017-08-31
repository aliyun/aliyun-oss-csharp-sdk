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
    public class LifecycleRule
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
    }
}
