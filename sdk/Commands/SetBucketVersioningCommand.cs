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
    internal class SetBucketVersioningCommand : OssCommand
    {
        private readonly SetBucketVersioningRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        private SetBucketVersioningCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    SetBucketVersioningRequest request)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(request.BucketName);

            _request = request;
        }

        public static SetBucketVersioningCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 SetBucketVersioningRequest request)
        {
            return new SetBucketVersioningCommand(client, endpoint, context, request);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_VERSIONING, null }
                };
            }
        }

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateSetBucketVersioningRequestSerializer()
                    .Serialize(_request);
            }
        }
    }
}

