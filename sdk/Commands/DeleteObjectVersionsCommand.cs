/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Commands
{
    internal class DeleteObjectVersionsCommand : OssCommand<DeleteObjectVersionsResult>
    {
        private readonly DeleteObjectVersionsRequest _request;
        
        protected override string Bucket
        {
            get { return _request.BucketName; }
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
                parameters[RequestParameters.SUBRESOURCE_DELETE] = null;
                parameters[RequestParameters.ENCODING_TYPE] = _request.EncodingType;
                return parameters;
            }
        }
        
        protected override Stream Content
        {
            get 
            { 
                return SerializerFactory.GetFactory().CreateDeleteObjectVersionsRequestSerializer()
                    .Serialize(_request); 
            }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                headers[HttpHeaders.ContentLength] = Content.Length.ToString();
                headers[HttpHeaders.ContentMd5] = OssUtils.ComputeContentMd5(Content, Content.Length);
                if (_request.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                return headers;
            }
        }

        private DeleteObjectVersionsCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                IDeserializer<ServiceResponse, DeleteObjectVersionsResult> deserializeMethod,
                                DeleteObjectVersionsRequest request)
            : base(client, endpoint, context, deserializeMethod)
        {
            _request = request;
        }

        public static DeleteObjectVersionsCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                  DeleteObjectVersionsRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            return new DeleteObjectVersionsCommand(client, endpoint, context, 
                                            DeserializerFactory.GetFactory().CreateDeleteObjectVersionsResultDeserializer(),
                                            request);
        }
    }
}
