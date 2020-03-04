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
using System.Text;

namespace Aliyun.OSS.Commands
{
    internal class SetBucketPolicyCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly SetBucketPolicyRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private SetBucketPolicyCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, SetBucketPolicyRequest request)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);

            _bucketName = bucketName;
            _request = request;
        }

        public static SetBucketPolicyCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 string bucketName, SetBucketPolicyRequest request)
        {
            return new SetBucketPolicyCommand(client, endpoint, context, bucketName, request);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_POLICY, null }
                };
            }
        }

        protected override Stream Content
        {
            get
            {
                return new MemoryStream(Encoding.UTF8.GetBytes(_request.Policy));
            }
        }
    }
}
