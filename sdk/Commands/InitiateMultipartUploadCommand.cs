/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
using System;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
#pragma warning disable 618, 3005

    internal class InitiateMultipartUploadCommand : OssCommand<InitiateMultipartUploadResult>
    {
        private readonly InitiateMultipartUploadRequest _initiateMultipartUploadRequest;
        
        protected override string Bucket
        {
            get { return _initiateMultipartUploadRequest.BucketName; }
        }
        
        protected override string Key
        {
            get { return _initiateMultipartUploadRequest.Key; }
        }
        
        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }
        
        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_UPLOADS, null },
                    { RequestParameters.ENCODING_TYPE, _initiateMultipartUploadRequest.EncodingType }
                };
            }
        }
        
        protected override Stream Content
        {
            get { return new MemoryStream(new byte[0]); }
        }
        
        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                if (_initiateMultipartUploadRequest.ObjectMetadata != null)
                {
                    _initiateMultipartUploadRequest.ObjectMetadata.Populate(headers);
                }
                if (_initiateMultipartUploadRequest.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                return headers;
            }
        }
        
        private InitiateMultipartUploadCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                               IDeserializer<ServiceResponse, InitiateMultipartUploadResult> deserializeMethod,
                                               InitiateMultipartUploadRequest initiateMultipartUploadRequest)
            : base(client, endpoint, context, deserializeMethod)
        {
            _initiateMultipartUploadRequest = initiateMultipartUploadRequest;
        }
        
        public static InitiateMultipartUploadCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                            InitiateMultipartUploadRequest initiateMultipartUploadRequest)
        {
            OssUtils.CheckBucketName(initiateMultipartUploadRequest.BucketName);
            OssUtils.CheckObjectKey(initiateMultipartUploadRequest.Key);

            return new InitiateMultipartUploadCommand(client, endpoint, context,
                                        DeserializerFactory.GetFactory().CreateInitiateMultipartUploadResultDeserializer(),
                                        initiateMultipartUploadRequest);
        }
    }

#pragma warning restore 618, 3005
}
