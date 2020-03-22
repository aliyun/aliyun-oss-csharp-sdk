/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class ListObjectVersionsCommand : OssCommand<ObjectVersionList>
    {
        private readonly ListObjectVersionsRequest _request;

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }  

        protected override IDictionary<string, string> Parameters
        {
            get 
            {
                var parameters = base.Parameters;
                parameters[RequestParameters.VERSIONS] = null;
                Populate(_request, parameters);
                return parameters;
            }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                if (_request.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                return headers;
            }
        }

        private ListObjectVersionsCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                   IDeserializer<ServiceResponse, ObjectVersionList> deserializer,
                                   ListObjectVersionsRequest request)
            : base(client, endpoint, context, deserializer)
        {
            _request = request;
        }
        
        public static ListObjectVersionsCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                ListObjectVersionsRequest request)
        {
            return new ListObjectVersionsCommand(client, endpoint, context,
                                          DeserializerFactory.GetFactory().CreateListObjectVersionsResultDeserializer(),
                                          request);
        }

        private static void Populate(ListObjectVersionsRequest request, IDictionary<string, string> parameters)
        {
            if (request.Prefix != null) 
            {
                parameters[RequestParameters.PREFIX] = request.Prefix;
            }

            if (request.KeyMarker != null) 
            {
                parameters[RequestParameters.KEY_MARKER] = request.KeyMarker;
            }

            if (request.VersionIdMarker != null)
            {
                parameters[RequestParameters.VERSION_ID_MARKER] = request.VersionIdMarker;
            }

            if (request.Delimiter != null) 
            {
                parameters[RequestParameters.DELIMITER] = request.Delimiter;
            }

            if (request.MaxKeys.HasValue) 
            {
                parameters[RequestParameters.MAX_KEYS] =
                    request.MaxKeys.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (request.EncodingType != null)
            {
                parameters[RequestParameters.ENCODING_TYPE] = request.EncodingType;
            }
        }
    }
}
