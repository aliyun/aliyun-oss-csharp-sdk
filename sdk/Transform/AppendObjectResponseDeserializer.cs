/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class AppendObjectResponseDeserializer : ResponseDeserializer<AppendObjectResult, AppendObjectResult>
    {
        public AppendObjectResponseDeserializer()
            : base(null)
        { }
        
        public override AppendObjectResult Deserialize(ServiceResponse xmlStream)
        {
            var result = new AppendObjectResult();
            if (xmlStream.Headers.ContainsKey(HttpHeaders.ETag))
                result.ETag = OssUtils.TrimQuotes(xmlStream.Headers[HttpHeaders.ETag]);

            if (xmlStream.Headers.ContainsKey(HttpHeaders.NextAppendPosition))
            {
                result.NextAppendPosition = long.Parse(xmlStream.Headers[HttpHeaders.NextAppendPosition]);
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.HashCrc64Ecma))
            {
                result.HashCrc64Ecma = ulong.Parse(xmlStream.Headers[HttpHeaders.HashCrc64Ecma]);
            }

            DeserializeGeneric(xmlStream, result);

            return result;
        }
    }
}
