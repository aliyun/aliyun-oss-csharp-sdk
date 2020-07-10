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
    internal class GetBucketInventoryConfigurationCommand : OssCommand<GetBucketInventoryConfigurationResult>
    {
        private readonly GetBucketInventoryConfigurationRequest _request;

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        private GetBucketInventoryConfigurationCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    GetBucketInventoryConfigurationRequest request,
                                    IDeserializer<ServiceResponse, GetBucketInventoryConfigurationResult> deserializer)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(request.BucketName);
            _request = request;
        }

        public static GetBucketInventoryConfigurationCommand Create(IServiceClient client, Uri endpoint,
                                                     ExecutionContext context,
                                                    GetBucketInventoryConfigurationRequest request)
        {
            return new GetBucketInventoryConfigurationCommand(client, endpoint, context, request,
                                              DeserializerFactory.GetFactory().CreateGetBucketInventoryConfigurationResultDeserializer());
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_INVENTORY, null },
                    { RequestParameters.SUBRESOURCE_INVENTORY_ID, _request.Id }
                };
            }
        }
    }
}
