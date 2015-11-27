/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
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
                    rule.ExpirationTime = DateTime.Parse(lcc.Expiration.Date);

                rules.Add(rule);
            }
            return rules;            
        }

        /// <summary>
        /// 为了兼容.NET2.0，实现了一个类似.NET4.0中Enum.TryParse的方法
        /// </summary>
        /// <param name="value">转换的枚举名称或基础值的字符串表示形式。</param>
        /// <param name="status">此方法在返回时包含一个类型为 TEnum 的一个对象，其值由 value 表示。该参数未经初始化即被传递。</param>
        /// <returns>是否解析成功</returns>
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
    }
}
