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
    internal class GetBucketTaggingResultDeserializer : ResponseDeserializer<GetBucketTaggingResult, Tagging>
    {
        public GetBucketTaggingResultDeserializer(IDeserializer<Stream, Tagging> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override GetBucketTaggingResult Deserialize(ServiceResponse xmlStream)
        {
            GetBucketTaggingResult result = new GetBucketTaggingResult();

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
                    result.AddTag(tag);
                }
            }

            DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}
