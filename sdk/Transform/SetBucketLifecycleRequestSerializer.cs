/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.IO;
using Aliyun.OSS.Util;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class SetBucketLifecycleRequestSerializer : RequestSerializer<SetBucketLifecycleRequest, LifecycleConfiguration>
    {
        public SetBucketLifecycleRequestSerializer(ISerializer<LifecycleConfiguration, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(SetBucketLifecycleRequest request)
        {
            var rules = request.LifecycleRules;
            var lcc = new LifecycleConfiguration
            {
                LifecycleRules = new Model.LifecycleRule[rules.Count]
            };
            
            for (var i = 0; i < rules.Count; i++)
            {
                lcc.LifecycleRules[i] = new Model.LifecycleRule
                {
                    ID = rules[i].ID, 
                    Prefix = rules[i].Prefix
                };

                switch (rules[i].Status)
                {
                    case RuleStatus.Enabled:
                        lcc.LifecycleRules[i].Status = RuleStatus.Enabled.ToString();
                        break;
                    case RuleStatus.Disabled:
                        lcc.LifecycleRules[i].Status = RuleStatus.Disabled.ToString();
                        break;
                }

                lcc.LifecycleRules[i].Expiration = new Expiration();
                if (rules[i].ExpirationTime.HasValue)
                    lcc.LifecycleRules[i].Expiration.Date = DateUtils.FormatIso8601Date(rules[i].ExpirationTime.Value);
                else if (rules[i].ExpriationDays.HasValue)
                    lcc.LifecycleRules[i].Expiration.Days = rules[i].ExpriationDays.Value;
            }

            return ContentSerializer.Serialize(lcc);
        }
    }
}
