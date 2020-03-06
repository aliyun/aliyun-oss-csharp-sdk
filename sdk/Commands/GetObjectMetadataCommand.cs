/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;
using System.Collections.Generic;

namespace Aliyun.OSS.Commands
{
    internal class GetObjectMetadataCommand : OssCommand<ObjectMetadata>
    {
        private readonly GetObjectMetadataRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Head; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        protected override string Key
        {
            get { return _request.Key; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                if (_request.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                return headers;
            }
        }

        private GetObjectMetadataCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                         IDeserializer<ServiceResponse, ObjectMetadata> deserializer,
                                         GetObjectMetadataRequest request)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);
            _request = request;
        }

        public static GetObjectMetadataCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                      GetObjectMetadataRequest request)
        {
            return new GetObjectMetadataCommand(client, endpoint, context,
                                                DeserializerFactory.GetFactory().CreateGetObjectMetadataResultDeserializer(),
                                                request);
        }
    }

}
