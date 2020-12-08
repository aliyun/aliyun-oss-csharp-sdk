/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;

namespace Aliyun.OSS.Transform
{
    internal class ProcessObjectResultDeserializer : ResponseDeserializer<ProcessObjectResult, Stream>
    {
        public ProcessObjectResultDeserializer(IDeserializer<Stream, Stream> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override ProcessObjectResult Deserialize(ServiceResponse xmlStream)
        {
            ProcessObjectResult result = new ProcessObjectResult();

            StreamReader reader = new StreamReader(xmlStream.Content);
            result.Content = reader.ReadToEnd();

            DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}

