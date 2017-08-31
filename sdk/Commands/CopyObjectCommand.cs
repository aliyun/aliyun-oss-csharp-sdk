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

namespace Aliyun.OSS.Commands
{
    internal class CopyObjectCommand : OssCommand<CopyObjectResult>
    {
        private readonly CopyObjectRequest _copyObjectRequset;
        
        protected override string Bucket
        {
            get
            {
                return _copyObjectRequset.DestinationBucketName;
            }
        }
        
        protected override string Key
        {
            get
            {
                return _copyObjectRequset.DestinationKey;
            }
        }
        
        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }
        
        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                _copyObjectRequset.Populate(headers);
                return headers;
            }
        }
        
        private CopyObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                 IDeserializer<ServiceResponse, CopyObjectResult> deserializer,
                                 CopyObjectRequest copyObjectRequest)
            : base(client, endpoint, context, deserializer)
        {
            _copyObjectRequset = copyObjectRequest;
        }
        
        public static CopyObjectCommand Create(IServiceClient client, Uri endpoint,
                                 ExecutionContext context, CopyObjectRequest copyObjectRequest)
        {
            OssUtils.CheckBucketName(copyObjectRequest.SourceBucketName);
            OssUtils.CheckObjectKey(copyObjectRequest.SourceKey);
            OssUtils.CheckBucketName(copyObjectRequest.DestinationBucketName);
            OssUtils.CheckObjectKey(copyObjectRequest.DestinationKey);
            
            return new CopyObjectCommand(client, endpoint, context,
                                        DeserializerFactory.GetFactory().CreateCopyObjectResultDeserializer(),
                                        copyObjectRequest);
        
        }
    }
}
