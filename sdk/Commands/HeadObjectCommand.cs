/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Commands
{
    internal class HeadObjectCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly string _key;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Head; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        protected override string Key
        {
            get { return _key; }
        }

        private HeadObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, string key)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(key);

            _bucketName = bucketName;
            _key = key;
        }

        public static HeadObjectCommand Create(IServiceClient client, Uri endpoint,
                                              ExecutionContext context,
                                              string bucketName, string key)
        {
            return new HeadObjectCommand(client, endpoint, context, bucketName, key);
        }
    }
}
