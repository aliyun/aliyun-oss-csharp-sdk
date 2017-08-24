/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Text;
using System.Collections.Generic;

namespace Aliyun.OSS.Util
{
    /// <summary>
    /// Callback body's format type. The OSS will issue a post request to the callback url with the data specified in the request's callbackbody header.
    /// <para>
    /// OSS does not validate the data sent to callback url.
    /// </para>
    /// </summary>
    public enum CallbackBodyType
    {
        /// <summary>
        /// Url encoded. 
        /// </summary>
        [StringValue("application/x-www-form-urlencoded")]
        Url = 0,

        /// <summary>
        /// Json encoded 
        /// </summary>
        [StringValue("application/json")]
        Json
    }

    /// <summary>
    /// The callback header's builder
    /// </summary>
    public class CallbackHeaderBuilder
    {
        /// <summary>
        /// Gets or sets the callback url such as “http://callback.oss.demo.com:9000”
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// Gets or sets the callback host.By default it's <see cref="CallbackHeaderBuilder.CallbackUrl"/>
        /// </summary>
        public string CallbackHost { get; set; }

        /// <summary>
        /// Gets or sets the callback body.
        /// </summary>
        public string CallbackBody { get; set; }

        /// <summary>
        /// Gets or sets the callback body type.
        /// </summary>
        public CallbackBodyType CallbackBodyType { get; set; } 

        /// <summary>
        /// Creates a new instance of <see cref="CallbackHeaderBuilder" />
        /// </summary>
        /// <param name="callbackUrl">callback url</param>
        /// <param name="callbackBody">callback body</param>
        public CallbackHeaderBuilder(string callbackUrl, string callbackBody) 
        {
            CallbackUrl = callbackUrl;
            CallbackHost = null;
            CallbackBody = callbackBody;
            CallbackBodyType = CallbackBodyType.Url;
        }

        /// <summary>
        /// Creates a new instance of <see cref="CallbackHeaderBuilder" />
        /// </summary>
        /// <param name="callbackUrl">callback url</param>
        /// <param name="callbackHost"> callback host</param>
        /// <param name="callbackBody">callback body</param>
        /// <param name="callbackBodyType">callback body type</param>
        public CallbackHeaderBuilder(string callbackUrl, string callbackHost, string callbackBody, CallbackBodyType callbackBodyType)
        {
            CallbackUrl = callbackUrl;
            CallbackHost = callbackHost;
            CallbackBody = callbackBody;
            CallbackBodyType = callbackBodyType;
        }

        /// <summary>
        /// Builds the callback header.
        /// </summary>
        /// <returns>the callback header</returns>
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
    /// The callback variable header builder.
    /// </summary>
    public class CallbackVariableHeaderBuilder
    {
        private readonly IDictionary<string, string> _callbackVariable = new Dictionary<string, string>();

        /// <summary>
        /// Gets the callback variable dictionary.
        /// </summary>
        /// <remarks>
        /// The custom parameter's key must start with "x:" and be in lowercase.
        /// </remarks>
        public IDictionary<string, string> CallbackVariable
        {
            get { return _callbackVariable; }
        }

        /// <summary>
        /// Adds the callback variable
        /// </summary>
        /// <param name="key">the custom variable, must start with "x:"</param>
        /// <param name="value">the value of the custom variable.</param>
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
        /// Builds the callback variables' header value
        /// </summary>
        /// <returns>The callback variables' header value</returns>
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
