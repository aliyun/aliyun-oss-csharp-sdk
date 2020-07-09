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
    internal class GetVodPlaylistCommand : OssCommand<GetVodPlaylistResult>
    {
        private readonly GetVodPlaylistRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Get; }
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
                    { RequestParameters.SUBRESOURCE_VOD, null},
                    { "endTime", DateUtils.FormatUnixTime(_request.EndTime)},
                    { "startTime", DateUtils.FormatUnixTime(_request.StartTime)},
                };
                return parameters;
            }
        }

        private GetVodPlaylistCommand(IServiceClient client, Uri endpoint, 
                                    ExecutionContext context, IDeserializer<ServiceResponse, GetVodPlaylistResult> deserializer,
                                    GetVodPlaylistRequest request)
            : base(client, endpoint, context, deserializer)
        {
            _request = request;
        }

        public static GetVodPlaylistCommand Create(IServiceClient client, Uri endpoint, 
                                                 ExecutionContext context,
                                                 GetVodPlaylistRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            return new GetVodPlaylistCommand(client, endpoint, context,
                DeserializerFactory.GetFactory().CreateGetVodPlaylistResultDeserializer(),
                request);
        }
    }
}
