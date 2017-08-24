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
    internal class DeleteBucketWebsiteCommand  : OssCommand
    {
        private readonly string _bucketName;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }

        protected override string Bucket
        {
            get
            {
                return _bucketName;
            }
        }

        private DeleteBucketWebsiteCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            _bucketName = bucketName;
        }

        public static DeleteBucketWebsiteCommand Create(IServiceClient client, Uri endpoint,
                                                       ExecutionContext context,
                                                       string bucketName)
        {
            return new DeleteBucketWebsiteCommand(client, endpoint, context, bucketName);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_WEBSITE, null }
                };
            }
        }
    }
}
