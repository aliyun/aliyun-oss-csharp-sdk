/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class SetBucketInventoryConfigurationCommand : OssCommand
    {
        private readonly SetBucketInventoryConfigurationRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateSetBucketInventoryConfigurationRequestSerializer()
                    .Serialize(_request);
            }
        }

        private SetBucketInventoryConfigurationCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    SetBucketInventoryConfigurationRequest request)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(request.BucketName);
            _request = request;
        }

        public static SetBucketInventoryConfigurationCommand Create(IServiceClient client, Uri endpoint,
                                                       ExecutionContext context,
                                                       SetBucketInventoryConfigurationRequest request)
        {
            return new SetBucketInventoryConfigurationCommand(client, endpoint, context, request);
        }


        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_INVENTORY_ID, _request.Configuration.Id },
                    { RequestParameters.SUBRESOURCE_INVENTORY, null }

                };
            }
        }
    }
}
