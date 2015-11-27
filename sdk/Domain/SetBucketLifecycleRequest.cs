/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// 设置存储空间生命周期的请求
    /// </summary>
    public class SetBucketLifecycleRequest
    {
        private IList<LifecycleRule> _lifecycleRules = new List<LifecycleRule>(); 

        /// <summary>
        /// 获取<see cref="OssObject" />所在<see cref="Bucket" />的名称。
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Lifecycle规则列表，每个bucket最多允许1000条规则。
        /// </summary>
        public IList<LifecycleRule> LifecycleRules 
        {
            get { return ((List<LifecycleRule>)_lifecycleRules).AsReadOnly(); }
            set
            {
                if (value == null)
                    throw new ArgumentException("LifecycleRule list should not be null.");
                if (value.Count > OssUtils.LifecycleRuleLimit)
                    throw new ArgumentException("One bucket not allow exceed one thousand items of LifecycleRules.");
                _lifecycleRules = value;
            }
        }

        /// <summary>
        /// 构造一个新的<see cref="SetBucketLifecycleRequest" />实例。
        /// </summary>
        /// <param name="bucketName"><see cref="OssObject" />所在<see cref="Bucket" />的名称</param>
        public SetBucketLifecycleRequest(string bucketName) 
        {
            BucketName = bucketName;
        }

        /// <summary>
        /// 添加一条LifecycleRule。
        /// </summary>
        /// <param name="lifecycleRule"></param>
        public void AddLifecycleRule(LifecycleRule lifecycleRule)
        {
            if (lifecycleRule == null)
                throw new ArgumentException("lifecycleRule should not be null or empty.");

            if (_lifecycleRules.Count >= OssUtils.LifecycleRuleLimit)
                throw new ArgumentException("One bucket not allow exceed one thousand item of LifecycleRules.");

            if ((!lifecycleRule.ExpirationTime.HasValue && !lifecycleRule.ExpriationDays.HasValue)
                || (lifecycleRule.ExpirationTime.HasValue && lifecycleRule.ExpriationDays.HasValue))
            {
                throw new ArgumentException("Only one expiration property should be specified.");
            }

            _lifecycleRules.Add(lifecycleRule);
        }

    }

}
