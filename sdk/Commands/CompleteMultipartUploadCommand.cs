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
using Aliyun.OSS.Common.Handlers;
using Aliyun.OSS.Util;
using Aliyun.OSS.Properties;

namespace Aliyun.OSS.Commands
{
    internal class CompleteMultipartUploadCommand : OssCommand<CompleteMultipartUploadResult>
    {
        private readonly CompleteMultipartUploadRequest _completeMultipartUploadRequest;
        
        protected override string Bucket
        {
            get
            {
                return _completeMultipartUploadRequest.BucketName;
            }
        }
        
        protected override string Key
        {
            get
            {
                return _completeMultipartUploadRequest.Key;
            }
        }
        
        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                if (_completeMultipartUploadRequest.Metadata != null)
                {
                    _completeMultipartUploadRequest.Metadata.Populate(headers);
                }
                if (_completeMultipartUploadRequest.RequestPayer == RequestPayer.Requester)
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
                var parameters = base.Parameters;
                parameters[RequestParameters.UPLOAD_ID] = _completeMultipartUploadRequest.UploadId;
                return parameters;
            }
        }
        
        protected override Stream Content
        {
            get 
            { 
                return SerializerFactory.GetFactory().CreateCompleteUploadRequestSerializer()
                    .Serialize(_completeMultipartUploadRequest); 
            }
        }

        protected override bool LeaveResponseOpen
        {
            get { return _completeMultipartUploadRequest.IsNeedResponseStream(); }
        }
        
        private CompleteMultipartUploadCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                 IDeserializer<ServiceResponse, CompleteMultipartUploadResult> deserializeMethod,
                                 CompleteMultipartUploadRequest completeMultipartUploadRequest)
                        : base(client, endpoint, context, deserializeMethod)
        {
            _completeMultipartUploadRequest = completeMultipartUploadRequest;
        }
        
        public static CompleteMultipartUploadCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                  CompleteMultipartUploadRequest completeMultipartUploadRequest)
        {
            OssUtils.CheckBucketName(completeMultipartUploadRequest.BucketName);
            OssUtils.CheckObjectKey(completeMultipartUploadRequest.Key);

            if (string.IsNullOrEmpty(completeMultipartUploadRequest.UploadId))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "uploadId");

            // handle upload callback error 203
            if (completeMultipartUploadRequest.IsCallbackRequest())
            {
                context.ResponseHandlers.Add(new CallbackResponseHandler());
            }

            var conf = OssUtils.GetClientConfiguration(client);
            if (conf.EnableCrcCheck)
            {
                context.ResponseHandlers.Add(new CompleteMultipartUploadCrc64Handler(completeMultipartUploadRequest)); 
            }

            return new CompleteMultipartUploadCommand(client, endpoint, context,
                                               DeserializerFactory.GetFactory().CreateCompleteUploadResultDeserializer(completeMultipartUploadRequest),
                                               completeMultipartUploadRequest);
            
        }
    }
}
