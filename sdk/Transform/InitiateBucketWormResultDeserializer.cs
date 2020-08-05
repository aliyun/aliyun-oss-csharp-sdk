/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Common.Communication;

namespace Aliyun.OSS.Transform
{
    internal class InitiateBucketWormResultDeserializer : ResponseDeserializer<InitiateBucketWormResult, Stream>
    {
        public InitiateBucketWormResultDeserializer(IDeserializer<Stream, Stream> contentDeserializer)
                 : base(contentDeserializer)
        {
        }

        public override InitiateBucketWormResult Deserialize(ServiceResponse xmlStream)
        {
            var result = new InitiateBucketWormResult();

            if (xmlStream.Headers.ContainsKey("x-oss-worm-id"))
            {
                result.WormId = xmlStream.Headers["x-oss-worm-id"];
            }

            DeserializeGeneric(xmlStream, result);

            return result;
        }
    }
}
