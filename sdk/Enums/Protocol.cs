/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using Aliyun.OSS.Util;

namespace Aliyun.OSS
{
    /// <summary>
    /// 表示请求OSS服务时采用的通信协议，默认值为HTTP。
    /// </summary>
    public enum Protocol
    {
        /// <summary>
        /// 超文本传输协议
        /// </summary>
        [StringValue("http")]
        Http = 0,

        /// <summary>
        /// 超文本安全传输协议
        /// </summary>
        [StringValue("https")]
        Https
    }
}
