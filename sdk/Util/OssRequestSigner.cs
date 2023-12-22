/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using System.Collections.Generic;

namespace Aliyun.OSS.Util
{
    internal abstract class OssRequestSigner : IRequestSigner
    {
        public string Bucket { get; internal set; }

        public string Key { get; internal set; }

        public string Region { get; internal set; }

        public string Product { get; internal set; }

        public List<string> AdditionalHeaders { get; internal set; }

        public abstract void Sign(ServiceRequest request, ICredentials credentials);

        public abstract void PreSign(ServiceRequest request, SigningContext signingContext);

        public static OssRequestSigner Create(SignatureVersion version)
        {
            if (SignatureVersion.V4.Equals(version))
            {
                return new OssRequestSignerV4();
            }
            return new OssRequestSignerV1();
        }

        protected string getResourcePath()
        {
            var resourcePath = "/" + (Bucket ?? string.Empty) + ((Key != null ? "/" + Key : ""));
            if (Bucket != null && Key == null)
            {
                resourcePath = resourcePath + "/";
            }

            return resourcePath;
        }

        protected static bool IsAnonymousCredentials(ICredentials credentials)
        {
            if (credentials == null ||
                string.IsNullOrEmpty(credentials.AccessKeyId) ||
                string.IsNullOrEmpty(credentials.AccessKeySecret))
            {
                return true;
            }

            return false;
        }
    }
}
