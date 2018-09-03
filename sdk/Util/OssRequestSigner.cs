/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Util
{
    internal class OssRequestSigner : IRequestSigner
    {
        private readonly string _resourcePath;
        private readonly string _version;
        private readonly IList<string> _headersToSign;

        public OssRequestSigner(string resourcePath, string version, IList<string> headerToSign)
        {
            _resourcePath = resourcePath;
            _version = version;
            _headersToSign = headerToSign;
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
                var signedHeadersList = (_headersToSign == null) ? null : new List<string>();
                var canonicalString = SignUtils.BuildCanonicalString(httpMethod, resourcePath, 
                                                                     request, _version,
                                                                    _headersToSign, signedHeadersList);
                var signature = ServiceSignature.Create(_version).ComputeSignature(accessKeySecret, canonicalString);
                var authorization = SignUtils.BuildAuthorizationValue(accessKeyId, signature, signedHeadersList, _version);
                // request could be retried and the Authorization header may exist already.
                // Fix for #OSS-1579/11349300 
                request.Headers[HttpHeaders.Authorization] = authorization;
            }
        }
    }
}
