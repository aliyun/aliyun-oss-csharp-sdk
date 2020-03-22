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
                var headers = new Dictionary<string, string>()
                {
                    { OssHeaders.OssObjectCannedAcl, EnumUtils.GetStringValue(_request.ACL) }
                };
                if (_request.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                return headers;
            }
        }
        
        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_ACL, null }
                };
                if (!string.IsNullOrEmpty(_request.VersionId))
                {
                    parameters.Add(RequestParameters.SUBRESOURCE_VERSIONID, _request.VersionId);
                }
                return parameters;
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
