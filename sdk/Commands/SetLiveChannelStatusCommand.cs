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
using System.IO;

namespace Aliyun.OSS.Commands
{
    internal class SetLiveChannelStatusCommand : OssCommand
    {
        private readonly SetLiveChannelStatusRequest _request;

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
            get { return _request.ChannelName; }
        }

        protected override Stream Content
        {
            get { return new MemoryStream(); }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_LIVE, null },
                    { RequestParameters.SUBRESOURCE_STATUS, _request.Status }
                };
                return parameters;
            }
        }

        private SetLiveChannelStatusCommand(IServiceClient client, Uri endpoint, 
                                    ExecutionContext context,
                                    SetLiveChannelStatusRequest request)
            : base(client, endpoint, context)
        {
            _request = request;
        }

        public static SetLiveChannelStatusCommand Create(IServiceClient client, Uri endpoint, 
                                                 ExecutionContext context,
                                                 SetLiveChannelStatusRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            return new SetLiveChannelStatusCommand(client, endpoint, context, request);
        }
    }
}
