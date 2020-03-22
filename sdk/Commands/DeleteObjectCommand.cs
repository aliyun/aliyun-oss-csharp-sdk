/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;


namespace Aliyun.OSS.Commands
{
    internal class DeleteObjectCommand : OssCommand <DeleteObjectResult>
    {
        private readonly DeleteObjectRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
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

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, string>()
                {
                    {RequestParameters.ENCODING_TYPE, HttpUtils.UrlEncodingType }
                };
                if (!string.IsNullOrEmpty(_request.VersionId))
                {
                    parameters.Add(RequestParameters.SUBRESOURCE_VERSIONID, _request.VersionId);
                }
                return parameters;
            }
        }

        private DeleteObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    IDeserializer<ServiceResponse, DeleteObjectResult> deserialize,
                                    DeleteObjectRequest request)
            : base(client, endpoint, context, deserialize)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);

            _request = request;
        }

        public static DeleteObjectCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                 DeleteObjectRequest request)
        {
            return new DeleteObjectCommand(client, endpoint, context,
                DeserializerFactory.GetFactory().CreateDeleteObjectResultDeserializer(),
                request);
        }
    }
}
