/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Util;
using System;

namespace Aliyun.OSS.Common.Handlers
{
    internal class CompleteMultipartUploadCrc64Handler : ResponseHandler
    {
        CompleteMultipartUploadRequest _request;
        public CompleteMultipartUploadCrc64Handler(CompleteMultipartUploadRequest request)
        {
            _request = request;
        }

        public override void Handle(Communication.ServiceResponse response)
        {
            
            if (response.Headers.ContainsKey(HttpHeaders.HashCrc64Ecma))
            {
                ulong crc64 = 0;
                bool checkCrc = true;
                foreach (var part in _request.PartETags)
                {
                    if (!String.IsNullOrEmpty(part.Crc64) && part.Length != 0)
                    {
                        crc64 = Crc64.Combine(crc64, ulong.Parse(part.Crc64), part.Length);
                    }
                    else
                    {
                        checkCrc = false;
                    }
                }

                if (!checkCrc) // the request does not have CRC64 for at least one part, skip the check
                {
                    return;
                }

                var ossCalculatedHashStr = response.Headers[HttpHeaders.HashCrc64Ecma];
                if (!crc64.ToString().Equals(ossCalculatedHashStr))
                {
                    response.Dispose();
                    throw new ClientException("Crc64 validation failed. Expected hash not equal to calculated hash");
                }
            }
        }
    }
}
