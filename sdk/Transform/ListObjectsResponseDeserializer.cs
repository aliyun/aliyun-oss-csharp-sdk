/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class ListObjectsResponseDeserializer : ResponseDeserializer<ObjectListing, ListBucketResult>
    {
        public ListObjectsResponseDeserializer(IDeserializer<Stream, ListBucketResult> contentDeserializer)
            : base(contentDeserializer)
        { }
        
        public override ObjectListing Deserialize(ServiceResponse xmlStream)
        {
            var listBucketResult = ContentDeserializer.Deserialize(xmlStream.Content);
            string encodeType = listBucketResult.EncodingType == null ?
                                string.Empty : listBucketResult.EncodingType.ToLowerInvariant();

            var objectList = new ObjectListing(listBucketResult.Name)
            {
                Delimiter = Decode(listBucketResult.Delimiter, encodeType),
                IsTruncated = listBucketResult.IsTruncated,
                Marker = Decode(listBucketResult.Marker, encodeType),
                MaxKeys = listBucketResult.MaxKeys,
                NextMarker = Decode(listBucketResult.NextMarker, encodeType),
                Prefix = Decode(listBucketResult.Prefix, encodeType)
            };

            if (listBucketResult.Contents != null)
            {
                foreach(var contents in listBucketResult.Contents)
                {
                    var summary = new OssObjectSummary
                    {
                        BucketName = listBucketResult.Name,
                        Key = Decode(contents.Key, encodeType),
                        LastModified = contents.LastModified,
                        ETag = contents.ETag != null ? OssUtils.TrimQuotes(contents.ETag) : string.Empty,
                        Size = contents.Size,
                        StorageClass = contents.StorageClass,
                        Owner = contents.Owner != null ? 
                                new Owner(contents.Owner.Id, contents.Owner.DisplayName) : null
                    };

                    objectList.AddObjectSummary(summary);
                }
            }

            if (listBucketResult.CommonPrefixes != null)
            {
                foreach(var commonPrefixes in listBucketResult.CommonPrefixes)
                {
                    if (commonPrefixes.Prefix != null)
                    {
                        foreach(var prefix in commonPrefixes.Prefix)
                        {
                            objectList.AddCommonPrefix(Decode(prefix, encodeType));
                        }
                    }
                }
            }

            DeserializeGeneric(xmlStream, objectList);

            return objectList;
        }

    }
}
