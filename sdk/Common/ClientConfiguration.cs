/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common
{
    /// <summary>
    /// 表示访问阿里云服务的配置信息。
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

        /// <summary>
        /// HttpWebRequest最大的并发连接数目。
        /// </summary>
        public const int ConnectionLimit = 512;

        private Protocol _protocol = Protocol.Http;

        /// <summary>
        /// 获取访问请求的User-Agent。
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
        }

        /// <summary>
        /// 获取或设置代理服务器的地址。
        /// </summary>
        public string ProxyHost { get; set; }

        /// <summary>
        /// 获取或设置代理服务器的端口。
        /// </summary>
        public int ProxyPort
        {
            get { return _proxyPort; }
            set { _proxyPort = value; }
        }

        /// <summary>
        /// 获取或设置用户名。
        /// </summary>
        public string ProxyUserName { get; set; }

        /// <summary>
        /// 获取或设置密码。
        /// </summary>
        public string ProxyPassword { get; set; }

        /// <summary>
        /// 获取或设置代理服务器授权用户所在的域。
        /// </summary>
        public string ProxyDomain { get; set; }

        /// <summary>
        /// 获取或设置连接超时时间，单位为毫秒。
        /// </summary>
        public int ConnectionTimeout
        {
            get { return _connectionTimeout; }
            set { _connectionTimeout = value; }
        }

        /// <summary>
        /// 获取或设置请求发生错误时最大的重试次数。
        /// </summary>
        public int MaxErrorRetry
        {
            get { return _maxErrorRetry; }
            set { _maxErrorRetry = value; }
        }

        /// <summary>
        /// 获取或设置请求OSS服务时采用的通信协议。
        /// </summary>
        public Protocol Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        /// <summary>
        /// 获取或设置请求的Endpoint是否是cname
        /// 如果是cname，不支持ListBuckets操作
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
        /// 获取或设置上传/下载回调更新间隔，单位byte，默认4096bytes。
        /// </summary>
        public long ProgressUpdateInterval
        {
            get { return _progressUpdateInterval; }
            set { _progressUpdateInterval = value; }
        }

        /// <summary>
        /// 是否开启MD5校验，默认开启
        /// </summary>
        public bool EnalbeMD5Check
        {
            get { return _enalbeMD5Check; }
            set { _enalbeMD5Check = value; }
        }

        /// <summary>
        /// <para>设置自定义基准时间。</para>
        /// <para>
        /// 由于OSS的token校验是时间相关的，可能会因为终端系统时间不准导致无法访问OSS服务。
        /// 通过该接口设置自定义Epoch秒数，SDK计算出本机当前时间与自定义时间的差值，之后的
        /// 每次请求时间均加上该差值，以达到时间校验的目的。
        /// </para>
        /// </summary>
        /// <param name="epochTicks">自定义Epoch秒数。</param>
        public void SetCustomEpochTicks(long epochTicks)
        {
            var epochTime = new DateTime(1970, 1, 1);
            var timeSpan = DateTime.UtcNow.Subtract(epochTime);
            var localTicks = (long)timeSpan.TotalSeconds;
            TickOffset = epochTicks - localTicks;
        }

        /// <summary>
        /// 获取自定义基准时间与本地时间的时差值，单位为秒。
        /// </summary>
        public long TickOffset { get; internal set; }

        /// <summary>
        /// 获取User-Agent信息。
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
