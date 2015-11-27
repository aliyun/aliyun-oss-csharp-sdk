/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Communication;

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

            if (uploadPartRequest.Md5Digest == null && uploadPartRequest.PartSize != null)
            {
                uploadPartRequest.Md5Digest = OssUtils.ComputeContentMd5(uploadPartRequest.InputStream, 
                                                                        (long)uploadPartRequest.PartSize);
            }

            return new UploadPartCommand(client, endpoint, context, 
                                        DeserializerFactory.GetFactory().CreateUploadPartResultDeserializer(uploadPartRequest.PartNumber.Value),
                                        uploadPartRequest);
        }
    }
}
