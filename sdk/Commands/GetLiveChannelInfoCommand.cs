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
    internal class GetLiveChannelInfoCommand : OssCommand<GetLiveChannelInfoResult>
    {
        private readonly GetLiveChannelInfoRequest _request;

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

        private GetLiveChannelInfoCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                      IDeserializer<ServiceResponse, GetLiveChannelInfoResult> deserializer,
                                      GetLiveChannelInfoRequest request)
           : base(client, endpoint, context, deserializer)
        {
            _request = request;
        }

        public static GetLiveChannelInfoCommand Create(IServiceClient client, Uri endpoint,
                                                   ExecutionContext context,
                                                   GetLiveChannelInfoRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            return new GetLiveChannelInfoCommand(client, endpoint, context,
                                           DeserializerFactory.GetFactory().CreateGetLiveChannelInfoResultDeserializer(),
                                           request);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters =  new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_LIVE, null }
                };
                return parameters;
            }
        }
    }
}

