/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System;

namespace Aliyun.OSS.Util
{
    public static class HttpUtils
    {
        public const string Utf8Charset = "utf-8";
        public const string Iso88591Charset = "iso-8859-1";
        public const string UrlEncodingType = "url";
        public const string HttpProto = "http://";
        public const string HttpsProto = "https://";
        public const string DefaultContentType = "application/octet-stream";
        private const string UrlAllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        private static IDictionary<string, string> _mimeDict = new Dictionary<string, string>();

        static HttpUtils()
        {
            char[] delimiterChars = { ' ', '\t'};

            var mimeLines = Properties.Resources.MimeData.ToString().Split('\n');
            foreach (var mimeLine in mimeLines)
            {
                var mimeParts = mimeLine.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                if (mimeParts.Length != 2)
                {
                    continue;
                }

                var key = mimeParts[0].Trim();
                var value = mimeParts[1].Trim();
                _mimeDict[key] = value;
            }
        }


        public static string ConbineQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var isFirst = true;
            var queryString = new StringBuilder();

            foreach (var p in parameters)
            {
                if (!isFirst)
                    queryString.Append("&");                
                
                isFirst = false;

                queryString.Append(p.Key);
                if (!string.IsNullOrEmpty(p.Value))
                    queryString.Append("=").Append(EncodeUri(p.Value, Utf8Charset));
            }

            return queryString.ToString();
        }
        
        public static string EncodeUri(string uriToEncode, string charset)
        {
            if (string.IsNullOrEmpty(uriToEncode))
                return string.Empty;

            const string escapeFlag = "%";
            var encodedUri = new StringBuilder(uriToEncode.Length * 2);
            var bytes = Encoding.GetEncoding(charset).GetBytes(uriToEncode);
            foreach (var b in bytes)
            {
                char ch = (char)b;
                if (UrlAllowedChars.IndexOf(ch) != -1)
                    encodedUri.Append(ch);
                else
                {
                    encodedUri.Append(escapeFlag).Append(
                        string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)b));
                }
            }

            return encodedUri.ToString();
        }

        public static string DecodeUri(string uriToDecode)
        {
            if (!string.IsNullOrEmpty(uriToDecode))
            {
                uriToDecode = uriToDecode.Replace("+", " ");
                return Uri.UnescapeDataString(uriToDecode);
            }
            return string.Empty;
        }

        public static string Reencode(string data, string fromCharset, string toCharset)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            var bytes = Encoding.GetEncoding(fromCharset).GetBytes(data);
            return Encoding.GetEncoding(toCharset).GetString(bytes);
        }

        public static string GetContentType(string key, string file)
        {
            string fileType = ""; 
            try
            {
                if (file != null)
                {
                    fileType = System.IO.Path.GetExtension(file);
                }
                else if (key != null)
                {
                    fileType = System.IO.Path.GetExtension(key);
                }
            }
            catch
            {
            }

            fileType = fileType.Trim().TrimStart(new char[1] { '.' }).ToLower();
            
            if (_mimeDict.ContainsKey(fileType))
            {
                return _mimeDict[fileType];
            }

            return DefaultContentType;
        }
    }
}
