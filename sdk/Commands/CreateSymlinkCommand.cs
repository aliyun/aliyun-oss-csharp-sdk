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
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class CreateSymlinkCommand : OssCommand <CreateSymlinkResult>
    {
        private readonly ObjectMetadata _objectMetadata;
        private readonly CreateSymlinkRequest _request;

        protected override string Bucket
        {
            get { return _request.BucketName; }
        }

        protected override string Key
        {
            get { return _request.Symlink; }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {RequestParameters.SUBRESOURCE_SYMLINK, null}
                };
            }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                headers.Add(OssHeaders.SymlinkTarget, _request.Target);
                if (_objectMetadata != null)
                {
                    _objectMetadata.Populate(headers);
                }
                if (_request.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                return headers;
            }
        }

        private CreateSymlinkCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                     IDeserializer<ServiceResponse, CreateSymlinkResult> deserializer,
                                     CreateSymlinkRequest request)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(request.BucketName);
            OssUtils.CheckObjectKey(request.Symlink);
            OssUtils.CheckObjectKey(request.Target);

            if (request.Symlink == request.Target)
            {
                throw new ArgumentException("Symlink file name must be different with its target.");
            }
            _request = request;
        }

        private CreateSymlinkCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                     IDeserializer<ServiceResponse, CreateSymlinkResult> deserializer,
                                     CreateSymlinkRequest request, ObjectMetadata metadata)
            :this(client, endpoint, context, deserializer, request)
        {
            _objectMetadata = metadata;
        }

        public static CreateSymlinkCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context, CreateSymlinkRequest request)
        {
            return new CreateSymlinkCommand(client, endpoint, context,
                DeserializerFactory.GetFactory().CreateCreateSymlinkResultDeserializer(), 
                request, request.ObjectMetadata);
        }
    }
}
