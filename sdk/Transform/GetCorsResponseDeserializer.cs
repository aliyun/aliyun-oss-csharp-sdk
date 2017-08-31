/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using System.Collections.Generic;
using System.IO;

namespace Aliyun.OSS.Transform
{
    internal class GetCorsResponseDeserializer : ResponseDeserializer<IList<CORSRule>, SetBucketCorsRequestModel>
    {
        public GetCorsResponseDeserializer(IDeserializer<Stream, SetBucketCorsRequestModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override IList<CORSRule> Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);
            
            IList<CORSRule> corsRuleList = new List<CORSRule>();
            foreach (var corsRuleModel in model.CORSRuleModels)
            {
                var corsRule = new CORSRule();
                if (corsRuleModel.AllowedHeaders != null && corsRuleModel.AllowedHeaders.Length > 0)
                    corsRule.AllowedHeaders = ToList(corsRuleModel.AllowedHeaders);
                if (corsRuleModel.AllowedMethods != null && corsRuleModel.AllowedMethods.Length > 0)
                    corsRule.AllowedMethods = ToList(corsRuleModel.AllowedMethods);
                if (corsRuleModel.AllowedOrigins != null && corsRuleModel.AllowedOrigins.Length > 0)
                    corsRule.AllowedOrigins = ToList(corsRuleModel.AllowedOrigins);
                if (corsRuleModel.ExposeHeaders != null && corsRuleModel.ExposeHeaders.Length > 0)
                    corsRule.ExposeHeaders = ToList(corsRuleModel.ExposeHeaders);
                corsRule.MaxAgeSeconds = corsRuleModel.MaxAgeSeconds;
                corsRuleList.Add(corsRule);
            }

            return corsRuleList;
        }

        private IList<string> ToList(string[] array)
        {
            IList<string> list = new List<string>();
            foreach(var value in array)
            {
                list.Add(value);
            }
            return list;
        }
    }
}
