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
    internal class DeleteBucketPolicyCommand : OssCommand
    {
        private readonly string _bucketName;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private DeleteBucketPolicyCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                       string bucketName)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            _bucketName = bucketName;
        }

        public static DeleteBucketPolicyCommand Create(IServiceClient client, Uri endpoint,
                                                    ExecutionContext context,
                                                    string bucketName)
        {
            return new DeleteBucketPolicyCommand(client, endpoint, context, bucketName);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_POLICY, null }
                };
            }
        }
    }
}
