/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Common.Communication;

namespace Aliyun.OSS.Common.Handlers
{
    internal interface IResponseHandler
    {
        void Handle(ServiceResponse response);
    }
}
