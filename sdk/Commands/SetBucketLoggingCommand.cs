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
    internal class SetBucketLoggingCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly SetBucketLoggingRequest _setBucketLoggingRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private SetBucketLoggingCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, SetBucketLoggingRequest setBucketLoggingRequest)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(setBucketLoggingRequest.BucketName);
            OssUtils.CheckBucketName(setBucketLoggingRequest.TargetBucket);

            if (!OssUtils.IsLoggingPrefixValid(setBucketLoggingRequest.TargetPrefix))
                throw new ArgumentException("Invalid logging prefix " + setBucketLoggingRequest.TargetPrefix);

            _bucketName = bucketName;
            _setBucketLoggingRequest = setBucketLoggingRequest;
        }

        public static SetBucketLoggingCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                     string bucketName, SetBucketLoggingRequest setBucketLoggingRequest)
        {
            return new SetBucketLoggingCommand(client, endpoint, context, bucketName, setBucketLoggingRequest);
        }
        
        
        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_LOGGING, null }
                };
            }
        }
        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateSetBucketLoggingRequestSerializer()
                    .Serialize(_setBucketLoggingRequest);
            }
        }
    }
}
