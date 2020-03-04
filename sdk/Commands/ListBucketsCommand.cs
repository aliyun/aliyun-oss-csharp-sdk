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
    internal class ListBucketsCommand : OssCommand<ListBucketsResult>
    {
        private readonly ListBucketsRequest _request;

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                if (_request != null)
                    Populate(_request, parameters);
                return parameters;
            }
        }

        private static void Populate(ListBucketsRequest request, IDictionary<string, string> parameters)
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

            if (request.Tag != null)
            {
                if (request.Tag.Key != null)
                {
                    parameters[RequestParameters.TAG_KEY] = request.Tag.Key;
                }

                if (request.Tag.Value != null)
                {
                    parameters[RequestParameters.TAG_VALUE] = request.Tag.Value;
                }
            }
        }

        private ListBucketsCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                  IDeserializer<ServiceResponse, ListBucketsResult> deserializeMethod,
                                  ListBucketsRequest request)
            : base(client, endpoint, context, deserializeMethod)
        {
            _request = request;
        }
        
        public static ListBucketsCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
            ListBucketsRequest request)
        {
            return new ListBucketsCommand(client, endpoint, context,
                                          DeserializerFactory.GetFactory().CreateListBucketResultDeserializer(), request);
        }
    }
}
