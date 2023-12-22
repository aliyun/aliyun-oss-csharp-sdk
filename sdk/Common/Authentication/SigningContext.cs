/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;

namespace Aliyun.OSS.Common.Authentication
{
    internal class SigningContext
    {
        public ICredentials Credentials { get; set; }

        public DateTime Expiration { get; set; }

        public DateTime SignTime { get; set; }
    }
}
