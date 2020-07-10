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
using Aliyun.OSS.Model;
namespace Aliyun.OSS.Commands
{
    internal class GetBucketInventoryConfigurationCommand : OssCommand<BucketInventoryConfigurationResult>
    {
        private readonly string _bucketName;
        private readonly string _id;

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private GetBucketInventoryConfigurationCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName,string id, IDeserializer<ServiceResponse, BucketInventoryConfigurationResult> deserializer)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(bucketName);
            _bucketName = bucketName;
            _id = id;
        }

        public static GetBucketInventoryConfigurationCommand Create(IServiceClient client, Uri endpoint,
                                                     ExecutionContext context,
                                                    string bucketName, string id)
        {
            return new GetBucketInventoryConfigurationCommand(client, endpoint, context, bucketName,id,
                                              DeserializerFactory.GetFactory().CreateGetBucketInventoryConfigurationResultDeserializer());
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_INVENTORY, null },
                    { RequestParameters.SUBRESOURCE_ID, _id }
                };
            }
        }
    }
}
