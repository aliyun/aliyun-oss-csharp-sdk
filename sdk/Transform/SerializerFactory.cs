/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal abstract class SerializerFactory
    {
        public static SerializerFactory GetFactory(string contentType = null)
        {
            if (contentType == null || contentType.Contains("xml"))
            {
                return new XmlSerializerFactory();
            }

            // Ignore other content types, current only supports XML serializer factory.
            return null;
        }

        protected abstract ISerializer<T, Stream> CreateContentSerializer<T>();

        public ISerializer<CompleteMultipartUploadRequest, Stream> CreateCompleteUploadRequestSerializer()
        {
            return new CompleteMultipartUploadRequestSerializer(CreateContentSerializer<CompleteMultipartUploadRequestModel>());
        }

        public ISerializer<SetBucketLoggingRequest, Stream> CreateSetBucketLoggingRequestSerializer()
        {
            return new SetBucketLoggingRequestSerializer(CreateContentSerializer<SetBucketLoggingRequestModel>());
        }

        public ISerializer<SetBucketWebsiteRequest, Stream> CreateSetBucketWebsiteRequestSerializer()
        {
            return new SetBucketWebsiteRequestSerializer(CreateContentSerializer<SetBucketWebsiteRequestModel>());
        }

        public ISerializer<SetBucketStorageCapacityRequest, Stream> CreateSetBucketStorageCapacityRequestSerializer()
        {
            return new SetBucketStorageCapacityRequestSerializer(CreateContentSerializer<BucketStorageCapacityModel>());
        }

        public ISerializer<SetBucketCorsRequest, Stream> CreateSetBucketCorsRequestSerializer()
        {
            return new SetBucketCorsRequestSerializer(CreateContentSerializer<SetBucketCorsRequestModel>());
        }

        public ISerializer<DeleteObjectsRequest, Stream> CreateDeleteObjectsRequestSerializer()
        {
            return new DeleteObjectsRequestSerializer(CreateContentSerializer<DeleteObjectsRequestModel>());
        }

        public ISerializer<SetBucketRefererRequest, Stream> CreateSetBucketRefererRequestSerializer()
        {
            return new SetBucketRefererRequestSerializer(CreateContentSerializer<RefererConfiguration>());
        }

        public ISerializer<SetBucketLifecycleRequest, Stream> CreateSetBucketLifecycleRequestSerializer()
        {
            return new SetBucketLifecycleRequestSerializer(CreateContentSerializer<LifecycleConfiguration>());
        }

        public ISerializer<StorageClass, Stream> CreateCreateBucketSerialization()
        {
            return new CreateBucketRequestSerializer(CreateContentSerializer<CreateBucketRequestModel>());
        }
    }

    internal class XmlSerializerFactory : SerializerFactory
    {
        protected override ISerializer<T, Stream> CreateContentSerializer<T>()
        {
            return new XmlStreamSerializer<T>();
        }
    }
}
