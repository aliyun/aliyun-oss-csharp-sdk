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
        private readonly string _bucketName;
        private readonly SetBucketInventoryConfigurationRequest _setBucketInventoryConfigurationRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateSetBucketInventoryConfigurationRequestSerializer()
                    .Serialize(_setBucketInventoryConfigurationRequest);
            }
        }

        private SetBucketInventoryConfigurationCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, SetBucketInventoryConfigurationRequest setBucketInventoryConfigurationRequest)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);

            _bucketName = bucketName;
            _setBucketInventoryConfigurationRequest = setBucketInventoryConfigurationRequest;
        }

        public static SetBucketInventoryConfigurationCommand Create(IServiceClient client, Uri endpoint,
                                                       ExecutionContext context,
                                                       string bucketName, SetBucketInventoryConfigurationRequest setBucketInventoryConfigurationRequest)
        {
            return new SetBucketInventoryConfigurationCommand(client, endpoint, context, bucketName, setBucketInventoryConfigurationRequest);
        }


        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_ID, _setBucketInventoryConfigurationRequest.Id },
                    { RequestParameters.SUBRESOURCE_INVENTORY, null }

                };
            }
        }
    }
}
