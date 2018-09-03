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
        public const string SignerVerion1 = "v1";
        public const string SignerVerion2 = "v2";

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
			"location", "qos", 
            ResponseHeaderOverrides.ResponseCacheControl,
            ResponseHeaderOverrides.ResponseContentDisposition,
            ResponseHeaderOverrides.ResponseContentEncoding,
            ResponseHeaderOverrides.ResponseHeaderContentLanguage,
            ResponseHeaderOverrides.ResponseHeaderContentType,
            ResponseHeaderOverrides.ResponseHeaderExpires
        };

        public static string BuildCanonicalString(string method, string resourcePath,
                                                  ServiceRequest request, string version, 
                                                  IList<string> headersToSign,
                                                  IList<string> signedHeadersList)
        {
            if (version == SignerVerion2)
            {
                return BuildCanonicalStringV2(method, resourcePath, request, 
                                            headersToSign, signedHeadersList);
            }
            else
            {
                return BuildCanonicalStringV1(method, resourcePath, request);
            }
        }

        public static string BuildAuthorizationValue(string accessKeyId, string signature,
                                                     List<string> signedHeadersList, string version)
        {
            if (version == SignerVerion2)
            {
                string headers = ""; 
                if (signedHeadersList != null)
                {
                    headers = string.Join(";", signedHeadersList);
                }
                if (!string.IsNullOrEmpty(headers))
                {
                    headers = ",AdditionalHeaders:" + headers;
                }
                return "OSS2 AccessKeyId:" + accessKeyId + headers + ",Signature:" + signature;
            }
            else
            {
                return "OSS " + accessKeyId + ":" + signature;
            }
        }

        private static string BuildCanonicalStringV1(string method, string resourcePath,
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
            canonicalString.Append(BuildCanonicalizedResourceV1(resourcePath, request.Parameters));

            return canonicalString.ToString();
        }

        private static string BuildCanonicalizedResourceV1(string resourcePath,
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

        private static string BuildCanonicalStringV2(string method, string resourcePath,
                                                  ServiceRequest request, 
                                                  IList<string> HeadersToSign,
                                                  IList<string> signedHeadersList)
        {
            var canonicalString = new StringBuilder();

            // sign rule:
            // verb + '\n'
            // content_md5  + '\n'
            // content_type + '\n'
            // date + '\n'
            // canonicalized_oss_headers_and_HeadersToSign + '\n'
            // signedHeader_name_exclued_oss-x- + '\n'
            // canonicalized_resource in urlencoded + '\n'

            canonicalString.Append(method).Append(NewLineMarker);

            var headers = request.Headers;
            var content_md5 = "";
            var content_type = "";
            var date = "";
            IDictionary<string, string> headersToSign = new Dictionary<string, string>();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    var lowerKey = header.Key.ToLowerInvariant();

                    if (lowerKey == HttpHeaders.ContentMd5.ToLowerInvariant())
                    {
                        content_md5 = header.Value;
                    }
                    else if (lowerKey == HttpHeaders.ContentType.ToLowerInvariant())
                    {
                        content_type = header.Value;
                    }
                    else if (lowerKey == HttpHeaders.Date.ToLowerInvariant())
                    {
                        date = header.Value;
                    }
                    else if (lowerKey == HttpHeaders.ContentLength.ToLowerInvariant()
                            && Int32.Parse(header.Value) < 0)
                    {
                        continue;
                    }

                    if (lowerKey.StartsWith(OssHeaders.OssPrefix))
                    {
                        headersToSign.Add(lowerKey, header.Value);
                    }
                    else if (HeadersToSign   != null 
                        && signedHeadersList != null
                        && !string.IsNullOrEmpty(header.Value)
                        && HeadersToSign.Contains(header.Key))
                    {
                        headersToSign.Add(lowerKey, header.Value);
                    }
                }
            }

            // Add md5,type and date
            canonicalString.Append(content_md5).Append(NewLineMarker);
            canonicalString.Append(content_type).Append(NewLineMarker);
            canonicalString.Append(date).Append(NewLineMarker);

            // Add all headers to sign into canonical string, 
            // note that these headers should be ordered before adding.
            var sortedHeaders = new List<string>(headersToSign.Keys);
            sortedHeaders.Sort(new KeyComparer());
            foreach (var key in sortedHeaders)
            {
                canonicalString.Append(key).Append(':').Append(headersToSign[key]).Append(NewLineMarker);
                if (!key.StartsWith(OssHeaders.OssPrefix))
                {
                    signedHeadersList.Add(key);
                }
            }

            // Add additionalHeadersKey
            if (signedHeadersList != null)
            {
                canonicalString.Append(string.Join(";", signedHeadersList));
            }
            canonicalString.Append(NewLineMarker);

            // Add canonical resource
            canonicalString.Append(BuildCanonicalizedResourceV2(resourcePath, request.Parameters));

            return canonicalString.ToString();
        }

        private static string BuildCanonicalizedResourceV2(string resourcePath,
                                                         IDictionary<string, string> parameters)
        {
            var canonicalizedResource = new StringBuilder();

            canonicalizedResource.Append(HttpUtils.EncodeUri(resourcePath, HttpUtils.Utf8Charset));

            if (parameters != null)
            {
                var parameterNames = new List<string>(parameters.Keys);
                parameterNames.Sort();

                var separator = '?';
                foreach (var paramName in parameterNames)
                {
                    canonicalizedResource.Append(separator);
                    canonicalizedResource.Append(HttpUtils.EncodeUri(paramName, HttpUtils.Utf8Charset));
                    var paramValue = parameters[paramName];
                    if (!string.IsNullOrEmpty(paramValue))
                        canonicalizedResource.Append("=").Append(HttpUtils.EncodeUri(paramValue, HttpUtils.Utf8Charset));

                    separator = '&';
                }
            }

            return canonicalizedResource.ToString();
        }
    }
}
