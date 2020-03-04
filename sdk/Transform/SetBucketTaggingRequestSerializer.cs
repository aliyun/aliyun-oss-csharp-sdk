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
    internal class SetBucketTaggingRequestSerializer : RequestSerializer<SetBucketTaggingRequest, Tagging>
    {
        public SetBucketTaggingRequestSerializer(ISerializer<Tagging, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(SetBucketTaggingRequest request)
        {
            var tags = request.Tags;
            var model = new Tagging
            {
                TagSet = new Tagging.TagSetModel()
            };

            model.TagSet.Tags = new Tagging.TagSetModel.Tag[tags.Count];

            for (var i = 0; i < tags.Count; i++)
            {
                model.TagSet.Tags[i] = new Tagging.TagSetModel.Tag
                {
                    Key = tags[i].Key,
                    Value = tags[i].Value
                };
            }
            return ContentSerializer.Serialize(model);
        }
    }
}
