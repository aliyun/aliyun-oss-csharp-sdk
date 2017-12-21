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
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Common.Handlers;

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

            var conf = OssUtils.GetClientConfiguration(client);
            var originalStream = request.Content;
            var streamLength = request.Content.Length;

            // setup progress
            var callback = request.StreamTransferProgress;
            if (callback != null)
            {
                originalStream = OssUtils.SetupProgressListeners(originalStream, conf.ProgressUpdateInterval, client, callback);
                request.Content = originalStream;
            }

            // wrap input stream in MD5Stream
            if (conf.EnalbeMD5Check)
            {
                var hashStream = new MD5Stream(originalStream, null, streamLength);
                request.Content = hashStream;
                context.ResponseHandlers.Add(new MD5DigestCheckHandler(hashStream));
            }

            return new AppendObjectCommand(client, endpoint, context,
                                           DeserializerFactory.GetFactory().CreateAppendObjectReusltDeserializer(),
                                           request);
        }
    }
}
