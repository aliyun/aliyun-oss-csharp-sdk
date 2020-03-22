/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

using Aliyun.OSS.Common.Communication;

namespace Aliyun.OSS.Util
{
    internal static class SignUtils
    {
        private class KeyComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return String.Compare(x, y, StringComparison.Ordinal);
            }
        }

        private const string NewLineMarker = "\n";

        private static readonly IList<string> ParamtersToSign = new List<string> {
            "acl", "uploadId", "partNumber", "uploads", "cors", "logging", 
            "website", "delete", "referer", "lifecycle", "security-token","append", 
            "position", "x-oss-process", "restore", "bucketInfo", "stat", "symlink", 
            "location", "qos", "policy", "tagging", "requestPayment", "x-oss-traffic-limit",
            "objectMeta", "encryption", "versioning", "versionId", "versions",
            ResponseHeaderOverrides.ResponseCacheControl,
            ResponseHeaderOverrides.ResponseContentDisposition,
            ResponseHeaderOverrides.ResponseContentEncoding,
            ResponseHeaderOverrides.ResponseHeaderContentLanguage,
            ResponseHeaderOverrides.ResponseHeaderContentType,
            ResponseHeaderOverrides.ResponseHeaderExpires
        };

        public static string BuildCanonicalString(string method, string resourcePath,
                                                  ServiceRequest request)
        {
            var canonicalString = new StringBuilder();
            
            canonicalString.Append(method).Append(NewLineMarker);

            var headers = request.Headers;
            IDictionary<string, string> headersToSign = new Dictionary<string, string>();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    var lowerKey = header.Key.ToLowerInvariant();

                    if (lowerKey == HttpHeaders.ContentType.ToLowerInvariant()
                        || lowerKey == HttpHeaders.ContentMd5.ToLowerInvariant()
                        || lowerKey == HttpHeaders.Date.ToLowerInvariant()
                        || lowerKey.StartsWith(OssHeaders.OssPrefix))
                    {
                        headersToSign.Add(lowerKey, header.Value);
                    }
                }
            }

            if (!headersToSign.ContainsKey(HttpHeaders.ContentType.ToLowerInvariant()))
                headersToSign.Add(HttpHeaders.ContentType.ToLowerInvariant(), "");
            if (!headersToSign.ContainsKey(HttpHeaders.ContentMd5.ToLowerInvariant()))
                headersToSign.Add(HttpHeaders.ContentMd5.ToLowerInvariant(), "");

            // Add all headers to sign into canonical string, 
            // note that these headers should be ordered before adding.
            var sortedHeaders = new List<string>(headersToSign.Keys);
            sortedHeaders.Sort(new KeyComparer());
            foreach (var key in sortedHeaders)
            {
                var value = headersToSign[key];
                if (key.StartsWith(OssHeaders.OssPrefix))
                    canonicalString.Append(key).Append(':').Append(value);
                else
                    canonicalString.Append(value);

                canonicalString.Append(NewLineMarker);
            }

            // Add canonical resource
            canonicalString.Append(BuildCanonicalizedResource(resourcePath, request.Parameters));

            return canonicalString.ToString();
        }

        private static string BuildCanonicalizedResource(string resourcePath,
                                                         IDictionary<string, string> parameters)
        {
            var canonicalizedResource = new StringBuilder();
            
            canonicalizedResource.Append(resourcePath);

            if (parameters != null)
            {
                var parameterNames = new List<string>(parameters.Keys);
                parameterNames.Sort();

                var separator = '?';
                foreach (var paramName in parameterNames)
                {
                    if (!ParamtersToSign.Contains(paramName))
                        continue;

                    canonicalizedResource.Append(separator);
                    canonicalizedResource.Append(paramName);
                    var paramValue = parameters[paramName];
                    if (!string.IsNullOrEmpty(paramValue))
                        canonicalizedResource.Append("=").Append(paramValue);

                    separator = '&';
                }
            }

            return canonicalizedResource.ToString();
        }
    }
}
