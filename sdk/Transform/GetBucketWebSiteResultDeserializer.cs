/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using System.IO;

namespace Aliyun.OSS.Transform
{
    internal class GetBucketWebSiteResultDeserializer : ResponseDeserializer<BucketWebsiteResult, SetBucketWebsiteRequestModel>
    {
        public GetBucketWebSiteResultDeserializer(IDeserializer<Stream, SetBucketWebsiteRequestModel> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override BucketWebsiteResult Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);
            var bucketWebsiteResult = new BucketWebsiteResult
            {
                IndexDocument = model.IndexDocument.Suffix,
                ErrorDocument = model.ErrorDocument.Key
            };

            DeserializeGeneric(xmlStream, bucketWebsiteResult);

            return bucketWebsiteResult;
        }
    }
}
