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
    internal class CreateLiveChannelCommand : OssCommand<CreateLiveChannelResult>
    {
        private readonly CreateLiveChannelRequest _request;

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

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateCreateLiveChannelRequestSerializer()
                    .Serialize(_request);
            }
        }

        private CreateLiveChannelCommand(IServiceClient client, Uri endpoint, 
                                    ExecutionContext context, IDeserializer<ServiceResponse, CreateLiveChannelResult> deserializer,
                                    CreateLiveChannelRequest request)
            : base(client, endpoint, context, deserializer)
        {
            _request = request;
        }

        public static CreateLiveChannelCommand Create(IServiceClient client, Uri endpoint, 
                                                 ExecutionContext context,
                                                 CreateLiveChannelRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            return new CreateLiveChannelCommand(client, endpoint, context, 
                DeserializerFactory.GetFactory().CreateCreateLiveChannelResultDeserializer(), 
                request);
        }
    }
}
