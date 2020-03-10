/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Common.Handlers;

namespace Aliyun.OSS.Commands
{
    internal class UploadPartCommand : OssCommand<UploadPartResult>
    {
        private readonly UploadPartRequest _uploadPartRequest;
        
        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _uploadPartRequest.BucketName; }
        } 
        
        protected override string Key
        {
            get { return _uploadPartRequest.Key; }
        } 
        
        protected override IDictionary<string, string> Parameters
        {
            get 
            {
                var parameters = base.Parameters;
                parameters[RequestParameters.PART_NUMBER] = _uploadPartRequest.PartNumber.ToString();
                parameters[RequestParameters.UPLOAD_ID] = _uploadPartRequest.UploadId;
                return parameters;
            }
        }
        
        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                headers[HttpHeaders.ContentLength] = _uploadPartRequest.PartSize.ToString();
                headers[HttpHeaders.ContentMd5] = _uploadPartRequest.Md5Digest;
                if (_uploadPartRequest.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                if (_uploadPartRequest.TrafficLimit > 0)
                {
                    headers.Add(OssHeaders.OssTrafficLimit, _uploadPartRequest.TrafficLimit.ToString());
                }
                return headers;
            }
        }
        
        protected override Stream Content
        {
            get { return _uploadPartRequest.InputStream; }
        }
        
        protected override bool LeaveRequestOpen
        {
            get { return true; }
        }
        
        private UploadPartCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                 IDeserializer<ServiceResponse, UploadPartResult> deserializer,                                  
                                                UploadPartRequest uploadPartRequest)
            : base(client, endpoint, context, deserializer)
        {
            _uploadPartRequest = uploadPartRequest;
        }
        
        public static UploadPartCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                               UploadPartRequest uploadPartRequest)
        {
            OssUtils.CheckBucketName(uploadPartRequest.BucketName);
            OssUtils.CheckObjectKey(uploadPartRequest.Key);

            if (string.IsNullOrEmpty(uploadPartRequest.UploadId))
                throw new ArgumentException("uploadId should be specified");  
            if (!uploadPartRequest.PartNumber.HasValue)
                throw new ArgumentException("partNumber should be specified");
            if (!uploadPartRequest.PartSize.HasValue)
                throw new ArgumentException("partSize should be specified");
            if (uploadPartRequest.InputStream == null)
                throw new ArgumentException("inputStream should be specified");

            if (uploadPartRequest.PartSize < 0 || uploadPartRequest.PartSize > OssUtils.MaxFileSize)
                throw new ArgumentException("partSize not live in valid range");
            if (!OssUtils.IsPartNumberInRange(uploadPartRequest.PartNumber))
                throw new ArgumentException("partNumber not live in valid range");

            var conf = OssUtils.GetClientConfiguration(client);
            var originalStream = uploadPartRequest.InputStream;
            var streamLength = uploadPartRequest.PartSize.Value;

            // wrap input stream in PartialWrapperStream
            originalStream = new PartialWrapperStream(originalStream, streamLength);

            // setup progress
            var callback = uploadPartRequest.StreamTransferProgress;
            if (callback != null)
            {
                originalStream = OssUtils.SetupProgressListeners(originalStream, conf.ProgressUpdateInterval, client, callback);
                uploadPartRequest.InputStream = originalStream;
            }

            // wrap input stream in MD5Stream
            if (conf.EnalbeMD5Check)
            {
                var hashStream = new MD5Stream(originalStream, null, streamLength);
                uploadPartRequest.InputStream = hashStream;
                context.ResponseHandlers.Add(new MD5DigestCheckHandler(hashStream));
            }
            else if (conf.EnableCrcCheck)
            {
                var hashStream = new Crc64Stream(originalStream, null, streamLength);
                uploadPartRequest.InputStream = hashStream;
                context.ResponseHandlers.Add(new Crc64CheckHandler(hashStream));
            }

            return new UploadPartCommand(client, endpoint, context, 
                                         DeserializerFactory.GetFactory().CreateUploadPartResultDeserializer(uploadPartRequest.PartNumber.Value, streamLength),
                                        uploadPartRequest);
        }
    }
}
