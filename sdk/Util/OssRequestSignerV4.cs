/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Util
{
    internal class OssRequestSignerV4 : OssRequestSigner
    {
        private const string UnsignedPayload = "UNSIGNED-PAYLOAD";
        private const string X_Oss_Content_SHA256 = "x-oss-content-sha256";
        private const string DateTimeFormat = "yyyyMMdd'T'HHmmss'Z'";
        private const string DateFormat = "yyyyMMdd";

        public OssRequestSignerV4()
        {
        }

        public override void Sign(ServiceRequest request, ICredentials credentials)
        {
            if (IsAnonymousCredentials(credentials))
                return;

            // Date
            var signTime = DateTime.UtcNow;
            if (request.Headers.ContainsKey(HttpHeaders.Date))
            {
                signTime = DateUtils.ParseRfc822Date(request.Headers[HttpHeaders.Date]);
            }
            var datetime = FormatDateTime(signTime);
            var date = FormatDate(signTime); ;

            // Credentials information
            if (credentials.UseToken)
            {
                request.Headers[HttpHeaders.SecurityToken] = credentials.SecurityToken;
            }

            // Other Headers
            request.Headers[X_Oss_Content_SHA256] = UnsignedPayload;
            request.Headers["x-oss-date"] = datetime;

            // Scope
            var scope = string.Format("{0}/{1}/{2}/aliyun_v4_request", date, Region, Product);

            var additionalHeaders = GetAdditionalHeaders(request.Headers, AdditionalHeaders);

            // CanonicalRequest
            var canonicalRequest = CanonicalizeRequest(request, additionalHeaders);

            // StringToSign
            var stringToSign = CalcStringToSign(datetime, scope, canonicalRequest);

            // Signature
            var signature = CalcSignature(credentials.AccessKeySecret, date, Region, Product, stringToSign);

            // Credential
            var sb = new StringBuilder();
            sb.AppendFormat("OSS4-HMAC-SHA256 Credential={0}/{1}", credentials.AccessKeyId, scope);
            if (additionalHeaders.Count > 0)
            {
                sb.AppendFormat(",AdditionalHeaders={0}", CanonicalizeHeaderNames(additionalHeaders));
            }
            sb.AppendFormat(",Signature={0}", signature);

            request.Headers[HttpHeaders.Authorization] = sb.ToString();

            Console.WriteLine("canonicalRequest:{0}\n", canonicalRequest);
            Console.WriteLine("stringToSign:{0}\n", stringToSign);
            Console.WriteLine("signature:{0}\n", signature);
        }

        public override void PreSign(ServiceRequest request, SigningContext signingContext)
        {
            if (signingContext == null || 
                signingContext.Expiration == null ||
                IsAnonymousCredentials(signingContext.Credentials))
                return;

            // Date
            var signTime = DateTime.UtcNow;
            if (signingContext.SignTime != null)
            {
                signTime = signingContext.SignTime;
            }
            var datetime = FormatDateTime(signTime);
            var date = FormatDate(signTime);
            var expires = ((long)signingContext.Expiration.Subtract(signTime).TotalSeconds).ToString(CultureInfo.InvariantCulture);

            // Scope
            var scope = string.Format("{0}/{1}/{2}/aliyun_v4_request", date, Region, Product);

            var additionalHeaders = GetAdditionalHeaders(request.Headers, AdditionalHeaders);

            // Credentials information
            var credentials = signingContext.Credentials;
            if (credentials.UseToken)
            {
                request.Parameters.Add("x-oss-security-token", credentials.SecurityToken);
            }
            request.Parameters.Add("x-oss-signature-version", "OSS4-HMAC-SHA256");
            request.Parameters.Add("x-oss-date", datetime);
            request.Parameters.Add("x-oss-expires", expires);
            request.Parameters.Add("x-oss-credential", string.Format("{0}/{1}", credentials.AccessKeyId, scope));
            if (additionalHeaders != null && additionalHeaders.Count > 0)
            {
                request.Parameters.Add("x-oss-additional-headers", CanonicalizeHeaderNames(additionalHeaders));
            }

            // CanonicalRequest
            var canonicalRequest = CanonicalizeRequest(request, additionalHeaders);

            // StringToSign
            var stringToSign = CalcStringToSign(datetime, scope, canonicalRequest);

            // Signature
            var signature = CalcSignature(credentials.AccessKeySecret, date, Region, Product, stringToSign);

            // Credential
            request.Parameters.Add("x-oss-signature", signature);

            Console.WriteLine("canonicalRequest:{0}\n", canonicalRequest);
            Console.WriteLine("stringToSign:{0}\n", stringToSign);
            Console.WriteLine("signature:{0}\n", signature);
        }

        private static string FormatDateTime(DateTime dtime)
        {
            return dtime.ToUniversalTime().ToString(DateTimeFormat, CultureInfo.InvariantCulture);
        }

        private static string FormatDate(DateTime dtime)
        {
            return dtime.ToUniversalTime().ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        private string CanonicalizeRequest(ServiceRequest request, List<string> additionalHeaders)
        {
            /*
                Canonical Request
                HTTP Verb + "\n" +
                Canonical URI + "\n" +
                Canonical Query String + "\n" +
                Canonical Headers + "\n" +
                Additional Headers + "\n" +
                Hashed PayLoad
            */
            var httpMethod = request.Method.ToString().ToUpperInvariant();

            // Canonical Uri
            var canonicalUri = HttpUtils.EncodePath(getResourcePath());

            // Canonical Query
            var canonicalQueryString = CanonicalizeQuery(request.Parameters);

            // Canonical Headers
            var canonicalHeaderString = CanonicalizeHeaders(request.Headers, additionalHeaders);

            // Additional Headers
            var additionalHeadersString = CanonicalizeHeaderNames(additionalHeaders);

            var hashBody = CanonicalizeBodyHash(request.Headers);

            var canonicalRequest = new StringBuilder();
            canonicalRequest.AppendFormat("{0}\n", httpMethod);
            canonicalRequest.AppendFormat("{0}\n", canonicalUri);
            canonicalRequest.AppendFormat("{0}\n", canonicalQueryString);
            canonicalRequest.AppendFormat("{0}\n", canonicalHeaderString);
            canonicalRequest.AppendFormat("{0}\n", additionalHeadersString);
            canonicalRequest.AppendFormat("{0}", hashBody);

            return canonicalRequest.ToString();
        }


        private static string CanonicalizeQuery(IDictionary<String, String> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return string.Empty;

            var sortedParameters = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var p in parameters)
            {
                sortedParameters[HttpUtils.EncodeUri(p.Key, "utf-8")] = HttpUtils.EncodeUri(p.Value, "utf-8");
            }

            var sb = new StringBuilder();
            foreach (var p in sortedParameters)
            {
                if (sb.Length > 0)
                    sb.Append("&");
                sb.AppendFormat("{0}", p.Key);
                if (p.Value.Length > 0)
                    sb.AppendFormat("={0}", p.Value);
            }

            return sb.ToString();
        }

        private static string CanonicalizeHeaders(IDictionary<string, string> headers, List<string> additionalHeaders)
        {
            if (headers == null || headers.Count == 0)
                return string.Empty;

            var addHeadersMap = new Dictionary<string, string>();
            foreach (var header in additionalHeaders)
            {
                addHeadersMap[header.ToLowerInvariant()] = string.Empty;
            }

            var sortedHeaderMap = new SortedDictionary<string, string>();
            foreach (var header in headers)
            {
                var lowerKey = header.Key.ToLowerInvariant();
                if (IsDefaultSignedHeader(lowerKey) ||
                    addHeadersMap.ContainsKey(lowerKey))
                {
                    sortedHeaderMap[lowerKey] = header.Value.Trim();
                }
            }

            var sb = new StringBuilder();
            foreach (var header in sortedHeaderMap)
            {
                sb.AppendFormat("{0}:{1}\n", header.Key, header.Value.Trim());
            }

            return sb.ToString();
        }

        private static string CanonicalizeHeaderNames(List<string> additionalHeaders)
        {
            var headersToSign = new List<string>(additionalHeaders);
            headersToSign.Sort(StringComparer.OrdinalIgnoreCase);
            var sb = new StringBuilder();
            foreach (var header in headersToSign)
            {
                if (sb.Length > 0)
                    sb.Append(";");
                sb.Append(header.ToLower());
            }
            return sb.ToString();
        }

        private static string CanonicalizeBodyHash(IDictionary<string, string> headers)
        {
            if (headers != null && headers.ContainsKey(X_Oss_Content_SHA256))
            {
                return headers[X_Oss_Content_SHA256];
            }

            return UnsignedPayload;
        }

        private static bool IsDefaultSignedHeader(string lowerKey)
        {
            if (lowerKey == "content-type" ||
                lowerKey == "content-md5" ||
                lowerKey.StartsWith(OssHeaders.OssPrefix))
            {
                return true;
            }
            return false;
        }

        private static List<string> GetAdditionalHeaders(IDictionary<string, string> headers, List<string> additionalHeaders)
        {
            var keys = new List<string>();
            if (additionalHeaders == null || 
                additionalHeaders.Count == 0 ||
                headers == null ||
                headers.Count == 0)
            {
                return keys;
            }

            foreach (var k in additionalHeaders)
            {
                var lowk = k.ToLowerInvariant();
                if (IsDefaultSignedHeader(lowk))
                {
                    continue;
                }
                else if (headers.ContainsKey(lowk))
                {
                    keys.Add(lowk);
                }
            }
            return keys;
        }
        
        private static string CalcStringToSign(string datetime, string scope, string canonicalRequest)
        {
            /*
            StringToSign
            "OSS4-HMAC-SHA256" + "\n" +
            TimeStamp + "\n" +
            Scope + "\n" +
            Hex(SHA256Hash(Canonical Request))
            */
            var hash = HashAlgorithm.Create("SHA-256");
            var hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(canonicalRequest));

            return "OSS4-HMAC-SHA256" + "\n" +
                datetime + "\n" +
                scope + "\n" +
                OssUtils.ToHexString(hashBytes, true);
        }

        private static string CalcSignature(string accessKeySecret, string date, string region, string product, string stringToSign)
        {
            var kha = KeyedHashAlgorithm.Create("HMACSHA256");

            var ksecret = Encoding.UTF8.GetBytes("aliyun_v4" + accessKeySecret);

            kha.Key = ksecret;
            var hashDate = kha.ComputeHash(Encoding.UTF8.GetBytes(date));

            kha.Key = hashDate;
            var hashRegion = kha.ComputeHash(Encoding.UTF8.GetBytes(region));

            kha.Key = hashRegion;
            var hashProduct = kha.ComputeHash(Encoding.UTF8.GetBytes(product));

            kha.Key = hashProduct;
            var signingKey = kha.ComputeHash(Encoding.UTF8.GetBytes("aliyun_v4_request"));

            kha.Key = signingKey;
            var signature = kha.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));

            Console.WriteLine("ksecret:{0}\n", OssUtils.ToHexString(ksecret, true));
            Console.WriteLine("hashDate:{0}\n", OssUtils.ToHexString(hashDate, true));
            Console.WriteLine("hashRegion:{0}\n", OssUtils.ToHexString(hashRegion, true));
            Console.WriteLine("hashProduct:{0}\n", OssUtils.ToHexString(hashProduct, true));
            Console.WriteLine("signature:{0}\n", OssUtils.ToHexString(signature, true));

            return OssUtils.ToHexString(signature, true);
        }
    }
}

