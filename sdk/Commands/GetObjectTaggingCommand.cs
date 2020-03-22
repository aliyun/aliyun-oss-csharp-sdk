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
    internal class GetObjectTaggingCommand : OssCommand<GetObjectTaggingResult>
    {
        private readonly GetObjectTaggingRequest _request;

        protected override string Bucket
        {
            get
            {
                return _request.BucketName;
            }
        }

        protected override string Key
        {
            get
            {
                return _request.Key;
            }
        }

        private GetObjectTaggingCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                      IDeserializer<ServiceResponse, GetObjectTaggingResult> deserializer,
                                      GetObjectTaggingRequest request)
           : base(client, endpoint, context, deserializer)
        {
            _request = request;
        }

        public static GetObjectTaggingCommand Create(IServiceClient client, Uri endpoint,
                                                   ExecutionContext context,
                                                   GetObjectTaggingRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);
            return new GetObjectTaggingCommand(client, endpoint, context,
                                           DeserializerFactory.GetFactory().CreateGetObjectTaggingResultDeserializer(),
                                           request);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters =  new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_TAGGING, null }
                };
                if (!string.IsNullOrEmpty(_request.VersionId))
                {
                    parameters.Add(RequestParameters.SUBRESOURCE_VERSIONID, _request.VersionId);
                }
                return parameters;
            }
        }
    }
}

