/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Util
{
    internal class OssRequestSignerV1 : OssRequestSigner
    {
        public OssRequestSignerV1()
        {
        }

        public override void Sign(ServiceRequest request, ICredentials credentials)
        {
            var accessKeyId = credentials.AccessKeyId;
            var accessKeySecret = credentials.AccessKeySecret;
            var httpMethod = request.Method.ToString().ToUpperInvariant();
            // Because the resource path to is different from the one in the request uri,
            // can't use ServiceRequest.ResourcePath here.
            if (!string.IsNullOrEmpty(accessKeySecret))
            {
                if (credentials.UseToken)
                    request.Headers[HttpHeaders.SecurityToken] = credentials.SecurityToken;

                var resourcePath = getResourcePath();
                var canonicalString = SignUtils.BuildCanonicalString(httpMethod, resourcePath, request);
                var signature = ServiceSignature.Create().ComputeSignature(accessKeySecret, canonicalString);

                // request could be retried and the Authorization header may exist already.
                // Fix for #OSS-1579/11349300 
                request.Headers[HttpHeaders.Authorization] =  "OSS " + accessKeyId + ":" + signature;
            }
        }

        public override void PreSign(ServiceRequest request, SigningContext signingContext)
        {
            if (signingContext == null ||
                signingContext.Expiration == null ||
                IsAnonymousCredentials(signingContext.Credentials))
                return;

            // Credentials information
            var credentials = signingContext.Credentials;
            if (credentials.UseToken)
            {
                request.Parameters.Add(RequestParameters.SECURITY_TOKEN, credentials.SecurityToken);
            }

            var canonicalResource = "/" + (Bucket ?? "") + ((Key != null ? "/" + Key : ""));
            var httpMethod = request.Method.ToString().ToUpperInvariant();
            var expires = DateUtils.FormatUnixTime(signingContext.Expiration);

            request.Headers.Add(HttpHeaders.Date, expires);

            var canonicalString =
                SignUtils.BuildCanonicalString(httpMethod, canonicalResource, request/*, expires*/);
            var signature = ServiceSignature.Create().ComputeSignature(credentials.AccessKeySecret, canonicalString);

            //Console.WriteLine("canonicalString:{0}\n", canonicalString);
            //Console.WriteLine("signature:{0}\n", signature);

            request.Parameters[RequestParameters.EXPIRES] = expires;
            request.Parameters[RequestParameters.OSS_ACCESS_KEY_ID] = credentials.AccessKeyId;
            request.Parameters[RequestParameters.SIGNATURE] = signature;
        }
    }
}
