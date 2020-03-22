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
using Aliyun.OSS.Properties;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Commands
{
    internal class RestoreObjectCommand : OssCommand<RestoreObjectResult>
    {
        private readonly RestoreObjectRequest _request;

        protected override string Bucket
        {
            get { 
                return _request.BucketName; 
            }
        }

        protected override string Key
        {
            get {
                return _request.Key; 
            }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, string>();
                parameters[RequestParameters.SUBRESOURCE_RESTORE] = null;
                if (!string.IsNullOrEmpty(_request.VersionId))
                {
                    parameters.Add(RequestParameters.SUBRESOURCE_VERSIONID, _request.VersionId);
                }
                return parameters;
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

        private RestoreObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                     RestoreObjectRequest request, IDeserializer<ServiceResponse, RestoreObjectResult> deserializer)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);
            _request = request;
            this.ParametersInUri = true; // in restore request, the parameter restore needs to be in uri
        }

        public static RestoreObjectCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 RestoreObjectRequest request)
        {
            return new RestoreObjectCommand(client, endpoint, context, request, DeserializerFactory.GetFactory().CreateRestoreObjectResultDeserializer());
        }
    }
}
