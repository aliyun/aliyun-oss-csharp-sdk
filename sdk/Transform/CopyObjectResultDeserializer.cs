/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class CopyObjectResultDeserializer : ResponseDeserializer<CopyObjectResult, CopyObjectResultModel>
    {
        public CopyObjectResultDeserializer(IDeserializer<Stream, CopyObjectResultModel> contentDeserializer)
                 : base(contentDeserializer)
        {
        }
        
        public override CopyObjectResult Deserialize(ServiceResponse xmlStream)
        {
            var result = ContentDeserializer.Deserialize(xmlStream.Content);
            var copyObjectResult = new CopyObjectResult
            {
                ETag = OssUtils.TrimQuotes(result.ETag),
                LastModified = result.LastModified
            };

            DeserializeGeneric(xmlStream, copyObjectResult);

            return copyObjectResult;
        }
    }
}
