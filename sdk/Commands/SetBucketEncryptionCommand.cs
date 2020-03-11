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
    internal class SetBucketEncryptionCommand : OssCommand
    {
        private readonly SetBucketEncryptionRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        private SetBucketEncryptionCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    SetBucketEncryptionRequest request)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(request.BucketName);
            _request = request;
        }

        public static SetBucketEncryptionCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                     SetBucketEncryptionRequest request)
        {
            return new SetBucketEncryptionCommand(client, endpoint, context, request);
        }
        
        
        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_ENCRYPTION, null }
                };
            }
        }
        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateSetBucketEncryptionRequestSerializer()
                    .Serialize(_request);
            }
        }
    }
}
