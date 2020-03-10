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
        private readonly bool _simplifiedMetadata;

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

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var paramters = base.Parameters;
                if (_simplifiedMetadata)
                {
                    paramters.Add(RequestParameters.SUBRESOURCE_OBJECTMETA, null);
                }
                return paramters;
            }
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
                                         GetObjectMetadataRequest request, bool simplifiedMetadata)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);
            _request = request;
            _simplifiedMetadata = simplifiedMetadata;
        }

        public static GetObjectMetadataCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                      GetObjectMetadataRequest request)
        {
            return new GetObjectMetadataCommand(client, endpoint, context,
                                                DeserializerFactory.GetFactory().CreateGetObjectMetadataResultDeserializer(),
                                                request, false);
        }

        public static GetObjectMetadataCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                              GetObjectMetadataRequest request, bool simplifiedMetadata)
        {
            return new GetObjectMetadataCommand(client, endpoint, context,
                                                DeserializerFactory.GetFactory().CreateGetObjectMetadataResultDeserializer(),
                                                request, simplifiedMetadata);
        }
    }

}
