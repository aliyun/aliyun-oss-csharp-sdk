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
using System.IO;

namespace Aliyun.OSS.Commands
{
    internal class ExtendBucketWormCommand : OssCommand
    {
        private readonly ExtendBucketWormRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        private ExtendBucketWormCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    ExtendBucketWormRequest request)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(request.BucketName);

            _request = request;
        }

        public static ExtendBucketWormCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 ExtendBucketWormRequest request)
        {
            return new ExtendBucketWormCommand(client, endpoint, context, request);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_WORMID, _request.WormId },
                    { RequestParameters.SUBRESOURCE_WORMExtend, null }
                };
            }
        }

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateExtendBucketWormSerializer()
                    .Serialize(_request);
            }
        }
    }
}

