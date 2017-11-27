/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Commands
{
    internal class CreateBucketCommand : OssCommand
    {
        private readonly string _bucketName;

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
                                        .Serialize(StorageClass.Value);
            }
        }

        protected StorageClass? StorageClass
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
        }

        public static CreateBucketCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 string bucketName, StorageClass? storageClass = null)
        {
            return new CreateBucketCommand(client, endpoint, context, bucketName, storageClass);
        }
    }
}
