/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class PutObjectResponseDeserializer : ResponseDeserializer<PutObjectResult, PutObjectResult>
    {
        public PutObjectResponseDeserializer()
            : base(null)
        { }
        
        public override PutObjectResult Deserialize(ServiceResponse xmlStream)
        {
            var result = new PutObjectResult();
            if (xmlStream.Headers.ContainsKey(HttpHeaders.ETag))
                result.ETag = OssUtils.TrimQuotes(xmlStream.Headers[HttpHeaders.ETag]);
            return result;
        }
    }
}
