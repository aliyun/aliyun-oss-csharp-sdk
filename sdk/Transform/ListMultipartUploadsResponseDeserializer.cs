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
    internal class ListMultipartUploadsResponseDeserializer : ResponseDeserializer<MultipartUploadListing, ListMultipartUploadsResult>
    {
        public ListMultipartUploadsResponseDeserializer(IDeserializer<Stream, ListMultipartUploadsResult> contentDeserializer)
            : base(contentDeserializer)
        { }
             
        public override MultipartUploadListing Deserialize(ServiceResponse xmlStream)
        {
            var listMultipartUploadsResult = ContentDeserializer.Deserialize(xmlStream.Content);
            string encodeType = listMultipartUploadsResult.EncodingType == null ? 
                                string.Empty : listMultipartUploadsResult.EncodingType.ToLowerInvariant();

            var uploadsList = new MultipartUploadListing(listMultipartUploadsResult.Bucket)
            {
                BucketName = listMultipartUploadsResult.Bucket,
                Delimiter = Decode(listMultipartUploadsResult.Delimiter, encodeType),
                IsTruncated = listMultipartUploadsResult.IsTruncated,
                KeyMarker = Decode(listMultipartUploadsResult.KeyMarker, encodeType),
                MaxUploads = listMultipartUploadsResult.MaxUploads,
                NextKeyMarker = Decode(listMultipartUploadsResult.NextKeyMarker, encodeType),
                NextUploadIdMarker = listMultipartUploadsResult.NextUploadIdMarker,
                Prefix = Decode(listMultipartUploadsResult.Prefix, encodeType),
                UploadIdMarker = listMultipartUploadsResult.UploadIdMarker
            };

            if (listMultipartUploadsResult.CommonPrefix != null)
            {
                if (listMultipartUploadsResult.CommonPrefix.Prefixs != null)
                {
                    foreach (var prefix in listMultipartUploadsResult.CommonPrefix.Prefixs)
                    {
                        uploadsList.AddCommonPrefix(Decode(prefix, encodeType));
                    }
                }
            }
            
            if (listMultipartUploadsResult.Uploads != null)
            {
                foreach (var uploadResult in listMultipartUploadsResult.Uploads)
                {
                    var upload = new MultipartUpload
                    {
                        Initiated = uploadResult.Initiated,
                        Key = Decode(uploadResult.Key, encodeType),
                        UploadId = uploadResult.UploadId,
                        StorageClass = uploadResult.StorageClass
                    };
                    uploadsList.AddMultipartUpload(upload);
                }
            }

            DeserializeGeneric(xmlStream, uploadsList);

            return uploadsList;
        }
    }
}
