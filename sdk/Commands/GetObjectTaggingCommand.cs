/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class GetObjectTaggingCommand : OssCommand<GetObjectTaggingResult>
    {
        private string bucketName;
        private string key;

        protected override string Bucket
        {
            get
            {
                return bucketName;
            }
        }

        protected override string Key
        {
            get
            {
                return key;
            }
        }

        private GetObjectTaggingCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                      string BucketName, string Key, IDeserializer<ServiceResponse, GetObjectTaggingResult> deserializer)
           : base(client, endpoint, context, deserializer)
        {
            bucketName = BucketName;
            key = Key;
        }

        public static GetObjectTaggingCommand Create(IServiceClient client, Uri endpoint,
                                                   ExecutionContext context,
                                                   string bucketName,  string key)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(key);
            return new GetObjectTaggingCommand(client, endpoint, context, bucketName, key,
                                           DeserializerFactory.GetFactory().CreateGetObjectTaggingResultDeserializer());
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_TAGGING, null }
                };
            }
        }
    }
}

