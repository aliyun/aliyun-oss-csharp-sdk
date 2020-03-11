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
        private readonly SetBucketWebsiteRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        private SetBucketWebsiteCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                       SetBucketWebsiteRequest request)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(request.BucketName);
            if (!string.IsNullOrEmpty(request.IndexDocument)
                && !OssUtils.IsWebpageValid(request.IndexDocument))
                throw new ArgumentException("Invalid index document, must be end with .html");
            if (!string.IsNullOrEmpty(request.ErrorDocument) 
                && !OssUtils.IsWebpageValid(request.ErrorDocument))
                throw new ArgumentException("Invalid error document, must be end with .html");

            _request = request;
        }

        public static SetBucketWebsiteCommand Create(IServiceClient client, Uri endpoint, 
                                                    ExecutionContext context,
                                                    string bucketName, SetBucketWebsiteRequest request)
        {
            return new SetBucketWebsiteCommand(client, endpoint, context, request);
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
                if (!string.IsNullOrEmpty(_request.Configuration))
                {
                    return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(_request.Configuration));
                }
                else
                {
                    return SerializerFactory.GetFactory().CreateSetBucketWebsiteRequestSerializer()
                        .Serialize(_request);
                }
            }
        }
    }
}
