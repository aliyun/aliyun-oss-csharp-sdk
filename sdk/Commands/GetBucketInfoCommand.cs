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
    internal class GetBucketInfoCommand : OssCommand<BucketInfo>
    {
        private readonly string _bucketName;

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private GetBucketInfoCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                     string bucketName, IDeserializer<ServiceResponse, BucketInfo> deserializer)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(bucketName);
            _bucketName = bucketName;
        }

        public static GetBucketInfoCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 string bucketName)
        {
            return new GetBucketInfoCommand(client, endpoint, context, bucketName,
                                           DeserializerFactory.GetFactory().CreateGetBucketInfoDeserializer());
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_BUCKETINFO, null }
                };
            }
        }
    }
}
