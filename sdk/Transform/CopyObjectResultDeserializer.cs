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
            var model = ContentDeserializer.Deserialize(xmlStream.Content);
            var result = new CopyObjectResult
            {
                ETag = OssUtils.TrimQuotes(model.ETag),
                LastModified = model.LastModified
            };
            if (xmlStream.Headers.ContainsKey(HttpHeaders.VersionId))
            {
                result.VersionId = xmlStream.Headers[HttpHeaders.VersionId];
            }
            if (xmlStream.Headers.ContainsKey("x-oss-copy-source-version-id"))
            {
                result.CopySourceVersionId = xmlStream.Headers["x-oss-copy-source-version-id"];
            }
            DeserializeGeneric(xmlStream, result);

            return result;
        }
    }
}
