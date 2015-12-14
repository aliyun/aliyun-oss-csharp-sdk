/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 表示追加<see cref="OssObject" />时的请求结果。
    /// </summary>
    public class AppendObjectResult
    {
        /// <summary>
        /// 获取一个值表示与Object相关的hex编码的128位MD5摘要。
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// 指明下一次请求应当提供的position。
        /// </summary>
        public long NextAppendPosition { get; internal set; }

        /// <summary>
        /// 表明Object的64位CRC值。该64位CRC根据ECMA-182标准计算得出。
        /// </summary>
        public ulong HashCrc64Ecma { get; internal set; }

        internal AppendObjectResult()
        { }
    }
}
