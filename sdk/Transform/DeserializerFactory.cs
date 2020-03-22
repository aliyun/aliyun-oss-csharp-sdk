/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
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

        public IDeserializer<ServiceResponse, BucketInfo> CreateGetBucketInfoDeserializer()
        {
            return new GetBucketInfoDeserializer(CreateContentDeserializer<BucketInfo>());
        }

        public IDeserializer<ServiceResponse, BucketStat> CreateGetBucketStatDeserializer()
        {
            return new GetBucketStatDeserializer(CreateContentDeserializer<BucketStat>());
        }

        public IDeserializer<ServiceResponse, AccessControlList> CreateGetAclResultDeserializer()
        {
            return new GetAclResponseDeserializer(CreateContentDeserializer<AccessControlPolicy>());
        }

        public IDeserializer<ServiceResponse, BucketLocationResult> CreateGetBucketLocationResultDeserializer()
        {
            return new GetBucketLocationResultDeserializer(CreateContentDeserializer<BucketLocationResult>());
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceResponse, BucketMetadata> CreateGetBucketMetadataResultDeserializer()
        {
            return new GetBucketMetadataResponseDeserializer();
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

        public IDeserializer<ServiceResponse, GetBucketStorageCapacityResult> CreateGetBucketStorageCapacityResultDeserializer()
        {
            return new GetBucketStorageCapacityResultDeserializer(CreateContentDeserializer<BucketStorageCapacityModel>());
        }

        public IDeserializer<ServiceResponse, GetBucketPolicyResult> CreateGetBucketPolicyDeserializer()
        {
            return new GetBucketPolicyDeserializer(CreateContentDeserializer<GetBucketPolicyResult>());
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceResponse, PutObjectResult> CreatePutObjectReusltDeserializer(PutObjectRequest request)
        {
            return new PutObjectResponseDeserializer(request);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceResponse, AppendObjectResult> CreateAppendObjectResultDeserializer()
        {
            return new AppendObjectResponseDeserializer();
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IDeserializer<ServiceResponse, OssObject> CreateGetObjectResultDeserializer(GetObjectRequest request, IServiceClient client)
        {
            return new GetObjectResponseDeserializer(request, client);
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

        public IDeserializer<ServiceResponse, ObjectVersionList> CreateListObjectVersionsResultDeserializer()
        {
            return new ListObjectVersionsResponseDeserializer(CreateContentDeserializer<ListVersionsResult>());
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

        public IDeserializer<ServiceResponse, UploadPartResult> CreateUploadPartResultDeserializer(int partNumber, long length)
        {
            return new UploadPartResultDeserializer(partNumber, length);
        }

        public IDeserializer<ServiceResponse, UploadPartCopyResult> CreateUploadPartCopyResultDeserializer(int partNumber)
        {
            return new UploadPartCopyResultDeserializer(CreateContentDeserializer<UploadPartCopyRequestModel>(), partNumber);
        }

        public IDeserializer<ServiceResponse, PartListing> CreateListPartsResultDeserializer()
        {
            return new ListPartsResponseDeserializer(CreateContentDeserializer<ListPartsResult>());
        }

        public IDeserializer<ServiceResponse, CompleteMultipartUploadResult> CreateCompleteUploadResultDeserializer(CompleteMultipartUploadRequest request)
        {
            return new CompleteMultipartUploadResultDeserializer(CreateContentDeserializer<CompleteMultipartUploadResultModel>(), request);
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

        public IDeserializer<ServiceResponse, RestoreObjectResult> CreateRestoreObjectResultDeserializer()
        {
            return new RestoreObjectResultDeserializer(CreateContentDeserializer<ErrorResult>());
        }

        public IDeserializer<ServiceResponse, GetBucketTaggingResult> CreateGetBucketTaggingResultDeserializer()
        {
            return new GetBucketTaggingResultDeserializer(CreateContentDeserializer<Tagging>());
        }

        public IDeserializer<ServiceResponse, GetObjectTaggingResult> CreateGetObjectTaggingResultDeserializer()
        {
            return new GetObjectTaggingResultDeserializer(CreateContentDeserializer<Tagging>());
        }

        public IDeserializer<ServiceResponse, GetBucketRequestPaymentResult> CreateGetBucketRequestPaymentResultDeserializer()
        {
            return new GetBucketRequestPaymentResultDeserializer(CreateContentDeserializer<RequestPaymentConfiguration>());
        }

        public IDeserializer<ServiceResponse, BucketEncryptionResult> CreateGetBucketEncryptionResultDeserializer()
        {
            return new GetBucketEncryptionResultDeserializer(CreateContentDeserializer<ServerSideEncryptionRule>());
        }

        public IDeserializer<ServiceResponse, GetBucketVersioningResult> CreateGetBucketVersioningResultDeserializer()
        {
            return new GetBucketVersioningResultDeserializer(CreateContentDeserializer<VersioningConfiguration>());
        }

        public IDeserializer<ServiceResponse, DeleteObjectVersionsResult> CreateDeleteObjectVersionsResultDeserializer()
        {
            return new DeleteObjectVersionsResultDeserializer(CreateContentDeserializer<DeleteObjectVersionsResultModel>());
        }

        public IDeserializer<ServiceResponse, CreateSymlinkResult> CreateCreateSymlinkResultDeserializer()
        {
            return new CreateSymlinkResultDeserializer(CreateContentDeserializer<Stream>());
        }

        public IDeserializer<ServiceResponse, DeleteObjectResult> CreateDeleteObjectResultDeserializer()
        {
            return new DeleteObjectResultDeserializer(CreateContentDeserializer<Stream>());
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
