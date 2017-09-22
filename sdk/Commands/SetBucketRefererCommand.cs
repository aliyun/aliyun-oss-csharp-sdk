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

namespace Aliyun.OSS.Commands
{
    /// <summary>
    /// Description of SetBucketRefererCommand.
    /// </summary>
    internal class SetBucketRefererCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly SetBucketRefererRequest _setBucketRefererRequest;

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
                return SerializerFactory.GetFactory().CreateSetBucketRefererRequestSerializer()
                    .Serialize(_setBucketRefererRequest);
            }
        }

        private SetBucketRefererCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, SetBucketRefererRequest setBucketRefererRequest)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);

            _bucketName = bucketName;
            _setBucketRefererRequest = setBucketRefererRequest;
        }

        public static SetBucketRefererCommand Create(IServiceClient client, Uri endpoint, 
                                                    ExecutionContext context,
                                                    string bucketName, SetBucketRefererRequest setBucketRefererRequest)
        {
            return new SetBucketRefererCommand(client, endpoint, context, bucketName, setBucketRefererRequest);
        }


        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_REFERER, null }
                };
            }
        }
    }
}
