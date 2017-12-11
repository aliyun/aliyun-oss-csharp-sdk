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
    /// <summary>
    /// Get bucket info command.
    /// </summary>
    internal class GetBucketStatCommand : OssCommand<BucketStat>
    {
        private readonly string _bucketName;

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private GetBucketStatCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                     string bucketName, IDeserializer<ServiceResponse, BucketStat> deserializer)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(bucketName);
            _bucketName = bucketName;
        }

        public static GetBucketStatCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 string bucketName)
        {
            return new GetBucketStatCommand(client, endpoint, context, bucketName,
                                           DeserializerFactory.GetFactory().CreateGetBucketStatDeserializer());
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_BUCKETSTAT, null }
                };
            }
        }
    }
}
