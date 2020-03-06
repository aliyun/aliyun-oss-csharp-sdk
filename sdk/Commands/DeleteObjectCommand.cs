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
        private readonly DeleteObjectRequest _deleteObjectRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }

        protected override string Bucket
        {
            get { return _deleteObjectRequest.BucketName; }
        }
        
        protected override string Key
        {
            get { return _deleteObjectRequest.Key; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                if (_deleteObjectRequest.RequestPayer == RequestPayer.Requester)
                {
                    headers.Add(OssHeaders.OssRequestPayer, RequestPayer.Requester.ToString().ToLowerInvariant());
                }
                return headers;
            }
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
                                    DeleteObjectRequest deleteObjectRequest)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(deleteObjectRequest.BucketName);
            OssUtils.CheckObjectKey(deleteObjectRequest.Key);

            _deleteObjectRequest = deleteObjectRequest;
        }

        public static DeleteObjectCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                 string bucketName, DeleteObjectRequest deleteObjectRequest)
        {
            return new DeleteObjectCommand(client, endpoint, context, deleteObjectRequest);
        }
    }
}
