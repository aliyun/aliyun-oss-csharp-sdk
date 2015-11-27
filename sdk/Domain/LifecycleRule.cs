/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;

namespace Aliyun.OSS
{
    /// <summary>
    /// Lifecycle规则状态。
    /// </summary>
    public enum RuleStatus
    {
        /// <summary>
        /// 启用规则
        /// </summary>
        Enabled,

        /// <summary>
        /// 禁用规则
        /// </summary>
        Disabled
    };

    /// <summary>
    /// 表示一条Lifecycle规则。
    /// </summary>
    public class LifecycleRule
    {
        /// <summary>
        /// 当前规则的标识符
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 当前规则适用的前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 当前规则的状态<see cref="RuleStatus"/>
        /// </summary>
        public RuleStatus Status { get; set; }

        /// <summary>
        /// 当前规则的有效天数
        /// </summary>
        public int? ExpriationDays { get; set; }

        /// <summary>
        /// 当前规则的有效时间
        /// </summary>
        public DateTime? ExpirationTime { get; set; }
    }
}
