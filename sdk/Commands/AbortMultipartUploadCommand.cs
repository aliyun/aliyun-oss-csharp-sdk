/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */
 
using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Properties;

namespace Aliyun.OSS.Commands
{
    internal class AbortMultipartUploadCommand : OssCommand
    {
        private readonly AbortMultipartUploadRequest _abortMultipartUploadRequest;
        
        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }

        protected override string Bucket
        {
            get
            {
                return _abortMultipartUploadRequest.BucketName;
            }
        }
        
        protected override string Key
        {
            get
            {
                return _abortMultipartUploadRequest.Key;
            }
        }
        
        protected override IDictionary<string, string> Parameters
        {
            get 
            {
                var parameters = base.Parameters;
                parameters[RequestParameters.UPLOAD_ID] = _abortMultipartUploadRequest.UploadId;
                return parameters;
            }
        }
        
        private AbortMultipartUploadCommand(IServiceClient client, Uri endpoint, ExecutionContext context, 
                                            AbortMultipartUploadRequest abortMultipartUploadRequest)
            : base(client, endpoint, context)
            
        {
            _abortMultipartUploadRequest = abortMultipartUploadRequest;
        }
        

        public static AbortMultipartUploadCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                 AbortMultipartUploadRequest abortMultipartUploadRequest)
        {
            OssUtils.CheckBucketName(abortMultipartUploadRequest.BucketName);
            OssUtils.CheckObjectKey(abortMultipartUploadRequest.Key);
            
            if (string.IsNullOrEmpty(abortMultipartUploadRequest.UploadId))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "uploadId");          
            
            return new AbortMultipartUploadCommand(client, endpoint, context, abortMultipartUploadRequest);
        }
    }
}
