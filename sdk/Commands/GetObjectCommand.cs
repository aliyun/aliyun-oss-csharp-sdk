/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Handlers;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class GetObjectCommand : OssCommand<OssObject>
    {
        private readonly GetObjectRequest _getObjectRequest;

        protected override string Bucket
        {
            get { return _getObjectRequest.BucketName; }
        }
        
        protected override string Key
        { 
            get { return _getObjectRequest.Key; }
        }

        protected override IDictionary<string, string> Headers 
        {
            get
            {
                var headers = new Dictionary<string, string>();
                _getObjectRequest.Populate(headers);
                return headers;
            }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                _getObjectRequest.ResponseHeaders.Populate(parameters);
                if (_getObjectRequest.Process != null)
                {
                    parameters[RequestParameters.OSS_PROCESS] = _getObjectRequest.Process;
                }
                if (!string.IsNullOrEmpty(_getObjectRequest.VersionId))
                {
                    parameters.Add(RequestParameters.SUBRESOURCE_VERSIONID, _getObjectRequest.VersionId);
                }
                return parameters;
            }
        }

        protected override bool LeaveResponseOpen
        {
            get { return true; }
        }

        private GetObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                 IDeserializer<ServiceResponse, OssObject> deserializer,
                                 GetObjectRequest getObjectRequest)
            : base(client, endpoint, context, deserializer)
        {
            _getObjectRequest = getObjectRequest;
        }

        public static GetObjectCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                              GetObjectRequest getObjectRequest)
        {
            OssUtils.CheckBucketName(getObjectRequest.BucketName);
            OssUtils.CheckObjectKey(getObjectRequest.Key);

            return new GetObjectCommand(client, endpoint, context,
                                 DeserializerFactory.GetFactory().CreateGetObjectResultDeserializer(getObjectRequest, client),
                                 getObjectRequest);
        }
    }
}
