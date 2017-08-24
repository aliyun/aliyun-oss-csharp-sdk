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

namespace Aliyun.OSS.Commands
{
    internal class GetObjectAclCommand : OssCommand<AccessControlList>
    {
        private readonly string _bucketName;
        private readonly string _key;

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        protected override string Key
        {
            get { return _key; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_ACL, null }
                };
            }
        }

        private GetObjectAclCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, string key, 
                                    IDeserializer<ServiceResponse, AccessControlList> deserializer)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(key);

            _bucketName = bucketName;
            _key = key;
        }

        public static GetObjectAclCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 string bucketName, string key)
        {
            return new GetObjectAclCommand(client, endpoint, context, bucketName, key,
                                           DeserializerFactory.GetFactory().CreateGetAclResultDeserializer());
        }
    }
}
