/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using System.Xml;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class GetBucketVersioningResultDeserializer : ResponseDeserializer<GetBucketVersioningResult, VersioningConfiguration>
    {
        public GetBucketVersioningResultDeserializer(IDeserializer<Stream, VersioningConfiguration> contentDeserializer)
            : base(contentDeserializer)
        { }

        public override GetBucketVersioningResult Deserialize(ServiceResponse xmlStream)
        {
            var result = new GetBucketVersioningResult();

            //var mode = ContentDeserializer.Deserialize(xmlStream.Content);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlStream.Content);
            var node = xmlDoc.SelectSingleNode("/VersioningConfiguration/Status");

            if (node != null)
            {
                var status = xmlDoc.SelectSingleNode("/VersioningConfiguration/Status").InnerText;
                if (status.ToLowerInvariant() == "enabled")
                {
                    result.Status = VersioningStatus.Enabled;
                }
                else if (status.ToLowerInvariant() == "suspended")
                {
                    result.Status = VersioningStatus.Suspended;
                }
            }

            DeserializeGeneric(xmlStream, result);
            return result;
        }
    }
}

