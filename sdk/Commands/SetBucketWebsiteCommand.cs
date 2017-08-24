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
    internal class SetBucketWebsiteCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly SetBucketWebsiteRequest _setBucketWebsiteRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private SetBucketWebsiteCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                       string bucketName, SetBucketWebsiteRequest setBucketWebsiteRequest)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            if (string.IsNullOrEmpty(setBucketWebsiteRequest.IndexDocument))
                throw new ArgumentException("index document must not be empty");
            if (!OssUtils.IsWebpageValid(setBucketWebsiteRequest.IndexDocument))
                throw new ArgumentException("Invalid index document, must be end with .html");
            if (!string.IsNullOrEmpty(setBucketWebsiteRequest.ErrorDocument) 
                && !OssUtils.IsWebpageValid(setBucketWebsiteRequest.ErrorDocument))
                throw new ArgumentException("Invalid error document, must be end with .html");

            _bucketName = bucketName;
            _setBucketWebsiteRequest = setBucketWebsiteRequest;
        }

        public static SetBucketWebsiteCommand Create(IServiceClient client, Uri endpoint, 
                                                    ExecutionContext context,
                                                    string bucketName, SetBucketWebsiteRequest setBucketWebsiteRequest)
        {
            return new SetBucketWebsiteCommand(client, endpoint, context, bucketName, setBucketWebsiteRequest);
        }
        
        
        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_WEBSITE, null }
                };
            }
        }
        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateSetBucketWebsiteRequestSerializer()
                    .Serialize(_setBucketWebsiteRequest);
            }
        }
    }
}
