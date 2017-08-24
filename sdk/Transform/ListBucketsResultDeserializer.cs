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
    internal class ListBucketsResultDeserializer : ResponseDeserializer<ListBucketsResult, ListAllMyBucketsResult>
    {
        public ListBucketsResultDeserializer(IDeserializer<Stream, ListAllMyBucketsResult> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override ListBucketsResult Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);
            
            var result = new ListBucketsResult {Prefix = model.Prefix, Marker = model.Marker};
            if (model.MaxKeys.HasValue)
                result.MaxKeys = model.MaxKeys.Value;
            if (model.IsTruncated.HasValue)
                result.IsTruncated = model.IsTruncated.Value;
            result.NextMaker = model.NextMarker;

            var selectedBuckets = new List<Bucket>();
            foreach (var e in model.Buckets)
            {
                var newBucket = new Bucket(e.Name)
                {
                    Location = e.Location,
                    Owner = new Owner(model.Owner.Id, model.Owner.DisplayName),
                    CreationDate = e.CreationDate
                };
                selectedBuckets.Add(newBucket);
            }
            result.Buckets = selectedBuckets;

            DeserializeGeneric(xmlStream, result);

            return result;
        }
    }
}
