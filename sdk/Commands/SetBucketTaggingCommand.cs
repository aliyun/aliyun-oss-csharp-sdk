/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Domain;

namespace Aliyun.OSS.Commands
{
    internal class SetBucketTaggingCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly SetBucketTaggingRequest _setBucketTaggingRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateSetBucketTaggingRequestSerializer()
                    .Serialize(_setBucketTaggingRequest);
            }
        }

        private SetBucketTaggingCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, SetBucketTaggingRequest setBucketTaggingRequest)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);

            _bucketName = bucketName;
            _setBucketTaggingRequest = setBucketTaggingRequest;
        }

        public static SetBucketTaggingCommand Create(IServiceClient client, Uri endpoint,
                                                       ExecutionContext context,
                                                       string bucketName, SetBucketTaggingRequest setBucketTaggingRequest)
        {
            return new SetBucketTaggingCommand(client, endpoint, context, bucketName, setBucketTaggingRequest);
        }


        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_TAGGING, null }
                };
            }
        }
    }
}
