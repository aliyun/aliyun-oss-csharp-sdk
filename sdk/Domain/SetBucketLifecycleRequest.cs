/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// The request class of the operation to set the bucket's lifecycle configuration.
    /// </summary>
    public class SetBucketLifecycleRequest
    {
        private IList<LifecycleRule> _lifecycleRules = new List<LifecycleRule>(); 

        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets or sets the Lifecycle rule list.Each bucket can have up to 1000 rules.
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
        /// Creates a new intance of <see cref="SetBucketLifecycleRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public SetBucketLifecycleRequest(string bucketName) 
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException("bucketName");
            }

            BucketName = bucketName;
        }

        /// <summary>
        /// Adds a LifeCycle rule
        /// </summary>
        /// <param name="lifecycleRule"></param>
        public void AddLifecycleRule(LifecycleRule lifecycleRule)
        {
            if (lifecycleRule == null)
                throw new ArgumentException("lifecycleRule should not be null or empty.");

            if (_lifecycleRules.Count >= OssUtils.LifecycleRuleLimit)
                throw new ArgumentException("One bucket not allow exceed one thousand item of LifecycleRules.");

            if (!lifecycleRule.Validate())
            {
                throw new ArgumentException("Only one expiration property should be specified.");
            }

            _lifecycleRules.Add(lifecycleRule);
        }

    }
}
