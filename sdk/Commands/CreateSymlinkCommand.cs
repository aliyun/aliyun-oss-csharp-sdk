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
    internal class CreateSymlinkCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly string _target;
        private readonly string _symlink;
        private readonly ObjectMetadata _objectMetadata;

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        protected override string Key
        {
            get { return _symlink; }
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
                headers.Add(OssHeaders.SymlinkTarget, _target);
                if (_objectMetadata != null)
                {
                    _objectMetadata.Populate(headers);
                }

                return headers;
            }
        }

        private CreateSymlinkCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                     string bucketName, string symlink, string target)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(symlink);
            OssUtils.CheckObjectKey(target);

            if (symlink == target)
            {
                throw new ArgumentException("Symlink file name must be different with its target.");
            }

            _bucketName = bucketName;
            _symlink = symlink;
            _target = target;
        }

        private CreateSymlinkCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                     string bucketName, string symlink, string target, ObjectMetadata metadata)
            :this(client, endpoint, context, bucketName, symlink, target)
        {
            _objectMetadata = metadata;
        }

        public static CreateSymlinkCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 string bucketName, string symlink, string target)
        {
            return new CreateSymlinkCommand(client, endpoint, context, bucketName, symlink, target);
        }

        public static CreateSymlinkCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context, CreateSymlinkRequest request)
        {
            return new CreateSymlinkCommand(client, endpoint, context, request.BucketName, 
                                            request.Symlink, request.Target, request.ObjectMetadata);
        }
    }
}
