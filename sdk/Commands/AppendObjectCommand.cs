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
    internal class AppendObjectCommand : OssCommand<AppendObjectResult>
    {
        private const string AppendName = "append";
        private const string AppendPosition = "position";
        private readonly AppendObjectRequest _request;

        protected override string Bucket
        {
            get { return _request.BucketName; }
        } 
        
        protected override string Key
        {
            get { return _request.Key; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = base.Parameters;
                parameters[AppendName] = string.Empty;
                parameters[AppendPosition] = _request.Position.ToString();
                return parameters;
            }
        }

        private AppendObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    IDeserializer<ServiceResponse, AppendObjectResult> deserializer,
                                    AppendObjectRequest request)
            : base(client, endpoint, context, deserializer)
        {
            _request = request;
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override Stream Content
        {
            get { return _request.Content; }
        }

        protected override bool LeaveRequestOpen
        {
            get { return true; }
        }
        

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                _request.ObjectMetadata.Populate(headers);
                return headers;
            }
        }

        public static AppendObjectCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                 AppendObjectRequest request)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Key);

            if (request.Content == null)
                throw new ArgumentNullException("request.Content");

            request.ObjectMetadata = request.ObjectMetadata ?? new ObjectMetadata();
            if (request.ObjectMetadata.ContentType == null)
                request.ObjectMetadata.ContentType = HttpUtils.GetContentType(request.Key, null);

            return new AppendObjectCommand(client, endpoint, context,
                                           DeserializerFactory.GetFactory().CreateAppendObjectReusltDeserializer(),
                                           request);
        }
    }
}
