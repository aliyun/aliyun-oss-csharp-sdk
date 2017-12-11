/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class GetBucketLifecycleDeserializer 
        : ResponseDeserializer<IList<LifecycleRule>, LifecycleConfiguration>
    {
        public GetBucketLifecycleDeserializer(IDeserializer<Stream, LifecycleConfiguration> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override IList<LifecycleRule> Deserialize(ServiceResponse xmlStream)
        {
            var lifecycleConfig = ContentDeserializer.Deserialize(xmlStream.Content);
            var rules = new List<LifecycleRule>();
            foreach (var lcc in lifecycleConfig.LifecycleRules)
            {
                var rule = new LifecycleRule
                {
                    ID = lcc.ID, 
                    Prefix = lcc.Prefix
                };

                RuleStatus status;
                if (TryParseEnum(lcc.Status, out status))
                    rule.Status = status;
                else
                    throw new InvalidEnumArgumentException(@"Unsupported rule status " + lcc.Status);

                if (lcc.Expiration.IsSetDays())
                    rule.ExpriationDays = lcc.Expiration.Days;
                else if (lcc.Expiration.IsSetDate())
                    rule.ExpirationTime = DateTime.Parse(lcc.Expiration.Date).ToUniversalTime();
                else if (!String.IsNullOrEmpty(lcc.Expiration.CreatedBeforeDate))
                    rule.CreatedBeforeDate = DateTime.Parse(lcc.Expiration.CreatedBeforeDate).ToUniversalTime();

                if (lcc.AbortMultipartUpload != null)
                {
                    rule.AbortMultipartUpload = new LifecycleRule.LifeCycleExpiration();
                    ConvertExpiration(lcc.AbortMultipartUpload, rule.AbortMultipartUpload);
                }

                if (lcc.Transition != null)
                {
                    rule.Transitions = new LifecycleRule.LifeCycleTransition[lcc.Transition.Length];
                    for (int i = 0; i < rule.Transitions.Length; i++)
                    {
                        rule.Transitions[i] = new LifecycleRule.LifeCycleTransition();
                        ConvertTransition(lcc.Transition[i], rule.Transitions[i]);
                    }
                }

                rules.Add(rule);
            }
            return rules;            
        }

        /// <summary>
        /// TryParseEnum does not exist in .net 2.0. But we need to support .net 2.0
        /// </summary>
        /// <param name="value">The string value to parse from.</param>
        /// <param name="status">The parsed value </param>
        /// <returns>True: the parse succeeds; False: the parse fails</returns>
        private bool TryParseEnum(string value, out RuleStatus status) 
        {
            if (!Enum.IsDefined(typeof(RuleStatus), value))
            {
                status = RuleStatus.Disabled;
                return false;
            }

            status = (RuleStatus)Enum.Parse(typeof(RuleStatus), value);
            return true;
        }

        internal static void ConvertExpiration(Expiration expiration, LifecycleRule.LifeCycleExpiration lifeCycleExpiration)
        {
            lifeCycleExpiration.Days = expiration.Days;
            if (!String.IsNullOrEmpty(expiration.CreatedBeforeDate))
            {
                lifeCycleExpiration.CreatedBeforeDate = DateTime.Parse(expiration.CreatedBeforeDate).ToUniversalTime();
            }
        }

        internal static void ConvertTransition(LifecycleRuleTransition transition, LifecycleRule.LifeCycleTransition lifeCycleTransition)
        {
            ConvertExpiration(transition, lifeCycleTransition.LifeCycleExpiration);
            lifeCycleTransition.StorageClass = transition.StorageClass;
        }
    }
}
