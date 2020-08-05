/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using System.Collections.Generic;
using System.IO;
namespace Aliyun.OSS.Commands
{
    internal class CompleteBucketWormCommand : OssCommand
    {
        private readonly CompleteBucketWormRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        private CompleteBucketWormCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                          CompleteBucketWormRequest request)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(request.BucketName);
            _request = request;
        }

        public static CompleteBucketWormCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 CompleteBucketWormRequest request)
        {
            return new CompleteBucketWormCommand(client, endpoint, context, request);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_WORMID, _request.WormId }
                };
            }
        }

        protected override Stream Content
        {
            get { return new MemoryStream(new byte[0]); }
        }
    }
}
