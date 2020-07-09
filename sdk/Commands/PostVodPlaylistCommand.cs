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
    internal class PostVodPlaylistCommand : OssCommand
    {
        private readonly PostVodPlaylistRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }
        
        protected override string Key
        { 
            get { return _request.ChannelName + "/" +_request.PlaylistName; }
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
                    { RequestParameters.SUBRESOURCE_VOD, null},
                    { "endTime", DateUtils.FormatUnixTime(_request.EndTime)},
                    { "startTime", DateUtils.FormatUnixTime(_request.StartTime)},
                };
                return parameters;
            }
        }

        private PostVodPlaylistCommand(IServiceClient client, Uri endpoint, 
                                    ExecutionContext context,
                                    PostVodPlaylistRequest request)
            : base(client, endpoint, context)
        {
            _request = request;
        }

        public static PostVodPlaylistCommand Create(IServiceClient client, Uri endpoint, 
                                                 ExecutionContext context,
                                                 PostVodPlaylistRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            return new PostVodPlaylistCommand(client, endpoint, context, request);
        }
    }
}
