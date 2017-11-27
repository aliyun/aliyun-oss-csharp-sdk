/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class SetBucketStorageCapacityCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly SetBucketStorageCapacityRequest _setBucketStorageCapacityRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private SetBucketStorageCapacityCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                string bucketName, SetBucketStorageCapacityRequest setBucketStorageCapacityRequest)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            if (setBucketStorageCapacityRequest.StorageCapacity < -1)
                throw new ArgumentException("storage capacity must greater than -1");

            _bucketName = bucketName;
            _setBucketStorageCapacityRequest = setBucketStorageCapacityRequest;
        }

        public static SetBucketStorageCapacityCommand Create(IServiceClient client, Uri endpoint, 
                                                    ExecutionContext context,
                                                    string bucketName, SetBucketStorageCapacityRequest setBucketStorageCapacityRequest)
        {
            return new SetBucketStorageCapacityCommand(client, endpoint, context, bucketName, setBucketStorageCapacityRequest);
        }
        
        
        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_QOS, null }
                };
            }
        }
        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateSetBucketStorageCapacityRequestSerializer()
                    .Serialize(_setBucketStorageCapacityRequest);
            }
        }
    }
}
