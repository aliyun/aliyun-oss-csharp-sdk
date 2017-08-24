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
    internal class GetObjectMetadataResponseDeserializer : ResponseDeserializer<ObjectMetadata, ObjectMetadata>
    {
        public GetObjectMetadataResponseDeserializer()
            : base(null)
        { }

        public override ObjectMetadata Deserialize(ServiceResponse xmlStream)
        {
            var metadata = new ObjectMetadata();
            foreach(var header in xmlStream.Headers)
            {
                if (header.Key.StartsWith(OssHeaders.OssUserMetaPrefix, false, CultureInfo.InvariantCulture))
                {
                    // The key of user in the metadata should not contain the prefix.
                    metadata.UserMetadata.Add(header.Key.Substring(OssHeaders.OssUserMetaPrefix.Length),
                                              header.Value);
                }
                else if (string.Equals(header.Key, HttpHeaders.ContentLength, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Content-Length. Parse should not fail.
                    metadata.ContentLength = long.Parse(header.Value, CultureInfo.InvariantCulture);
                }
                else if (string.Equals(header.Key, HttpHeaders.ETag, StringComparison.InvariantCultureIgnoreCase))
                {
                    metadata.ETag = OssUtils.TrimQuotes(header.Value);
                }
                else if (string.Equals(header.Key, HttpHeaders.LastModified, StringComparison.InvariantCultureIgnoreCase))
                {
                    metadata.LastModified = DateUtils.ParseRfc822Date(header.Value);
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
