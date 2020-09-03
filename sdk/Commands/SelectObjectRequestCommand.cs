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
    internal class SelectObjectRequestCommand : OssCommand<OssObject>
    {
        private readonly SelectObjectRequest _request;

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
                return SerializerFactory.GetFactory().CreateSelectObjectRequestSerializer()
                    .Serialize(_request);
            }
        }

        private SelectObjectRequestCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                        IDeserializer<ServiceResponse, OssObject> deserializer,
                                        SelectObjectRequest selectObjectRequest)
            : base(client, endpoint, context, deserializer)
        {
            _request = selectObjectRequest;
        }

        public static SelectObjectRequestCommand Create(IServiceClient client, Uri endpoint,
                                                       ExecutionContext context,
                                                       SelectObjectRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);
            return new SelectObjectRequestCommand(client, endpoint, context,
                     DeserializerFactory.GetFactory().CreateSelectObjectRequestDeserializer(request),
                     request);
        }


        protected override IDictionary<string, string> Parameters
        {
            get
            {
                string str;
                if (_request.InputFormatType == InputFormatTypes.CSV)
                    str = "csv/select";
                else
                    str = "json/select";
                var parameters = new Dictionary<string, string>()
                {
                    { RequestParameters.OSS_PROCESS, str }
                };
                return parameters;
            }
        }

        protected override bool LeaveResponseOpen
        {
            get { return true; }
        }
    }
}

