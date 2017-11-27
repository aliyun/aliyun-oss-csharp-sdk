/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Globalization;

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class GetBucketMetadataResponseDeserializer : ResponseDeserializer<BucketMetadata, BucketMetadata>
    {
        public GetBucketMetadataResponseDeserializer()
            : base(null)
        { }

        public override BucketMetadata Deserialize(ServiceResponse xmlStream)
        {
            var metadata = new BucketMetadata();
            foreach(var header in xmlStream.Headers)
            {
                if (string.Equals(header.Key, HttpHeaders.BucketRegion, StringComparison.InvariantCultureIgnoreCase))
                {
                    metadata.BucketRegion = header.Value;
                }
                else
                {
                    // Treat the other headers just as strings.
                    metadata.AddHeader(header.Key, header.Value);
                }
            }
            return metadata;
        }
    }
}
