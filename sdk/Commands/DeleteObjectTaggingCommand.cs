/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using System.Collections.Generic;

namespace Aliyun.OSS.Commands
{
    /// <summary>
    /// Delete object tagging command.
    /// </summary>
    internal class DeleteObjectTaggingCommand : OssCommand
    {
        private string bucketName;
        private string key;

        protected override string Bucket
        {
            get
            {
                return bucketName;
            }
        }

        protected override string Key
        {
            get
            {
                return key;
            }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }

        private DeleteObjectTaggingCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                       string BucketName, string Key)
            : base(client, endpoint, context)
        {
            bucketName = BucketName;
            key = Key;
        }

        public static DeleteObjectTaggingCommand Create(IServiceClient client, Uri endpoint,
                                                    ExecutionContext context,
                                                    string bucketName, string key)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(key);
            return new DeleteObjectTaggingCommand(client, endpoint, context, bucketName, key);
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_TAGGING, null }
                };
            }
        }
    }
}

