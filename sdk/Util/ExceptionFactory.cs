/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using Aliyun.OSS.Properties;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Util
{
    internal static class ExceptionFactory
    {
        public static OssException CreateException(string errorCode,
                                                   string message,
                                                   string requestId,
                                                   string hostId)
        {
            return CreateException(errorCode, message, requestId, hostId, null);
        }
        
        public static OssException CreateException(string errorCode,
                                                   string message,
                                                   string requestId,
                                                   string hostId,
                                                   Exception innerException)
        {
            var exception = innerException != null ?
                new OssException(message, innerException) :
                new OssException(message);

            exception.RequestId = requestId;
            exception.HostId = hostId;
            exception.ErrorCode = errorCode;

            return exception;
        }
        
        public static Exception CreateInvalidResponseException(Exception innerException)
        {
            throw new InvalidOperationException(Resources.ExceptionInvalidResponse, innerException);
        }
    }
}
