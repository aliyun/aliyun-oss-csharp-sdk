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
    /// The request class of the operation to set the bucket CORS
    /// </summary>
    public class SetBucketCorsRequest
    {
        private IList<CORSRule> _corsRules = new List<CORSRule>(); 

        /// <summary>
        /// Gets bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets or sets the CORS list. Each bucket can have up to 10 rules.
        /// </summary>
        public IList<CORSRule> CORSRules 
        {
            get { return ((List<CORSRule>) _corsRules).AsReadOnly(); }
            set
            {
                if (value == null)
                    throw new ArgumentException("CORSRule list should not be null.");
                if (value.Count > OssUtils.BucketCorsRuleLimit)
                    throw new ArgumentException("One bucket not allow exceed ten item of CORSRules.");
                _corsRules = value;
            }
        }

        /// <summary>
        /// Creates a new instance of<see cref="SetBucketCorsRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        public SetBucketCorsRequest(string bucketName) 
        {
            BucketName = bucketName;
        }

        /// <summary>
        /// Add a CORRule instance.
        /// </summary>
        /// <param name="corsRule"></param>
        public void AddCORSRule(CORSRule corsRule)
        {
            if (corsRule == null)
                throw new ArgumentException("corsRule should not be null or empty");

            if (_corsRules.Count >= OssUtils.BucketCorsRuleLimit)
                throw new ArgumentException("One bucket not allow exceed ten item of CORSRules.");

            if (corsRule.AllowedOrigins.Count == 0)
                throw new ArgumentException("corsRule.AllowedOrigins should not be empty");

            if (corsRule.AllowedMethods.Count == 0)
                throw new ArgumentException("corsRule.AllowedMethods should not be empty.");

            _corsRules.Add(corsRule);
        }

    }

}
