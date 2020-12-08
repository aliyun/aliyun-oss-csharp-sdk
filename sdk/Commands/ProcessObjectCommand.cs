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
using System.IO;
using System.Text;

namespace Aliyun.OSS.Commands
{
    internal class ProcessObjectCommand : OssCommand<ProcessObjectResult>
    {
        private readonly ProcessObjectRequest _request;

        protected override string Bucket
        {
            get
            {
                return _request.BucketName;
            }
        }

        protected override string Key
        {
            get
            {
                return _request.Key;
            }
        }

        private ProcessObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                      IDeserializer<ServiceResponse, ProcessObjectResult> deserializer,
                                      ProcessObjectRequest request)
           : base(client, endpoint, context, deserializer)
        {
            _request = request;
        }

        public static ProcessObjectCommand Create(IServiceClient client, Uri endpoint,
                                                   ExecutionContext context,
                                                   ProcessObjectRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);
            return new ProcessObjectCommand(client, endpoint, context,
                                           DeserializerFactory.GetFactory().CreateProcessObjectResultDeserializer(),
                                           request);
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters =  new Dictionary<string, string>()
                {
                    { RequestParameters.OSS_PROCESS, null }
                };
                return parameters;
            }
        }

        protected override Stream Content
        {
            get
            {
                var value = "x-oss-process=" + _request.Process;
                return new MemoryStream(Encoding.UTF8.GetBytes(value));
            }
        }
    }
}

