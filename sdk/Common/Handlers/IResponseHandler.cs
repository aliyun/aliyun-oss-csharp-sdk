/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using Aliyun.OSS.Common.Communication;

namespace Aliyun.OSS.Common.Handlers
{
    internal interface IResponseHandler
    {
        void Handle(ServiceResponse response);
    }
}
