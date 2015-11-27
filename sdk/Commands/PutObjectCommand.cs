/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using System.IO;

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class PutObjectCommand : OssCommand<PutObjectResult>
    {
        private readonly OssObject _ossObject;

        protected override string Bucket
        {
            get { return _ossObject.BucketName; }
        } 
        
        protected override string Key
        {
            get { return _ossObject.Key; }
        } 
        
        protected override bool LeaveRequestOpen
        {
            get { return true; }
        }

        private PutObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                 IDeserializer<ServiceResponse, PutObjectResult> deserializer,
                                 OssObject ossObject)
            : base(client, endpoint, context, deserializer)
        {
            _ossObject = ossObject;
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override Stream Content
        {
            get { return _ossObject.Content; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                _ossObject.Metadata.Populate(headers);
                return headers;
            }
        }

        public static PutObjectCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                              string bucketName, string key,
                                              Stream content, ObjectMetadata metadata)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(key);

            if (content == null)
                throw new ArgumentNullException("content");

            var ossObject = new OssObject(key)
            {
                BucketName = bucketName,
                Content = content,
                Metadata = metadata ?? new ObjectMetadata()
            };

            return new PutObjectCommand(client, endpoint, context,
                                        DeserializerFactory.GetFactory().CreatePutObjectReusltDeserializer(),
                                        ossObject);
        }
    }
}
