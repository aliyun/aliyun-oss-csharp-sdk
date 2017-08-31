/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class SetObjectAclCommand : OssCommand
    {
        private readonly SetObjectAclRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }
        
        protected override string Key
        { 
            get { return _request.Key; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { OssHeaders.OssObjectCannedAcl, EnumUtils.GetStringValue(_request.ACL) }
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

        private SetObjectAclCommand(IServiceClient client, Uri endpoint, 
                                    ExecutionContext context,
                                    SetObjectAclRequest request)
            : base(client, endpoint, context)
        {
            _request = request;
        }

        public static SetObjectAclCommand Create(IServiceClient client, Uri endpoint, 
                                                 ExecutionContext context,
                                                 SetObjectAclRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);
            return new SetObjectAclCommand(client, endpoint, context, request);
        }
    }
}
