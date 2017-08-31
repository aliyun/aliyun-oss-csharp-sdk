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
    internal class SetBucketCorsCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly SetBucketCorsRequest _setBucketCorsRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get
            {
                return _bucketName;
            }
        }

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateSetBucketCorsRequestSerializer()
                    .Serialize(_setBucketCorsRequest);
            }
        }

        private SetBucketCorsCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, SetBucketCorsRequest setBucketCorsRequest)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);

            _bucketName = bucketName;
            _setBucketCorsRequest = setBucketCorsRequest;
        }

        public static SetBucketCorsCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 string bucketName, SetBucketCorsRequest setBucketCorsRequest)
        {
            return new SetBucketCorsCommand(client, endpoint, context, bucketName, setBucketCorsRequest);
        }


        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_CORS, null }
                };
            }
        }
    }
}
