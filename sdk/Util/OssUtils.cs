/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
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
using Aliyun.OSS.Common.Internal;

namespace Aliyun.OSS.Util
{
    /// <summary>
    /// The equvalent delegate of .Net4.0's System.Func. This is to make this code compatible with .Net 2.0
    /// </summary>
    public delegate TResult OssFunc<in T, out TResult>(T arg);

    /// <summary>
    /// The equvalent delegate of .Net 4.0's System.Action.
    /// </summary>
    public delegate void OssAction();

    public delegate void OssAction<in T>(T obj);

    /// <summary>
    /// Some common utility methods and constants
    /// </summary>
    public static class OssUtils
    {
        private const string DefaultBaseChars = "0123456789ABCDEF";
        private const string CharsetName = "utf-8";

        /// <summary>
        /// Max normal file size: 5G
        /// </summary>
        public const long MaxFileSize = 5 * 1024 * 1024 * 1024L;

        /// <summary>
        /// Max prefix length
        /// </summary>
        public const int MaxPrefixStringSize = 1024;

        /// <summary>
        /// Marker's max length.
        /// </summary>
        public const int MaxMarkerStringSize = 1024;

        /// <summary>
        /// Max delimiter length.
        /// </summary>
        public const int MaxDelimiterStringSize = 1024;

        /// <summary>
        /// Max keys to return in one call.
        /// </summary>
        public const int MaxReturnedKeys = 1000;

        /// <summary>
        /// Max objects to delete in multiple object deletion call.
        /// </summary>
        public const int DeleteObjectsUpperLimit = 1000;

        /// <summary>
        /// Max CORS rule count per bucket
        /// </summary>
        public const int BucketCorsRuleLimit = 10;

        /// <summary>
        /// Max lifecycle rule count per bucket.
        /// </summary>
        public const int LifecycleRuleLimit = 1000;

        /// <summary>
        /// Max object key's length.
        /// </summary>
        public const int ObjectNameLengthLimit = 1023;

        /// <summary>
        /// Max part number's upper limit.
        /// </summary>
        public const int PartNumberUpperLimit = 10000;

        /// <summary>
        /// Default part size.
        /// </summary>
        public const long DefaultPartSize = 8 * 1024 * 1024;

        /// <summary>
        /// Minimal part size in multipart upload or copy.
        /// </summary>
        public const long PartSizeLowerLimit = 100 * 1024;

        /// <summary>
        /// Max file path length.
        /// </summary>
        public const int MaxPathLength = 124;

        /// <summary>
        /// Min file path
        /// </summary>
        public const int MinPathLength = 4;

        /// <summary>
        /// Check if the bucket name is valid,.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <returns>true:valid bucket name</returns>
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
        /// validates the object key
        /// </summary>
        /// <param name="key">object key</param>
        /// <returns>true:valid object key</returns>
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
        /// checks if the endpoint is in IP format.
        /// </summary>
        /// <param name="endpoint">endpoint to check</param>
        /// <returns>true: the endpoint is ip.</returns>
        private static bool IsIp(Uri endpoint)
        {
            return Regex.IsMatch(endpoint.Host, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// Applies the Url encoding on the key
        /// </summary>
        /// <param name="key">the object key to encode</param>
        /// <returns>The encoded key</returns>
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
        /// Trims quotes in the ETag
        /// </summary>
        /// <param name="eTag">The Etag to trim</param>
        /// <returns>The Etag without the quotes</returns>
        public static string TrimQuotes(string eTag)
        {
            return eTag != null ? eTag.Trim('\"') : null;
        }

        /// <summary>
        /// Compute the MD5 on the input stream with the given size.
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="partSize">the part size---it could be less than the stream size</param>
        /// <returns>MD5 digest value</returns>
        public static string ComputeContentMd5(Stream input, long partSize)
        {
            using (var md5Calculator = MD5.Create())
            {
                long position = input.Position;
                var partialStream = new PartialWrapperStream(input, partSize);
                var md5Value = md5Calculator.ComputeHash(partialStream);
                input.Seek(position, SeekOrigin.Begin);
                return Convert.ToBase64String(md5Value);
            } 
        }

        /// <summary>
        /// Computes the content crc64.
        /// </summary>
        /// <returns>The content crc64.</returns>
        /// <param name="input">Input.</param>
        /// <param name="length">stream length</param>
        public static string ComputeContentCrc64(Stream input, long length)
        {
            using(Crc64Stream crcStream = new Crc64Stream(input, null, length))
            {
                byte[] buffer = new byte[32 * 1024];
                int readCount = 0;
                while(readCount < length)
                {
                    int read = crcStream.Read(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        break;
                    }
                    readCount += read;
                }

                if (crcStream.CalculatedHash == null)
                {
                    crcStream.CalculateHash();
                }

                if (crcStream.CalculatedHash == null || crcStream.CalculatedHash.Length == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return BitConverter.ToUInt64(crcStream.CalculatedHash, 0).ToString();
                }
            }
        }

        /// <summary>
        /// Checks if the webpage url is valid.
        /// </summary>
        /// <param name="webpage">The wenpage url to check</param>
        /// <returns>true: the url is valid.</returns>
        public static bool IsWebpageValid(string webpage)
        {
            const string pageSuffix = ".html";
            return !string.IsNullOrEmpty(webpage) && webpage.EndsWith(pageSuffix)
                   && webpage.Length > pageSuffix.Length;
        }

        /// <summary>
        /// Checks if the logging prefix is valid.
        /// </summary>
        /// <param name="loggingPrefix">The logging prefix to check</param>
        /// <returns>true:valid logging prefix</returns>
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
            RetryableAsyncResult retryableAsyncResult = asyncResult as RetryableAsyncResult;
            if (retryableAsyncResult == null)
            {
                retryableAsyncResult = asyncResult.AsyncState as RetryableAsyncResult;
            }

            if (retryableAsyncResult != null)
            {
                using (retryableAsyncResult)
                {
                    Debug.Assert(retryableAsyncResult != null);
                    OssCommand<TResult> cmd = (OssCommand<TResult>)retryableAsyncResult.Context.Command;
                    return cmd.DeserializeResponse(response);
                }
            }
            else
            {
                throw new ArgumentException("asyncResult");
            }
        }

        private static ServiceResponse EndOperationHelper(IServiceClient serviceClient, IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");

            var retryableAsyncResult = asyncResult as RetryableAsyncResult;
            if (retryableAsyncResult == null)
            {
                ServiceClientImpl.HttpAsyncResult httpAsyncResult =
                                     asyncResult as ServiceClientImpl.HttpAsyncResult;
                if (httpAsyncResult == null)
                {
                    throw new ArgumentException("asyncResult");
                }

                retryableAsyncResult = httpAsyncResult.AsyncState as RetryableAsyncResult;
            }

            if (retryableAsyncResult != null)
            {
                ServiceClientImpl.HttpAsyncResult httpAsyncResult =
                    retryableAsyncResult.InnerAsyncResult as ServiceClientImpl.HttpAsyncResult;
                return serviceClient.EndSend(httpAsyncResult);
            }
            else
            {
                throw new ArgumentException("asyncResult");
            }
        }

        internal static void CheckCredentials(string accessKeyId, string accessKeySecret)
        {
            if (string.IsNullOrEmpty(accessKeyId))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "accessKeyId");
            if (string.IsNullOrEmpty(accessKeySecret))
                throw new ArgumentException(Resources.ExceptionIfArgumentStringIsNullOrEmpty, "accessKeySecret");
        }

        internal static Stream SetupProgressListeners(Stream originalStream,
                                                      long progressUpdateInterval,
                                                      object sender,
                                                      EventHandler<StreamTransferProgressArgs> callback)
        {
            return SetupProgressListeners(originalStream, originalStream.Length, progressUpdateInterval, sender, callback);
        }

        internal static Stream SetupProgressListeners(Stream originalStream,
                                                      long contentLength,
                                                      long progressUpdateInterval,
                                                      object sender,
                                                      EventHandler<StreamTransferProgressArgs> callback)
        {
            return SetupProgressListeners(originalStream, contentLength, 0, progressUpdateInterval, sender, callback);
        }

        /// <summary>
        /// Sets up the progress listeners
        /// </summary>
        /// <param name="originalStream">The content stream</param>
        /// <param name="contentLength">The length of originalStream</param>
        /// <param name="totalBytesRead">The length which has read</param>
        /// <param name="progressUpdateInterval">The interval at which progress needs to be published</param>
        /// <param name="sender">The objects which is trigerring the progress changes</param>
        /// <param name="callback">The callback which will be invoked when the progress changed event is trigerred</param>
        /// <returns>an <see cref="EventStream"/> object, incase the progress is setup, else returns the original stream</returns>
        internal static Stream SetupProgressListeners(Stream originalStream,
                                                      long contentLength,
                                                      long totalBytesRead,
                                                      long progressUpdateInterval,
                                                      object sender,
                                                      EventHandler<StreamTransferProgressArgs> callback)
        {
            var eventStream = new EventStream(originalStream, true);
            var tracker = new StreamReadTracker(sender, callback, contentLength, totalBytesRead, progressUpdateInterval);
            eventStream.OnRead += tracker.ReadProgress;
            return eventStream;
        }

        internal static Stream SetupDownloadProgressListeners(Stream originalStream,
                                                      long contentLength,
                                                      long totalBytesWrite,
                                                      long progressUpdateInterval,
                                                      object sender,
                                                      EventHandler<StreamTransferProgressArgs> callback)
        {
            var eventStream = new EventStream(originalStream, true);
            var tracker = new StreamTransferTracker(sender, callback, contentLength, totalBytesWrite, progressUpdateInterval);
            eventStream.OnWrite += tracker.TransferredProgress;
            return eventStream;
        }

        /// <summary>
        /// Calls a specific EventHandler in a background thread
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="args"></param>
        /// <param name="sender"></param>
        internal static void InvokeInBackground<T>(EventHandler<T> handler, T args, object sender) where T : EventArgs
        {
            if (handler == null) return;

            var list = handler.GetInvocationList();
            foreach (var call in list)
            {
                var eventHandler = ((EventHandler<T>)call);
                if (eventHandler != null)
                {
                    // TODO: BackgroundInvoker
                    eventHandler(sender, args);
                }
            }
        }
    }
}
