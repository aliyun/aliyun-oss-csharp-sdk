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
    internal class GetLiveChannelHistoryCommand : OssCommand<GetLiveChannelHistoryResult>
    {
        private readonly GetLiveChannelHistoryRequest _request;

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

        private GetLiveChannelHistoryCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                      IDeserializer<ServiceResponse, GetLiveChannelHistoryResult> deserializer,
                                      GetLiveChannelHistoryRequest request)
           : base(client, endpoint, context, deserializer)
        {
            _request = request;
        }

        public static GetLiveChannelHistoryCommand Create(IServiceClient client, Uri endpoint,
                                                   ExecutionContext context,
                                                   GetLiveChannelHistoryRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            return new GetLiveChannelHistoryCommand(client, endpoint, context,
                                           DeserializerFactory.GetFactory().CreateGetLiveChannelHistoryResultDeserializer(),
                                           request);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters =  new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_LIVE, null },
                    { RequestParameters.SUBRESOURCE_COMP, "history" }
                };
                return parameters;
            }
        }
    }
}

