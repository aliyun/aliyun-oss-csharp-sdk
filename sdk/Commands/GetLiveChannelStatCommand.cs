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
    internal class GetLiveChannelStatCommand : OssCommand<GetLiveChannelStatResult>
    {
        private readonly GetLiveChannelStatRequest _request;

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

        private GetLiveChannelStatCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                      IDeserializer<ServiceResponse, GetLiveChannelStatResult> deserializer,
                                      GetLiveChannelStatRequest request)
           : base(client, endpoint, context, deserializer)
        {
            _request = request;
        }

        public static GetLiveChannelStatCommand Create(IServiceClient client, Uri endpoint,
                                                   ExecutionContext context,
                                                   GetLiveChannelStatRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            return new GetLiveChannelStatCommand(client, endpoint, context,
                                           DeserializerFactory.GetFactory().CreateGetLiveChannelStatResultDeserializer(),
                                           request);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters =  new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_LIVE, null },
                    { RequestParameters.SUBRESOURCE_COMP, "stat" }
                };
                return parameters;
            }
        }
    }
}

