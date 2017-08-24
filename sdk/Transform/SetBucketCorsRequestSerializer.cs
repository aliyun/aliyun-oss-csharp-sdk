/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
using System.IO;

namespace Aliyun.OSS.Transform
{
    internal class SetBucketCorsRequestSerializer : RequestSerializer<SetBucketCorsRequest, SetBucketCorsRequestModel>
    {
        public SetBucketCorsRequestSerializer(ISerializer<SetBucketCorsRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

       public override Stream Serialize(SetBucketCorsRequest request)
       {
            var model = new SetBucketCorsRequestModel
            {
                CORSRuleModels = new SetBucketCorsRequestModel.CORSRuleModel[request.CORSRules.Count]
            };

           for (var i = 0; i < request.CORSRules.Count ;i++ )
            {
                var corsRuleModel = new SetBucketCorsRequestModel.CORSRuleModel();

                if (request.CORSRules[i].AllowedHeaders != null)
                {
                    corsRuleModel.AllowedHeaders = new string[request.CORSRules[i].AllowedHeaders.Count];
                    for (var j = 0; j < request.CORSRules[i].AllowedHeaders.Count; j++)
                        corsRuleModel.AllowedHeaders[j] = request.CORSRules[i].AllowedHeaders[j];
                }

                if (request.CORSRules[i].AllowedMethods != null)
                {
                    corsRuleModel.AllowedMethods = new string[request.CORSRules[i].AllowedMethods.Count];
                    for (var j = 0; j < request.CORSRules[i].AllowedMethods.Count; j++)
                        corsRuleModel.AllowedMethods[j] = request.CORSRules[i].AllowedMethods[j];
                }

                if (request.CORSRules[i].AllowedOrigins != null)
                {
                    corsRuleModel.AllowedOrigins = new string[request.CORSRules[i].AllowedOrigins.Count];
                    for (var j = 0; j < request.CORSRules[i].AllowedOrigins.Count; j++)
                        corsRuleModel.AllowedOrigins[j] = request.CORSRules[i].AllowedOrigins[j];
                }

                if (request.CORSRules[i].ExposeHeaders != null)
                {
                    corsRuleModel.ExposeHeaders = new string[request.CORSRules[i].ExposeHeaders.Count];
                    for (var j = 0; j < request.CORSRules[i].ExposeHeaders.Count; j++)
                        corsRuleModel.ExposeHeaders[j] = request.CORSRules[i].ExposeHeaders[j];
                }

                corsRuleModel.MaxAgeSeconds = request.CORSRules[i].MaxAgeSeconds;

                model.CORSRuleModels[i] = corsRuleModel;
            }

            return ContentSerializer.Serialize(model);
        }
    }
}
