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
    internal class GetSymlinkCommand : OssCommand<OssSymlink>
    {
        private readonly string _bucketName;
        private readonly string _symlink;

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
            get { return HttpMethod.Get; }
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

        private GetSymlinkCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                  IDeserializer<ServiceResponse, OssSymlink> deserializer,
                                  string bucketName, string symlink)
            : base(client, endpoint, context, deserializer)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(symlink);
           
            _bucketName = bucketName;
            _symlink = symlink;
        }

        public static GetSymlinkCommand Create(IServiceClient client, Uri endpoint,
                                                 ExecutionContext context,
                                                 IDeserializer<ServiceResponse, OssSymlink> deserializer,
                                                 string bucketName, string symlink)
        {
            return new GetSymlinkCommand(client, endpoint, context, deserializer, bucketName, symlink);
        }
    }
}
