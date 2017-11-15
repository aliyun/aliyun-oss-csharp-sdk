/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;

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
            this.DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}
