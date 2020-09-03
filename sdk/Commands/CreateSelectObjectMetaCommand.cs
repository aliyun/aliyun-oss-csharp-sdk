/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class CreateSelectObjectMetaCommand : OssCommand<CreateSelectObjectMetaResult>
    {
        private readonly CreateSelectObjectMetaRequest _request;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        protected override string Key
        {
            get { return _request.Key; }
        }

        protected override Stream Content
        {
            get
            {
                if (_request.InputFormat.GetType() == typeof(CreateSelectObjectMetaCSVInputFormat))
                {
                    return SerializerFactory.GetFactory().CreateSelectObjectCsvMetaRequestSerializer()
                       .Serialize(_request);
                }
                else
                {
                    return SerializerFactory.GetFactory().CreateSelectObjectJsonMetaRequestSerializer()
                        .Serialize(_request);
                }
            }
        }

        private CreateSelectObjectMetaCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                        IDeserializer<ServiceResponse, CreateSelectObjectMetaResult> deserializer,
                                        CreateSelectObjectMetaRequest request)
            : base(client, endpoint, context, deserializer)
        {
            _request = request;
        }

        public static CreateSelectObjectMetaCommand Create(IServiceClient client, Uri endpoint,
                                                       ExecutionContext context,
                                                       CreateSelectObjectMetaRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);
            if (request.InputFormat == null)
                throw new ArgumentException("InputFormat should not be null");
            return new CreateSelectObjectMetaCommand(client, endpoint, context,
                     DeserializerFactory.GetFactory().CreateSelectObjectMetaRequestDeserializer(),
                     request);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                string str = _request.InputFormat.GetType() == typeof(CreateSelectObjectMetaCSVInputFormat)? "csv/meta":"json/meta";
                var parameters = new Dictionary<string, string>()
                {
                    { RequestParameters.OSS_PROCESS, str }
                };
                return parameters;
            }
        }
    }
}

