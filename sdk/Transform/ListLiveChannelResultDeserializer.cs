/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class ListLiveChannelResultDeserializer : ResponseDeserializer<ListLiveChannelResult, ListLiveChannelResultModel>
    {
        public ListLiveChannelResultDeserializer(IDeserializer<Stream, ListLiveChannelResultModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override ListLiveChannelResult Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);
            
            var result = new ListLiveChannelResult
            {
                Prefix = model.Prefix,
                Marker = model.Marker
            };

            if (model.MaxKeys.HasValue)
                result.MaxKeys = model.MaxKeys.Value;

            if (model.IsTruncated.HasValue)
                result.IsTruncated = model.IsTruncated.Value;

            result.NextMarker = model.NextMarker;

            var liveChannels = new List<LiveChannel>();
            if (model.LiveChannels != null)
            {
                foreach (var e in model.LiveChannels)
                {
                    var liveChannel = new LiveChannel()
                    {
                        Name = e.Name,
                        Description = e.Description,
                        Status = e.Status,
                        LastModified = e.LastModified
                    };

                    if (e.PublishUrls != null)
                    {
                        liveChannel.PublishUrl = e.PublishUrls.Url;
                    }

                    if (e.PlayUrls != null)
                    {
                        liveChannel.PlayUrl = e.PlayUrls.Url;
                    }

                    liveChannels.Add(liveChannel);
                }
            }
            result.LiveChannels = liveChannels;

            DeserializeGeneric(xmlStream, result);

            return result;
        }
    }
}
