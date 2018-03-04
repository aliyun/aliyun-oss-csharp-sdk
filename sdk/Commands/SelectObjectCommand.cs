/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Handlers;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class SelectObjectCommand : OssCommand<OssObject>
    {
        private readonly SelectObjectRequest _selectObjectRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override string Bucket
        {
            get { return _selectObjectRequest.BucketName; }
        }

        protected override string Key
        {
            get { return _selectObjectRequest.Key; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                _selectObjectRequest.Populate(headers);
                return headers;
            }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_SELECT, null }
                };
            }
        }

        protected override Stream Content
        {
            get
            {
                return SerializerFactory.GetFactory().CreateSelectObjectRequestSerializer()
                                        .Serialize(_selectObjectRequest);
            }
        }

        protected override bool LeaveResponseOpen
        {
            get { return true; }
        }

        private SelectObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                 IDeserializer<ServiceResponse, OssObject> deserializer,
                                 SelectObjectRequest selectObjectRequest)
            : base(client, endpoint, context, deserializer)
        {
            _selectObjectRequest = selectObjectRequest;
        }

        public static SelectObjectCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                              SelectObjectRequest selectObjectRequest)
        {
            OssUtils.CheckBucketName(selectObjectRequest.BucketName);
            OssUtils.CheckObjectKey(selectObjectRequest.Key);

            return new SelectObjectCommand(client, endpoint, context,
                                 DeserializerFactory.GetFactory().CreateGetObjectResultDeserializer(selectObjectRequest, client),
                                 selectObjectRequest);
        }
    }
}
