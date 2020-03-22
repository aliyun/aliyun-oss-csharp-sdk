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
    internal class ListObjectVersionsResponseDeserializer : ResponseDeserializer<ObjectVersionList, ListVersionsResult>
    {
        public ListObjectVersionsResponseDeserializer(IDeserializer<Stream, ListVersionsResult> contentDeserializer)
            : base(contentDeserializer)
        { }
        
        public override ObjectVersionList Deserialize(ServiceResponse xmlStream)
        {
            var model = ContentDeserializer.Deserialize(xmlStream.Content);
            string encodeType = model.EncodingType == null ?
                                string.Empty : model.EncodingType.ToLowerInvariant();

            var result = new ObjectVersionList(model.Name)
            {
                Delimiter = Decode(model.Delimiter, encodeType),
                Prefix = Decode(model.Prefix, encodeType),
                KeyMarker = Decode(model.KeyMarker, encodeType),
                NextKeyMarker = Decode(model.NextKeyMarker, encodeType),
                VersionIdMarker = model.VersionIdMarker,
                NextVersionIdMarker = model.NextVersionIdMarker
            };

            if (model.MaxKeys.HasValue)
                result.MaxKeys = model.MaxKeys.Value;
            if (model.IsTruncated.HasValue)
                result.IsTruncated = model.IsTruncated.Value;

            if (model.ObjectVersions != null)
            {
                foreach(var objectVersion in model.ObjectVersions)
                {
                    var summary = new ObjectVersionSummary
                    {
                        BucketName = model.Name,
                        Key = Decode(objectVersion.Key, encodeType),
                        VersionId = objectVersion.VersionId,
                        IsLatest = objectVersion.IsLatest,
                        LastModified = objectVersion.LastModified,
                        ETag = objectVersion.ETag != null ? OssUtils.TrimQuotes(objectVersion.ETag) : string.Empty,
                        Size = objectVersion.Size,
                        StorageClass = objectVersion.StorageClass,
                        Type = objectVersion.Type,
                        Owner = objectVersion.Owner != null ? 
                                new Owner(objectVersion.Owner.Id, objectVersion.Owner.DisplayName) : null
                    };

                    result.AddObjectVersionSummary(summary);
                }
            }

            if (model.ObjectDeleteMarkers != null)
            {
                foreach (var deleteMarker in model.ObjectDeleteMarkers)
                {
                    var summary = new DeleteMarkerSummary
                    {
                        BucketName = model.Name,
                        Key = Decode(deleteMarker.Key, encodeType),
                        VersionId = deleteMarker.VersionId,
                        IsLatest = deleteMarker.IsLatest,
                        LastModified = deleteMarker.LastModified,
                        Owner = deleteMarker.Owner != null ?
                                new Owner(deleteMarker.Owner.Id, deleteMarker.Owner.DisplayName) : null
                    };

                    result.AddDeleteMarkerSummary(summary);
                }
            }

            if (model.CommonPrefixes != null)
            {
                foreach(var prefixes in model.CommonPrefixes)
                {
                    if (prefixes.Prefix != null)
                    {
                        result.AddCommonPrefix(Decode(prefixes.Prefix, encodeType));
                    }
                }
            }

            DeserializeGeneric(xmlStream, result);

            return result;
        }

    }
}
