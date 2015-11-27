/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class DeleteObjectsResultDeserializer : ResponseDeserializer<DeleteObjectsResult, DeleteObjectsResult>
    {
        public DeleteObjectsResultDeserializer(IDeserializer<Stream, DeleteObjectsResult> contentDeserializer)
                 : base(contentDeserializer)
        { }

        public override DeleteObjectsResult Deserialize(ServiceResponse xmlStream)
        {
            if (int.Parse(xmlStream.Headers[HttpHeaders.ContentLength]) == 0)
                return new DeleteObjectsResult();
            
            return ContentDeserializer.Deserialize(xmlStream.Content);
        }
    }
}
