/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Util
{
    internal class OssRequestSigner : IRequestSigner
    {
        private readonly string _resourcePath;

        public OssRequestSigner(String resourcePath)
        {
            _resourcePath = resourcePath;
        }
        
        public void Sign(ServiceRequest request, ICredentials credentials)
        {
            var accessKeyId = credentials.AccessKeyId;
            var accessKeySecret = credentials.AccessKeySecret;
            var httpMethod = request.Method.ToString().ToUpperInvariant();
            // Because the resource path to is different from the one in the request uri,
            // can't use ServiceRequest.ResourcePath here.
            var resourcePath = _resourcePath;
            if (!string.IsNullOrEmpty(accessKeySecret))
            {
                var canonicalString = SignUtils.BuildCanonicalString(httpMethod, resourcePath, request);
                var signature = ServiceSignature.Create().ComputeSignature(accessKeySecret, canonicalString);
                
                request.Headers.Add(HttpHeaders.Authorization, "OSS " + accessKeyId + ":" + signature);
            }
        }
    }
}
