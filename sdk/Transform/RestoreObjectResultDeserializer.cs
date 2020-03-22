/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class RestoreObjectResultDeserializer : ResponseDeserializer<RestoreObjectResult, ErrorResult>
    {
        public RestoreObjectResultDeserializer(IDeserializer<Stream, ErrorResult> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override RestoreObjectResult Deserialize(ServiceResponse xmlStream)
        {
            RestoreObjectResult result = new RestoreObjectResult();
            if (xmlStream.Headers.ContainsKey(HttpHeaders.VersionId))
            {
                result.VersionId = xmlStream.Headers[HttpHeaders.VersionId];
            }
            DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}
