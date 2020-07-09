/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Commands
{
    internal class ListLiveChannelCommand : OssCommand<ListLiveChannelResult>
    {
        private readonly ListLiveChannelRequest _request;

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_LIVE, null},
                };

                if (_request != null)
                    Populate(_request, parameters);
                return parameters;
            }
        }

        private static void Populate(ListLiveChannelRequest request, IDictionary<string, string> parameters)
        {
            if (request.Prefix != null)
            {
                parameters[RequestParameters.PREFIX] = request.Prefix;
            }

            if (request.Marker != null)
            {
                parameters[RequestParameters.MARKER] = request.Marker;
            }

            if (request.MaxKeys.HasValue)
            {
                parameters[RequestParameters.MAX_KEYS] = request.MaxKeys.Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        private ListLiveChannelCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                  IDeserializer<ServiceResponse, ListLiveChannelResult> deserializeMethod,
                                  ListLiveChannelRequest request)
            : base(client, endpoint, context, deserializeMethod)
        {
            _request = request;
        }
        
        public static ListLiveChannelCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
            ListLiveChannelRequest request)
        {
            return new ListLiveChannelCommand(client, endpoint, context,
                                          DeserializerFactory.GetFactory().CreateListLiveChannelResultDeserializer(), request);
        }
    }
}
