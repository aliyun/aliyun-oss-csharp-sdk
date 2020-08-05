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
    internal class InitiateBucketWormCommand : OssCommand<InitiateBucketWormResult>
    {
        private readonly InitiateBucketWormRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        private InitiateBucketWormCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    InitiateBucketWormRequest request,
                                    IDeserializer<ServiceResponse, InitiateBucketWormResult> deserializer)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(request.BucketName);

            _request = request;
        }

        public static InitiateBucketWormCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 InitiateBucketWormRequest request)
        {
            return new InitiateBucketWormCommand(client, endpoint, context, request,
                DeserializerFactory.GetFactory().CreateInitiateBucketWormResultDeserializer());
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_WORM, null }
                };
            }
        }

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateInitiateBucketWormSerializer()
                    .Serialize(_request);
            }
        }
    }
}

