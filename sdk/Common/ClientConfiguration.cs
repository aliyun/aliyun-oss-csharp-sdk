/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common
{
    /// <summary>
    /// The client configuration that specifies the network parameters.
    /// </summary>
    public class ClientConfiguration
    {
        private const string UserAgentPrefix = "aliyun-sdk-dotnet/";
        private static readonly string _userAgent = GetDefaultUserAgent();

        // A temporary solution for undesired abortion when putting/getting large objects,
        // asynchronous implementions of putobject/getobject in next version can be a better 
        // solution to solve such problems.  
        private int _connectionTimeout = -1;
        private int _maxErrorRetry = 3;
        private int _proxyPort = -1;
        private bool _isCname = false;
        private bool _enalbeMD5Check = false;
        private long _progressUpdateInterval = 1024 * 4;
        private long _directWriteStreamThreshold = 0;
        private long _maxPartCachingSize = 1024 * 1024 * 100;
        private int _preReadBufferCount = 8;
        private bool _useSingleThreadReadInResumableUpload = false;
        private bool _enableCrcCheck = true;

        /// <summary>
        /// Max Http connection connection count. By default it's 512.
        /// </summary>
        public const int ConnectionLimit = 512;

        private Protocol _protocol = Protocol.Http;

        /// <summary>
        /// User-Agent in requests to OSS
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
        }

        /// <summary>
        /// Proxy host
        /// </summary>
        public string ProxyHost { get; set; }

        /// <summary>
        /// Proxy port
        /// </summary>
        public int ProxyPort
        {
            get { return _proxyPort; }
            set { _proxyPort = value; }
        }

        /// <summary>
        /// Proxy user name
        /// </summary>
        public string ProxyUserName { get; set; }

        /// <summary>
        /// Proxy user password
        /// </summary>
        public string ProxyPassword { get; set; }

        /// <summary>
        /// The proxy user name's domain for authentication
        /// </summary>
        public string ProxyDomain { get; set; }

        /// <summary>
        /// Connection timeout in milliseconds
        /// </summary>
        public int ConnectionTimeout
        {
            get { return _connectionTimeout; }
            set { _connectionTimeout = value; }
        }

        /// <summary>
        /// Max error retry count
        /// </summary>
        public int MaxErrorRetry
        {
            get { return _maxErrorRetry; }
            set { _maxErrorRetry = value; }
        }

        /// <summary>
		/// Protocols used to access OSS (HTTP or HTTPS)
        /// </summary>
        public Protocol Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        /// <summary>
        /// If the endpoint is the CName.
        /// If it's CName, ListBuckets is not supported.
        /// </summary>
        public bool IsCname
        {
            get
            {
                return _isCname;
            }
            set
            {
                _isCname = value;
            }
        }

        /// <summary>
        /// The progress update interval in terms of data upload/download's delta in bytes. By default it's 4096 bytes.
        /// </summary>
        public long ProgressUpdateInterval
        {
            get { return _progressUpdateInterval; }
            set { _progressUpdateInterval = value; }
        }

        /// <summary>
        /// Flag of enabling MD5 checksum.
        /// When EnalbeMD5Check is set true, MD5 will be checked and CRC check will be skipped whenever MD5 is applicable.  
        /// </summary>
        public bool EnalbeMD5Check
        {
            get { return _enalbeMD5Check; }
            set { _enalbeMD5Check = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Aliyun.OSS.Common.ClientConfiguration"/> enable
        /// crc check.
        /// When EnalbeMD5Check is set true, MD5 will be checked and CRC check will be skipped whenever MD5 is applicable.  
        /// </summary>
        /// <value><c>true</c> if enable crc check; otherwise, <c>false</c>.</value>
        public bool EnableCrcCheck
        {
            get { return _enableCrcCheck; }
            set { _enableCrcCheck = value; }
        }

        /// <summary>
        /// <para>Sets the custom base time</para>
        /// <para>
        /// OSS's token validation logic depends on the time. It requires that there's no more than 15 min time difference between client and OSS server.
        /// This API calculates the difference between local time to epoch time. Later one other APIs use this difference to offset the local time before sending request to OSS. 
        /// </para>
        /// </summary>
        /// <param name="epochTicks">Custom Epoch ticks (in seconds)</param>
        public void SetCustomEpochTicks(long epochTicks)
        {
            var epochTime = new DateTime(1970, 1, 1);
            var timeSpan = DateTime.UtcNow.Subtract(epochTime);
            var localTicks = (long)timeSpan.TotalSeconds;
            TickOffset = epochTicks - localTicks;
        }

        /// <summary>
        /// Gets the difference between customized epoch time and local time, in seconds
        /// </summary>
        public long TickOffset { get; internal set; }

        /// <summary>
        /// Gets or sets the direct write stream threshold.
        /// The theshold is the file size threshold that when the uploading file size is more than this value, the HttpWebRequest will not use write buffer to save the memory.
        /// </summary>
        /// <value>The direct write stream threshold.</value>
        public long DirectWriteStreamThreshold
        {
            get
            {
                return _directWriteStreamThreshold;
            }

            set
            {
                _directWriteStreamThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the max part caching size
        /// In multipart upload (resumable upload), if the part size is no bigger than MaxPartCachingSize, it will cache the whole part data before sending the data out. 
        /// </summary>
        /// <value>The size of the max part caching.</value>
        public long MaxPartCachingSize
        {
            get
            {
                return _maxPartCachingSize;
            }

            set 
            {
                _maxPartCachingSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the pre read buffer count in resumable upload.
        /// The max value could be the same size of MaxResumableUploadThreads;
        /// </summary>
        /// <value>The pre read buffer count.</value>
        public int PreReadBufferCount
        {
            get
            {
                return _preReadBufferCount;
            }
            set
            {
                _preReadBufferCount = value;
            }
        }

        /// <summary>
        /// When uploading a file with resumable upload, the default behavior is to read the source file in multiple-threading.
        /// But in old HDD, single thread read may be faster. And when the read speed is the bottleneck, try to change this parameter to compare the result. 
        /// </summary>
        /// <value><c>true</c> if use single thread read in resumable upload; otherwise, <c>false</c>.</value>
        public bool UseSingleThreadReadInResumableUpload
        {
            get
            {
                return _useSingleThreadReadInResumableUpload;
            }
            set
            {
                _useSingleThreadReadInResumableUpload = value;
            }
        }

        /// <summary>
        /// Gets the default user agent
        /// </summary>
        private static string GetDefaultUserAgent()
        {
            return UserAgentPrefix +
                typeof(ClientConfiguration).Assembly.GetName().Version + "(" +
                OssUtils.DetermineOsVersion() + "/" + 
                Environment.OSVersion.Version + "/" + 
                OssUtils.DetermineSystemArchitecture() + ";" + 
                Environment.Version + ")";
        }
    }
}
