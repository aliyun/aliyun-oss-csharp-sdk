/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Text;
using System.Collections.Generic;

namespace Aliyun.OSS.Util
{
    /// <summary>
    /// 回调请求消息体的格式类型。
    /// <para>
    /// OSS发给回调服务器请求的消息体类型，OSS按照该类型替换系统变量、自定义变量。
    /// OSS不校验消息体类型格式的正确性。
    /// </para>
    /// </summary>
    public enum CallbackBodyType
    {
        /// <summary>
        /// 私有权限，有权限才可以读和写
        /// </summary>
        [StringValue("application/x-www-form-urlencoded")]
        Url = 0,

        /// <summary>
        /// 默认权限，仅用于文件，与存储空间的权限相同
        /// </summary>
        [StringValue("application/json")]
        Json
    }

    /// <summary>
    /// 构建上传回调请求头
    /// </summary>
    public class CallbackHeaderBuilder
    {
        /// <summary>
        /// 回调服务器地址，必选参数，如“http://callback.oss.demo.com:9000”
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// 回调请求（OSS发给回调服务器的请求）消息头Host的值，可选参数，默认值<see cref="CallbackHeaderBuilder.CallbackUrl"/>
        /// </summary>
        public string CallbackHost { get; set; }

        /// <summary>
        /// 回调请求（OSS发给回调服务器的请求）的消息体，必选参数
        /// </summary>
        public string CallbackBody { get; set; }

        /// <summary>
        /// 回调请求（OSS发给回调服务器的请求）消息体的格式类型，可选参数，默认值<see cref="CallbackBodyType"/>的Url
        /// </summary>
        public CallbackBodyType CallbackBodyType { get; set; } 

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callbackUrl">回调服务器地址。</param>
        /// <param name="callbackBody">回调请求消息体。</param>
        public CallbackHeaderBuilder(string callbackUrl, string callbackBody) 
        {
            CallbackUrl = callbackUrl;
            CallbackHost = null;
            CallbackBody = callbackBody;
            CallbackBodyType = CallbackBodyType.Url;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callbackUrl">回调服务器地址。</param>
        /// <param name="callbackHost"> 回调请求消息头中Host的值。</param>
        /// <param name="callbackBody">回调请求消息体。</param>
        /// <param name="callbackBodyType">回调请求消息体的格式类型。</param>
        public CallbackHeaderBuilder(string callbackUrl, string callbackHost, string callbackBody, CallbackBodyType callbackBodyType)
        {
            CallbackUrl = callbackUrl;
            CallbackHost = callbackHost;
            CallbackBody = callbackBody;
            CallbackBodyType = callbackBodyType;
        }

        /// <summary>
        /// 构建上传回调请求头
        /// </summary>
        /// <returns>上传回调请求头</returns>
        public string Build()
        {
            if (CallbackUrl == null || CallbackBody == null)
            {
                throw new ArgumentException("Callback argument invalid");
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            stringBuilder.Append(String.Format("\"callbackUrl\":\"{0}\"", CallbackUrl));
            if (CallbackHost != null)
            {
                stringBuilder.Append(String.Format(",\"callbackHost\":\"{0}\"", CallbackHost));
            }
            stringBuilder.Append(String.Format(",\"callbackBody\":\"{0}\"", CallbackBody));
            if (CallbackBodyType == CallbackBodyType.Json)
            {
                stringBuilder.Append(String.Format(",\"callbackBodyType\":\"application/json\""));
            }
            stringBuilder.Append("}");

            return Convert.ToBase64String(Encoding.Default.GetBytes(stringBuilder.ToString()));
        }
    }

    /// <summary>
    /// 构建上传回调自定义参数请求头
    /// </summary>
    public class CallbackVariableHeaderBuilder
    {
        private readonly IDictionary<string, string> _callbackVariable = new Dictionary<string, string>();

        /// <summary>
        /// 获取上传回调自定义参数
        /// </summary>
        /// <remarks>
        /// 用户自定义参数的Key一定要以“x:”开头，且必须为小写。
        /// </remarks>
        public IDictionary<string, string> CallbackVariable
        {
            get { return _callbackVariable; }
        }

        /// <summary>
        /// 增加上传回调自定义参数
        /// </summary>
        /// <param name="key">自定义变量的名称，必须“x:”开头</param>
        /// <param name="value">自定义变量的值</param>
        public CallbackVariableHeaderBuilder AddCallbackVariable(string key, string value)
        {
            if (!key.StartsWith("x:"))
            {
                throw new ArgumentException("Callback variable key invalid");
            }

            _callbackVariable.Add(key, value);

            return this;
        }

        /// <summary>
        /// 构建上传回调自定义参数请求头
        /// </summary>
        /// <returns>上传回调请求头</returns>
        public string Build()
        {
            if (_callbackVariable.Count == 0)
            {
                return "";
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach (var entry in _callbackVariable)
            {
                if (stringBuilder.Length > 1)
                {
                    stringBuilder.Append(",");
                }
                stringBuilder.Append(String.Format("\"{0}\":\"{1}\"", entry.Key, entry.Value));
            }
            stringBuilder.Append("}");

            return Convert.ToBase64String(Encoding.Default.GetBytes(stringBuilder.ToString()));
        }
    }
}
