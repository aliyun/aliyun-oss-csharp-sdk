/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Handlers;

namespace Aliyun.OSS.Util
{
    internal class ExecutionContextBuilder
    {
        public ICredentials Credentials { get; set; }

        public IList<IResponseHandler> ResponseHandlers { get; private set; }
        
        public HttpMethod Method { get; set; }
        
        public string Bucket { get; set; }
        
        public string Key { get; set; }

        public string Region { get; set; }

        public string Product { get; set; }

        public SignatureVersion SignatureVersion { get; set; }

        public ExecutionContextBuilder()
        {
            ResponseHandlers = new List<IResponseHandler>();
        }
        
        public ExecutionContext Build()
        {
            var context = new ExecutionContext
            {
                Signer = CreateSigner(Bucket, Key, Region, Product, SignatureVersion), 
                Credentials = Credentials
            };
            foreach(var h in ResponseHandlers)
            {
                context.ResponseHandlers.Add(h);
            }
            return context;
        }
        
        private static IRequestSigner CreateSigner(string bucket, string key, string region, string product, SignatureVersion version)
        {    
            var singer = OssRequestSigner.Create(version);

            singer.Bucket = bucket;
            singer.Key = key;
            singer.Region = region;
            singer.Product = product;

            return singer;
        }
    }
}
