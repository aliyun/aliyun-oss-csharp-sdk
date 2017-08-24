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
        
        public ExecutionContextBuilder()
        {
            ResponseHandlers = new List<IResponseHandler>();
        }
        
        public ExecutionContext Build()
        {
            var context = new ExecutionContext
            {
                Signer = CreateSigner(Bucket, Key), 
                Credentials = Credentials
            };
            foreach(var h in ResponseHandlers)
            {
                context.ResponseHandlers.Add(h);
            }
            return context;
        }
        
        private static IRequestSigner CreateSigner(string bucket, string key)
        {    
            var resourcePath = "/" + (bucket ?? string.Empty) +
                ((key != null ? "/" + key : ""));
            
            // Hacked. the sign path is /bucket/key for two-level-domain mode
            // but /bucket/key/ for the three-level-domain mode.
            if (bucket != null && key == null)
            {
                resourcePath = resourcePath + "/";
            }
            
            return new OssRequestSigner(resourcePath);
        }
    }
}
