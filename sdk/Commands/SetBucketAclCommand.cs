/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Commands
{
    internal class SetBucketAclCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly SetBucketAclRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        } 

        private SetBucketAclCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, SetBucketAclRequest request)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);

            _bucketName = bucketName;
            _request = request;
        }

        public static SetBucketAclCommand Create(IServiceClient client, Uri endpoint, 
                                                 ExecutionContext context,
                                                 string bucketName, SetBucketAclRequest request)
        {
            return new SetBucketAclCommand(client, endpoint, context, bucketName, request);
        }
        
        protected override IDictionary<string, string> Headers
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { OssHeaders.OssCannedAcl, EnumUtils.GetStringValue(_request.ACL) }
                };
            }
        }
        
        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_ACL, null }
                };
            }
        }
    }
}
