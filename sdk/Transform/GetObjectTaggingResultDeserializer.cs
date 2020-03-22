/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class GetObjectTaggingResultDeserializer : ResponseDeserializer<GetObjectTaggingResult, Tagging>
    {
        public GetObjectTaggingResultDeserializer(IDeserializer<Stream, Tagging> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override GetObjectTaggingResult Deserialize(ServiceResponse xmlStream)
        {
            GetObjectTaggingResult result = new GetObjectTaggingResult();

            var tagging = ContentDeserializer.Deserialize(xmlStream.Content);

            if (tagging.TagSet != null && tagging.TagSet.Tags != null)
            {
                foreach (var lcc in tagging.TagSet.Tags)
                {
                    var tag = new Tag
                    {
                        Key = lcc.Key,
                        Value = lcc.Value
                    };
                    result.Addtag(tag);
                }
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.VersionId))
            {
                result.VersionId = xmlStream.Headers[HttpHeaders.VersionId];
            }

            DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}

