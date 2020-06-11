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
    /// Delete bucket tagging command.
    /// </summary>
    internal class DeleteBucketTaggingCommand : OssCommand
    {
        private readonly DeleteBucketTaggingRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        private DeleteBucketTaggingCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                       DeleteBucketTaggingRequest request)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(request.BucketName);
            _request = request;
        }

        public static DeleteBucketTaggingCommand Create(IServiceClient client, Uri endpoint,
                                                    ExecutionContext context,
                                                    DeleteBucketTaggingRequest request)
        {
            return new DeleteBucketTaggingCommand(client, endpoint, context, request);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                string str = null;
                for (var i = 0; i < _request.Tags.Count; i++)
                {
                    if (i != 0)
                        str += ",";
                    str += _request.Tags[i].Key;
                }
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_TAGGING, str }
                };
            }
        }
    }
}
