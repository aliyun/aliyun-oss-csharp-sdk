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

namespace Aliyun.OSS.Commands
{
    internal class SetBucketLifecycleCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly SetBucketLifecycleRequest _setBucketLifecycleRequest;

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
                return SerializerFactory.GetFactory().CreateSetBucketLifecycleRequestSerializer()
                    .Serialize(_setBucketLifecycleRequest);
            }
        }

        private SetBucketLifecycleCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, SetBucketLifecycleRequest setBucketLifecycleRequest)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);

            _bucketName = bucketName;
            _setBucketLifecycleRequest = setBucketLifecycleRequest;
        }

        public static SetBucketLifecycleCommand Create(IServiceClient client, Uri endpoint,
                                                       ExecutionContext context,
                                                       string bucketName, SetBucketLifecycleRequest setBucketLifecycleRequest)
        {
            return new SetBucketLifecycleCommand(client, endpoint, context, bucketName, setBucketLifecycleRequest);
        }


        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_LIFECYCLE, null }
                };
            }
        }
    }
}
