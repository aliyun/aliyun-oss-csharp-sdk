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
    internal class DeleteObjectsCommand : OssCommand<DeleteObjectsResult>
    {
        private readonly DeleteObjectsRequest _deleteObjectsRequest;
        
        protected override string Bucket
        {
            get { return _deleteObjectsRequest.BucketName; }
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
                parameters[RequestParameters.ENCODING_TYPE] = _deleteObjectsRequest.EncodingType;
                return parameters;
            }
        }
        
        protected override Stream Content
        {
            get 
            { 
                return SerializerFactory.GetFactory().CreateDeleteObjectsRequestSerializer()
                    .Serialize(_deleteObjectsRequest); 
            }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                headers[HttpHeaders.ContentLength] = Content.Length.ToString();
                headers[HttpHeaders.ContentMd5] = OssUtils.ComputeContentMd5(Content, Content.Length);
                if (_deleteObjectsRequest.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                return headers;
            }
        }

        private DeleteObjectsCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                IDeserializer<ServiceResponse, DeleteObjectsResult> deserializeMethod,
                                DeleteObjectsRequest deleteObjectsRequest)
            : base(client, endpoint, context, deserializeMethod)
        {
            _deleteObjectsRequest = deleteObjectsRequest;
        }

        public static DeleteObjectsCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                  DeleteObjectsRequest deleteObjectsRequest)
        {
            OssUtils.CheckBucketName(deleteObjectsRequest.BucketName);
            return new DeleteObjectsCommand(client, endpoint, context, 
                                            DeserializerFactory.GetFactory().CreateDeleteObjectsResultDeserializer(),
                                            deleteObjectsRequest);
        }
    }
}
