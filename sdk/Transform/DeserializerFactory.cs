/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal abstract class DeserializerFactory
    {
        public static DeserializerFactory GetFactory()
        {
            return GetFactory(null);
        }

        public static DeserializerFactory GetFactory(string contentType)
        {
            // Use XML for default.
            if (contentType == null)
                contentType = "text/xml";

            if (contentType.Contains("xml"))
                return new XmlDeserializerFactory();

            return null;
        }

        protected abstract IDeserializer<Stream, T> CreateContentDeserializer<T>();

        public IDeserializer<ServiceResponse, ErrorResult> CreateErrorResultDeserializer()
        {
            return new SimpleResponseDeserializer<ErrorResult>(CreateContentDeserializer<ErrorResult>());
        }

        public IDeserializer<ServiceResponse, ListBucketsResult> CreateListBucketResultDeserializer()
        {
            return new ListBucketsResultDeserializer(CreateContentDeserializer<ListAllMyBucketsResult>());
        }

        public IDeserializer<ServiceResponse, AccessControlList> CreateGetAclResultDeserializer()
        {
            return new GetAclResponseDeserializer(CreateContentDeserializer<AccessControlPolicy>());
        }

        public IDeserializer<ServiceResponse, IList<CORSRule>> CreateGetCorsResultDeserializer()
        {
            return new GetCorsResponseDeserializer(CreateContentDeserializer<SetBucketCorsRequestModel>());
        }

        public IDeserializer<ServiceResponse, BucketLoggingResult> CreateGetBucketLoggingResultDeserializer()
        {
            return new GetBucketLoggingResultDeserializer(CreateContentDeserializer<SetBucketLoggingRequestModel>());
        }

        public IDeserializer<ServiceResponse, BucketWebsiteResult> CreateGetBucketWebSiteResultDeserializer()
        {
            return new GetBucketWebSiteResultDeserializer(CreateContentDeserializer<SetBucketWebsiteRequestModel>());
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceResponse, PutObjectResult> CreatePutObjectReusltDeserializer()
        {
            return new PutObjectResponseDeserializer();
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceResponse, AppendObjectResult> CreateAppendObjectReusltDeserializer()
        {
            return new AppendObjectResponseDeserializer();
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceResponse, OssObject> CreateGetObjectResultDeserializer(GetObjectRequest request)
        {
            return new GetObjectResponseDeserializer(request);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceResponse, ObjectMetadata> CreateGetObjectMetadataResultDeserializer()
        {
            return new GetObjectMetadataResponseDeserializer();
        }

        public IDeserializer<ServiceResponse, ObjectListing> CreateListObjectsResultDeserializer()
        {
            return new ListObjectsResponseDeserializer(CreateContentDeserializer<ListBucketResult>());
        }

        public IDeserializer<ServiceResponse, MultipartUploadListing> CreateListMultipartUploadsResultDeserializer()
        {
            return new ListMultipartUploadsResponseDeserializer(CreateContentDeserializer<ListMultipartUploadsResult>());
        }

        public IDeserializer<ServiceResponse, InitiateMultipartUploadResult> CreateInitiateMultipartUploadResultDeserializer()
        {
            return new InitiateMultipartUploadResultDeserializer(CreateContentDeserializer<InitiateMultipartResult>());
        }

        public IDeserializer<ServiceResponse, UploadPartResult> CreateUploadPartResultDeserializer(int partNumber)
        {
            return new UploadPartResultDeserializer(partNumber);
        }

        public IDeserializer<ServiceResponse, UploadPartCopyResult> CreateUploadPartCopyResultDeserializer(int partNumber)
        {
            return new UploadPartCopyResultDeserializer(CreateContentDeserializer<UploadPartCopyRequestModel>(), partNumber);
        }

        public IDeserializer<ServiceResponse, PartListing> CreateListPartsResultDeserializer()
        {
            return new ListPartsResponseDeserializer(CreateContentDeserializer<ListPartsResult>());
        }

        public IDeserializer<ServiceResponse, CompleteMultipartUploadResult> CreateCompleteUploadResultDeserializer()
        {
            return new CompleteMultipartUploadResultDeserializer(CreateContentDeserializer<CompleteMultipartUploadResultModel>());
        }

        public IDeserializer<ServiceResponse, CopyObjectResult> CreateCopyObjectResultDeserializer()
        {
            return new CopyObjectResultDeserializer(CreateContentDeserializer<CopyObjectResultModel>());
        }

        public IDeserializer<ServiceResponse, DeleteObjectsResult> CreateDeleteObjectsResultDeserializer()
        {
            return new DeleteObjectsResultDeserializer(CreateContentDeserializer<DeleteObjectsResult>());
        }

        public IDeserializer<ServiceResponse, RefererConfiguration> CreateGetBucketRefererResultDeserializer()
        {
            return new SimpleResponseDeserializer<RefererConfiguration>(CreateContentDeserializer<RefererConfiguration>());
        }

        public IDeserializer<ServiceResponse, IList<LifecycleRule>> CreateGetBucketLifecycleDeserializer()
        {
            return new GetBucketLifecycleDeserializer(CreateContentDeserializer<LifecycleConfiguration>());
        }
    }

    internal class XmlDeserializerFactory : DeserializerFactory
    {
        protected override IDeserializer<Stream, T> CreateContentDeserializer<T>()
        {
            return new XmlStreamDeserializer<T>();
        }
    }
}
