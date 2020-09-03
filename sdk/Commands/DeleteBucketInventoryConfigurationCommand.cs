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
        private readonly string _bucketName;
        private readonly string _id;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private DeleteBucketInventoryConfigurationCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                       string bucketName,string id)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            _bucketName = bucketName;
            _id = id;
        }

        public static DeleteBucketInventoryConfigurationCommand Create(IServiceClient client, Uri endpoint,
                                                    ExecutionContext context,
                                                    string bucketName, string id)
        {
            return new DeleteBucketInventoryConfigurationCommand(client, endpoint, context, bucketName, id);
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
