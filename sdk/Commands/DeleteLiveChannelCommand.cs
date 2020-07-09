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
    /// Delete LiveChannel command.
    /// </summary>
    internal class DeleteLiveChannelCommand : OssCommand
    {
        private readonly DeleteLiveChannelRequest _request;

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
                return _request.ChannelName;
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
                    { RequestParameters.SUBRESOURCE_LIVE, null }
                };
                return parameters;
            }
        }

        private DeleteLiveChannelCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                       DeleteLiveChannelRequest request)
            : base(client, endpoint, context)
        {
            _request = request;
        }

        public static DeleteLiveChannelCommand Create(IServiceClient client, Uri endpoint,
                                                    ExecutionContext context,
                                                    DeleteLiveChannelRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            return new DeleteLiveChannelCommand(client, endpoint, context, request);
        }
    }
}

