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
    internal class GetBucketVersioningCommand : OssCommand<GetBucketVersioningResult>
    {
        private readonly string _bucketName;

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private GetBucketVersioningCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                      string bucketName, IDeserializer<ServiceResponse, GetBucketVersioningResult> deserializer)
           : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(bucketName);
            _bucketName = bucketName;
        }

        public static GetBucketVersioningCommand Create(IServiceClient client, Uri endpoint,
                                                   ExecutionContext context,
                                                   string bucketName)
        {
            return new GetBucketVersioningCommand(client, endpoint, context, bucketName,
                                           DeserializerFactory.GetFactory().CreateGetBucketVersioningResultDeserializer());
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_VERSIONING, null }
                };
            }
        }
    }
}
