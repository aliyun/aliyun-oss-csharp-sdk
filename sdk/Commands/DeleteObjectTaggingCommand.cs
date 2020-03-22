/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using System.Collections.Generic;

namespace Aliyun.OSS.Commands
{
    /// <summary>
    /// Delete object tagging command.
    /// </summary>
    internal class DeleteObjectTaggingCommand : OssCommand
    {
        private readonly DeleteObjectTaggingRequest _request;

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

        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, string>()
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

        private DeleteObjectTaggingCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                       DeleteObjectTaggingRequest request)
            : base(client, endpoint, context)
        {
            _request = request;
        }

        public static DeleteObjectTaggingCommand Create(IServiceClient client, Uri endpoint,
                                                    ExecutionContext context,
                                                    DeleteObjectTaggingRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);
            return new DeleteObjectTaggingCommand(client, endpoint, context, request);
        }
    }
}

