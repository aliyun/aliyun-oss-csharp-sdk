/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using System.Collections.Generic;
namespace Aliyun.OSS.Commands
{
    internal class DeleteBucketInventoryConfigurationCommand : OssCommand
    {
        private readonly DeleteBucketInventoryConfigurationRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        private DeleteBucketInventoryConfigurationCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                       DeleteBucketInventoryConfigurationRequest request)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(request.BucketName);
            _request = request;
        }

        public static DeleteBucketInventoryConfigurationCommand Create(IServiceClient client, Uri endpoint,
                                                    ExecutionContext context,
                                                    DeleteBucketInventoryConfigurationRequest request)
        {
            return new DeleteBucketInventoryConfigurationCommand(client, endpoint, context, request);
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
