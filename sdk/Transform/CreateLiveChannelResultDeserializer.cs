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

namespace Aliyun.OSS.Transform
{
    internal class CreateLiveChannelResultDeserializer : ResponseDeserializer<CreateLiveChannelResult, CreateLiveChannelResultModel>
    {
        public CreateLiveChannelResultDeserializer(IDeserializer<Stream, CreateLiveChannelResultModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override CreateLiveChannelResult Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);

            CreateLiveChannelResult result = new CreateLiveChannelResult();

            if (model.PublishUrls != null)
            {
                result.PublishUrl = model.PublishUrls.Url;
            }

            if (model.PlayUrls != null)
            {
                result.PlayUrl = model.PlayUrls.Url;
            }

            DeserializeGeneric(xmlStream, result);

            return result;
        }
    }
}
