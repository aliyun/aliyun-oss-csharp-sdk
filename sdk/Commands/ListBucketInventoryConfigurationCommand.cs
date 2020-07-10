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
using Aliyun.OSS.Model;
namespace Aliyun.OSS.Commands
{
    internal class ListBucketInventoryConfigurationCommand : OssCommand<ListBucketInventoryConfigurationResult>
    {
        private readonly string _bucketName;
        private readonly string _continuationToken;

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        private ListBucketInventoryConfigurationCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, string continuationToken, IDeserializer<ServiceResponse, ListBucketInventoryConfigurationResult> deserializer)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(bucketName);
            _bucketName = bucketName;
            _continuationToken = continuationToken;
        }

        public static ListBucketInventoryConfigurationCommand Create(IServiceClient client, Uri endpoint,
                                                     ExecutionContext context,
                                                    string bucketName, string id)
        {
            return new ListBucketInventoryConfigurationCommand(client, endpoint, context, bucketName, id,
                                              DeserializerFactory.GetFactory().CreateListBucketInventoryConfigurationResultDeserializer());
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                if (_continuationToken != null)
                {
                    return new Dictionary<string, string>()
                    {
                        { RequestParameters.SUBRESOURCE_CONTINUATIONTOKEN, _continuationToken },
                        { RequestParameters.SUBRESOURCE_INVENTORY, null}
                    };
                }
                else
                {
                    var parameters = base.Parameters;
                    parameters[RequestParameters.SUBRESOURCE_INVENTORY] = null;
                    return parameters;
                }
            }
        }
    }
}
