/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Handlers;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class PutObjectCommand : OssCommand<PutObjectResult>
    {
        private readonly PutObjectRequest _putObjectRequest;

        protected override string Bucket
        {
            get { return _putObjectRequest.BucketName; }
        } 
        
        protected override string Key
        {
            get { return _putObjectRequest.Key; }
        } 
        
        protected override bool LeaveRequestOpen
        {
            get { return true; }
        }

        protected override bool LeaveResponseOpen
        {
            get { return _putObjectRequest.IsNeedResponseStream(); }
        }

        private PutObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                 IDeserializer<ServiceResponse, PutObjectResult> deserializer,
                                 PutObjectRequest putObjectRequest)
            : base(client, endpoint, context, deserializer, putObjectRequest.UseChunkedEncoding)
        {
            _putObjectRequest = putObjectRequest;
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override Stream Content
        {
            get { return _putObjectRequest.Content; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                _putObjectRequest.Populate(headers);
                return headers;
            }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                if (_putObjectRequest.Process != null)
                {
                    parameters[RequestParameters.OSS_PROCESS] = _putObjectRequest.Process;
                }
                return parameters;
            }
        }

        public static PutObjectCommand Create(IServiceClient client, Uri endpoint, 
                                              ExecutionContext context, 
                                              PutObjectRequest putObjectRequest)
        {
            OssUtils.CheckBucketName(putObjectRequest.BucketName);
            OssUtils.CheckObjectKey(putObjectRequest.Key);

            if (putObjectRequest.Content == null)
                throw new ArgumentNullException("content");

            // handle upload callback error 203
            if (putObjectRequest.IsCallbackRequest())
            {
                context.ResponseHandlers.Add(new CallbackResponseHandler());
            }

            var conf = OssUtils.GetClientConfiguration(client);
            var originalStream = putObjectRequest.Content;

            // setup progress
            var callback = putObjectRequest.StreamTransferProgress;
            if (callback != null)
            {
                originalStream = OssUtils.SetupProgressListeners(originalStream, conf.ProgressUpdateInterval, client, callback);
                putObjectRequest.Content = originalStream;
            }

            // wrap input stream in MD5Stream
            if (conf.EnalbeMD5Check) 
            {
                var streamLength = originalStream.CanSeek ? originalStream.Length : -1;
                var hashStream = new MD5Stream(originalStream, null, streamLength);
                putObjectRequest.Content = hashStream;
                context.ResponseHandlers.Add(new MD5DigestCheckHandler(hashStream));
            }
            else if (conf.EnableCrcCheck)
            {
                var streamLength = originalStream.CanSeek ? originalStream.Length : -1;
                var hashStream = new Crc64Stream(originalStream, null, streamLength);
                putObjectRequest.Content = hashStream;
                context.ResponseHandlers.Add(new Crc64CheckHandler(hashStream));
            }
            
            return new PutObjectCommand(client, endpoint, context,
                                        DeserializerFactory.GetFactory().CreatePutObjectReusltDeserializer(putObjectRequest),
                                        putObjectRequest);
        }
    }
}
