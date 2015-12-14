/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Aliyun.OSS.Common;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Commands;
using Aliyun.OSS.Domain;
using Aliyun.OSS.Properties;

namespace Aliyun.OSS.Util
{
    /// <summary>
    /// 一些SDK中常用的工具方法
    /// </summary>
    public static class OssUtils
    {
        private const string DefaultBaseChars = "0123456789ABCDEF";
        private const string CharsetName = "utf-8";

        /// <summary>
        /// 普通上传支持的最大文件大小：5G
        /// </summary>
        public const long MaxFileSize = 5 * 1024 * 1024 * 1024L;

        /// <summary>
        /// 前缀最大长度，1K
        /// </summary>
        public const int MaxPrefixStringSize = 1024;

        /// <summary>
        /// marker最大长度，1K
        /// </summary>
        public const int MaxMarkerStringSize = 1024;

        /// <summary>
        /// 分隔字符串最大长度,1K
        /// </summary>
        public const int MaxDelimiterStringSize = 1024;

        /// <summary>
        /// 最多返回的文件数,1000
        /// </summary>
        public const int MaxReturnedKeys = 1000;

        /// <summary>
        /// 一次最多删除的文件数,1000
        /// </summary>
        public const int DeleteObjectsUpperLimit = 1000;

        /// <summary>
        /// 存储空间的跨域资源共享规则数量限制,10
        /// </summary>
        public const int BucketCorsRuleLimit = 10;

        /// <summary>
        /// 生命周期规则数量限制,1000
        /// </summary>
        public const int LifecycleRuleLimit = 1000;

        /// <summary>
        /// 文件长度最长限制,1023
        /// </summary>
        public const int ObjectNameLengthLimit = 1023;

        /// <summary>
        /// 分片个数限制,10000
        /// </summary>
        public const int PartNumberUpperLimit = 10000;

        /// <summary>
        /// 默认分片大小，8M
        /// </summary>
        public const long DefaultPartSize = 8 * 1024 * 1024;

        /// <summary>
        /// 分片上传或者拷贝时，最小分片大小,100K
        /// </summary>
        public const long PartSizeLowerLimit = 100 * 1024;

        /// <summary>
        /// 判断存储空间（bucket）名字是否合法
        /// </summary>
        /// <param name="bucketName">存储空间的名称</param>
        /// <returns>是否合法</returns>
        public static bool IsBucketNameValid(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName))
                return false;

            const string pattern = "^[a-z0-9][a-z0-9\\-]{1,61}[a-z0-9]$";
            var regex = new Regex(pattern);
            var m = regex.Match(bucketName);
            return m.Success;
        }

        /// <summary>
        /// 判断文件名是否合法
        /// </summary>
        /// <param name="key">文件名称</param>
        /// <returns>是否合法</returns>
        public static bool IsObjectKeyValid(string key)
        {
            if (string.IsNullOrEmpty(key) || key.StartsWith("/") || key.StartsWith("\\"))
                return false;

            var byteCount = Encoding.GetEncoding(CharsetName).GetByteCount(key);
            return byteCount <= ObjectNameLengthLimit;
        }

        internal static String MakeResourcePath(Uri endpoint, string bucket, string key)
        {
            String resourcePath = (key == null) ? string.Empty : key;

            if (IsIp(endpoint))
            {
                resourcePath = bucket + "/" + resourcePath;
            }
            
            return UrlEncodeKey(resourcePath);
        }

        internal static Uri MakeBucketEndpoint(Uri endpoint, string bucket, ClientConfiguration conf)
        {
            var uri = new Uri(endpoint.Scheme + "://"
                           + ((bucket != null && !conf.IsCname && !IsIp(endpoint)) 
                               ? (bucket + "." + endpoint.Host) : endpoint.Host)
                           + ((endpoint.Port != 80) ? (":" + endpoint.Port) : ""));
            return uri;
        }

        /// <summary>
        /// 判断endpoint是否是ip形式
        /// </summary>
        /// <param name="endpoint">endpoint的值</param>
        /// <returns>是否是ip</returns>
        private static bool IsIp(Uri endpoint)
        {
            return Regex.IsMatch(endpoint.Host, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 对key做编码
        /// </summary>
        /// <param name="key">需要编码的字符</param>
        /// <returns>编码后的字符</returns>
        private static String UrlEncodeKey(String key)
        {
            const char separator = '/';
            var segments = key.Split(separator);
            
            var encodedKey = new StringBuilder();
            encodedKey.Append(HttpUtils.EncodeUri(segments[0], CharsetName));
            for (var i = 1; i < segments.Length; i++)
                encodedKey.Append(separator).Append(HttpUtils.EncodeUri(segments[i], CharsetName));

            if (key.EndsWith(separator.ToString()))
            {
                // String#split ignores trailing empty strings, e.g., "a/b/" will be split as a 2-entries array,
                // so we have to append all the trailing slash to the uri.
                foreach (var ch in key)
                {
                    if (ch == separator)
                        encodedKey.Append(separator);
                    else
                        break;
                }
            }

            return encodedKey.ToString();
        }

        /// <summary>
        /// 删掉eTag起始和结束处的引号
        /// </summary>
        /// <param name="eTag">需要删除引号的eTag值</param>
        /// <returns>删除起始和结束处的引号后的值</returns>
        public static string TrimQuotes(string eTag)
        {
            return eTag != null ? eTag.Trim('\"') : null;
        }

        /// <summary>
        /// 对输入流计算md5值
        /// </summary>
        /// <param name="input">待计算的输入流</param>
        /// <param name="partSize">计算多长的输入流</param>
        /// <returns>流的md5值</returns>
        public static string ComputeContentMd5(Stream input, long partSize)
        {
            using (var md5 = MD5.Create())
            {
                int readSize = (int)partSize;
                long pos = input.Position;
                byte[] buffer = new byte[readSize];
                readSize = input.Read(buffer, 0, readSize);

                var data = md5.ComputeHash(buffer, 0, readSize);
                var charset = DefaultBaseChars.ToCharArray(); 
                var sBuilder = new StringBuilder();
                foreach (var b in data)
                {
                    sBuilder.Append(charset[b >> 4]);
                    sBuilder.Append(charset[b & 0x0F]);
                }
                input.Seek(pos, SeekOrigin.Begin);
                return Convert.ToBase64String(data);
            } 
        }

        /// <summary>
        /// 判断webpage是否合法
        /// </summary>
        /// <param name="webpage">需要验证的webpage值</param>
        /// <returns>是否合法</returns>
        public static bool IsWebpageValid(string webpage)
        {
            const string pageSuffix = ".html";
            return !string.IsNullOrEmpty(webpage) && webpage.EndsWith(pageSuffix)
                   && webpage.Length > pageSuffix.Length;
        }

        /// <summary>
        /// 判断日志前缀是否合法
        /// </summary>
        /// <param name="loggingPrefix">需要判断的日志前缀</param>
        /// <returns>是否合法</returns>
        public static bool IsLoggingPrefixValid(string loggingPrefix)
        {
            if (string.IsNullOrEmpty(loggingPrefix))
                return true;

            const string pattern = "^[a-zA-Z][a-zA-Z0-9\\-]{0,31}$";
            var regex = new Regex(pattern);
            var m = regex.Match(loggingPrefix);
            return m.Success;
        }

        internal static string BuildPartCopySource(string bucketName, string objectKey)
        {
            return "/" + bucketName + "/" + UrlEncodeKey(objectKey);
        }

        internal static string BuildCopyObjectSource(string bucketName, string objectKey)
        {
            return "/" + bucketName + "/" + UrlEncodeKey(objectKey);
        }

        internal static bool IsPartNumberInRange(int? partNumber)
        {
            return (partNumber.HasValue && partNumber > 0
                && partNumber <= OssUtils.PartNumberUpperLimit);
        }

        internal static void CheckBucketName(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "bucketName");
            if (!IsBucketNameValid(bucketName))
                throw new ArgumentException(OssResources.BucketNameInvalid, "bucketName");
        }

        internal static void CheckObjectKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "key");
            if (!IsObjectKeyValid(key))
                throw new ArgumentException(OssResources.ObjectKeyInvalid, "key");
        }

        internal static string DetermineOsVersion()
        {
            try
            {
                var os = Environment.OSVersion;
                return "windows " + os.Version.Major + "." + os.Version.Minor;
            }
            catch (InvalidOperationException /* ex */)
            {
                return "Unknown OSVersion";
            }
        }

        internal static string DetermineSystemArchitecture()
        {
            return (IntPtr.Size == 8) ? "x86_64" : "x86";
        }

        internal static string JoinETag(IEnumerable<string> etags)
        {
            StringBuilder result = new StringBuilder();

            var first = true;
            foreach (var etag in etags)
            {
                if (!first)
                    result.Append(", ");
                result.Append(etag);
                first = false;
            }

            return result.ToString();
        }

        internal static ClientConfiguration GetClientConfiguration(IServiceClient serviceClient)
        {
            var outerClient = (RetryableServiceClient) serviceClient;
            var innerClient = (ServiceClient)outerClient.InnerServiceClient();
            return innerClient.Configuration;
        }

        internal static IAsyncResult BeginOperationHelper<TCommand>(TCommand cmd, AsyncCallback callback, Object state)
            where TCommand : OssCommand
        {
            var retryableAsyncResult = cmd.AsyncExecute(callback, state) as RetryableAsyncResult;
            if (retryableAsyncResult == null)
            {
                throw new ArgumentException("retryableAsyncResult should not be null");
            }
            return retryableAsyncResult.InnerAsyncResult;
        }

        internal static TResult EndOperationHelper<TResult>(IServiceClient serviceClient, IAsyncResult asyncResult)
        {
            var response = EndOperationHelper(serviceClient, asyncResult);
            var retryableAsyncResult = asyncResult as RetryableAsyncResult;
            Debug.Assert(retryableAsyncResult != null);
            OssCommand<TResult> cmd = (OssCommand<TResult>)retryableAsyncResult.Context.Command;
            return cmd.DeserializeResponse(response);
        }

        private static ServiceResponse EndOperationHelper(IServiceClient serviceClient, IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");

            var retryableAsyncResult = asyncResult as RetryableAsyncResult;
            if (retryableAsyncResult == null)
                throw new ArgumentException("retryableAsyncResult should not be null");

            ServiceClientImpl.HttpAsyncResult httpAsyncResult =
                retryableAsyncResult.InnerAsyncResult as ServiceClientImpl.HttpAsyncResult;
            return serviceClient.EndSend(httpAsyncResult);
        }

        internal static void CheckCredentials(string accessKeyId, string accessKeySecret)
        {
            if (string.IsNullOrEmpty(accessKeyId))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "accessKeyId");
            if (string.IsNullOrEmpty(accessKeySecret))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "accessKeySecret");
        }
    }
}
