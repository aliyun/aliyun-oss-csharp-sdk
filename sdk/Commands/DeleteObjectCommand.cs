/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;


namespace Aliyun.OSS.Commands
{
    internal class DeleteObjectCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly string _key;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }
        
        protected override string Key
        {
            get { return _key; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {RequestParameters.ENCODING_TYPE, HttpUtils.UrlEncodingType }
                };
            }
        }

        private DeleteObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, string key)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(key);

            _bucketName = bucketName;
            _key = key;
        }

        public static DeleteObjectCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                 string bucketName, string key)
        {
            return new DeleteObjectCommand(client, endpoint, context, bucketName, key);
        }
    }
}
