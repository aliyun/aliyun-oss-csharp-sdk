/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Util;
using Aliyun.OSS.Model;
using Aliyun.OSS.Domain;

namespace Aliyun.OSS.Transform
{
    internal class CreateLiveChannelRequestSerializer : RequestSerializer<CreateLiveChannelRequest, LiveChannelConfiguration>
    {
        public CreateLiveChannelRequestSerializer(ISerializer<LiveChannelConfiguration, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(CreateLiveChannelRequest request)
        {
            var model = new LiveChannelConfiguration
            {
                Description = request.Description,
                Status = request.Status,
            };
           
            if (!string.IsNullOrEmpty(request.Type))
            {
                model.Target = new LiveChannelConfiguration.TargetModel()
                {
                    Type = request.Type,
                    FragDuration = request.FragDuration,
                    FragCount = request.FragCount,
                    PlaylistName = request.PlaylistName
                };
            }

            if (!string.IsNullOrEmpty(request.DestBucket))
            {
                model.Snapshot = new LiveChannelConfiguration.SnapshotModel()
                {
                    RoleName = request.RoleName,
                    DestBucket = request.DestBucket,
                    NotifyTopic = request.NotifyTopic,
                    Interval = request.Interval
                };
            }

            return ContentSerializer.Serialize(model);
        }
    }
}

