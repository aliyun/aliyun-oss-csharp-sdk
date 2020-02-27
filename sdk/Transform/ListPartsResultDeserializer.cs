/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */
 
using System;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class ListPartsResponseDeserializer : ResponseDeserializer<PartListing, ListPartsResult>
    {
        public ListPartsResponseDeserializer(IDeserializer<Stream, ListPartsResult> contentDeserializer)
            : base(contentDeserializer)
        { }
        
        public override PartListing Deserialize(ServiceResponse xmlStream)
        {
            var listPartResult = ContentDeserializer.Deserialize(xmlStream.Content);

            var partListing = new PartListing
            {
                BucketName = listPartResult.Bucket,
                Key = (listPartResult.EncodingType != null && listPartResult.EncodingType.ToLowerInvariant().Equals(HttpUtils.UrlEncodingType)) ? HttpUtils.DecodeUri(listPartResult.Key) : listPartResult.Key,
                MaxParts = listPartResult.MaxParts,
                NextPartNumberMarker = listPartResult.NextPartNumberMarker.Length == 0 ? 
                    0 : Convert.ToInt32(listPartResult.NextPartNumberMarker),
                PartNumberMarker = listPartResult.PartNumberMarker,
                UploadId = listPartResult.UploadId,
                IsTruncated = listPartResult.IsTruncated
            };

            if (listPartResult.PartResults != null)
            {
                foreach (var partResult in listPartResult.PartResults)
                {
                    var part = new Part
                    {
                        ETag = partResult.ETag != null ? OssUtils.TrimQuotes(partResult.ETag) : string.Empty,
                        LastModified = partResult.LastModified,
                        PartNumber = partResult.PartNumber,
                        Size = partResult.Size
                    };
                    partListing.AddPart(part);
                }
            }

            DeserializeGeneric(xmlStream, partListing);

            return partListing;
        }
    }
}
