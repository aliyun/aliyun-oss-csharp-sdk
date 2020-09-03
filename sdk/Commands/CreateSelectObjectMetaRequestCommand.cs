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
    internal class CreateSelectObjectMetaRequestCommand : OssCommand<CreateSelectObjectMetaResult>
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
                if (_request.InputFormatType == InputFormatTypes.CSV)
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

        private CreateSelectObjectMetaRequestCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                        IDeserializer<ServiceResponse, CreateSelectObjectMetaResult> deserializer,
                                        CreateSelectObjectMetaRequest selectObjectRequest)
            : base(client, endpoint, context, deserializer)
        {
            _request = selectObjectRequest;
        }

        public static CreateSelectObjectMetaRequestCommand Create(IServiceClient client, Uri endpoint,
                                                       ExecutionContext context,
                                                       CreateSelectObjectMetaRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);
            return new CreateSelectObjectMetaRequestCommand(client, endpoint, context,
                     DeserializerFactory.GetFactory().CreateSelectObjectMetaRequestDeserializer(request),
                     request);
        }


        protected override IDictionary<string, string> Parameters
        {
            get
            {
                string str;
                if (_request.InputFormatType == InputFormatTypes.CSV)
                    str = "csv/meta";
                else
                    str = "json/meta";
                var parameters = new Dictionary<string, string>()
                {
                    { RequestParameters.OSS_PROCESS, str }
                };
                return parameters;
            }
        }
    }
}

