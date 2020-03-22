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
    internal class CreateSymlinkResultDeserializer : ResponseDeserializer<CreateSymlinkResult, Stream>
    {
        public CreateSymlinkResultDeserializer(IDeserializer<Stream, Stream> contentDeserializer)
                 : base(contentDeserializer)
        {
        }
        
        public override CreateSymlinkResult Deserialize(ServiceResponse xmlStream)
        {
            var result = new CreateSymlinkResult();

            if (xmlStream.Headers.ContainsKey(HttpHeaders.ETag))
            {
                result.ETag = OssUtils.TrimQuotes(xmlStream.Headers[HttpHeaders.ETag]);
            }
            if (xmlStream.Headers.ContainsKey(HttpHeaders.VersionId))
            {
                result.VersionId = xmlStream.Headers[HttpHeaders.VersionId];
            }

            DeserializeGeneric(xmlStream, result);

            return result;
        }
    }
}
