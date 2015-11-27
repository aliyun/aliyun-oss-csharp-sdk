/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Test.Util
{
    internal static class OssClientFactory
    {
        public static IOss CreateOssClient()
        {
            return CreateOssClient(AccountSettings.Load());
        }

        public static IOss CreateOssClient(AccountSettings settings)
        {
            var clientConfiguration = new ClientConfiguration();
            if (!String.IsNullOrEmpty(settings.ProxyHost))
            {
                clientConfiguration.ProxyHost =  settings.ProxyHost;
                clientConfiguration.ProxyPort = settings.ProxyPort;
            }
            return new OssClient(settings.OssEndpoint,
                                 settings.OssAccessKeyId,
                                 settings.OssAccessKeySecret);
        }
    }
}
