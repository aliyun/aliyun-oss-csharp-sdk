/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Commands
{
    internal class CreateBucketCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly CreateBucketRequest _createBucketRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        protected override Stream Content
        {
            get
            {
                return StorageClass == null ? null : SerializerFactory.GetFactory().CreateCreateBucketSerialization()
                                        .Serialize(_createBucketRequest);
            }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                if (ACL != null && ACL != CannedAccessControlList.Default)
                {
                    headers[OssHeaders.OssCannedAcl] = EnumUtils.GetStringValue(ACL);
                }
                return headers;
            }
        }

        protected StorageClass? StorageClass
        {
            get;
            set;
        }

        protected CannedAccessControlList? ACL
        {
            get;
            set;
        }

        private CreateBucketCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, StorageClass? storageClass)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            _bucketName = bucketName;
            StorageClass = storageClass;
            ACL = null;
            if (storageClass != null) { 
                _createBucketRequest = new CreateBucketRequest(bucketName, storageClass.Value, CannedAccessControlList.Default);
            }
        }

        public static CreateBucketCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 string bucketName, StorageClass? storageClass = null)
        {
            return new CreateBucketCommand(client, endpoint, context, bucketName, storageClass);
        }

        private CreateBucketCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                            string bucketName, CreateBucketRequest createBucketRequest)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            _bucketName = bucketName;
            _createBucketRequest = createBucketRequest;
            StorageClass = createBucketRequest.StorageClass;
            ACL = createBucketRequest.ACL;
        }

        public static CreateBucketCommand Create(IServiceClient client, Uri endpoint,
                                         ExecutionContext context,
                                         string bucketName, CreateBucketRequest createBucketRequest)
        {
            return new CreateBucketCommand(client, endpoint, context, bucketName, createBucketRequest);
        }
    }
}
