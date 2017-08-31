/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Model;
using System.IO;

namespace Aliyun.OSS.Transform
{
    internal class SetBucketWebsiteRequestSerializer : RequestSerializer<SetBucketWebsiteRequest, SetBucketWebsiteRequestModel>
    {
        public SetBucketWebsiteRequestSerializer(ISerializer<SetBucketWebsiteRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(SetBucketWebsiteRequest request)
        {
            var model = new SetBucketWebsiteRequestModel
            {
                ErrorDocument = new SetBucketWebsiteRequestModel.ErrorDocumentModel(),
                IndexDocument = new SetBucketWebsiteRequestModel.IndexDocumentModel {Suffix = request.IndexDocument}
            };

            model.ErrorDocument.Key = request.ErrorDocument;
            
            return ContentSerializer.Serialize(model);
        }
    }
}
