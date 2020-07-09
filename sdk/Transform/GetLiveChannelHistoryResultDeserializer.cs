/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Model;
using System.Collections.Generic;

namespace Aliyun.OSS.Transform
{
    internal class GetLiveChannelHistoryResultDeserializer : ResponseDeserializer<GetLiveChannelHistoryResult, LiveChannelHistory>
    {
        public GetLiveChannelHistoryResultDeserializer(IDeserializer<Stream, LiveChannelHistory> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override GetLiveChannelHistoryResult Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);

            GetLiveChannelHistoryResult result = new GetLiveChannelHistoryResult();

            var liveRecords = new List<LiveRecord>();
            if (model.LiveRecords != null)
            {
                foreach (var e in model.LiveRecords)
                {
                    var liveRecord = new LiveRecord()
                    {
                        StartTime = e.StartTime,
                        EndTime = e.EndTime,
                        RemoteAddr = e.RemoteAddr
                    };
                    liveRecords.Add(liveRecord);
                }
            }
            result.LiveRecords = liveRecords;

            DeserializeGeneric(xmlStream, result);

            return result;
        }
    }
}
