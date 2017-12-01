/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Properties;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Commands
{
    internal class RestoreObjectCommand : OssCommand<RestoreObjectResult>
    {
        private string bucketName;
        private string key;

        protected override string Bucket
        {
            get { 
                return bucketName; 
            }
        }

        protected override string Key
        {
            get {
                return key; 
            }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, string>();
                parameters[RequestParameters.SUBRESOURCE_RESTORE] = null;
                return parameters;
            }
        }

        private RestoreObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                     string bucketName, string key, IDeserializer<ServiceResponse, RestoreObjectResult> deserializer)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(key);

            this.bucketName = bucketName;
            this.key = key;
            this.ParametersInUri = true; // in restore request, the parameter restore needs to be in uri
        }

        public static RestoreObjectCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 string bucketName, string key)
        {
            return new RestoreObjectCommand(client, endpoint, context, bucketName, key, DeserializerFactory.GetFactory().CreateRestoreObjectResultDeserializer());
        }
    }
}
