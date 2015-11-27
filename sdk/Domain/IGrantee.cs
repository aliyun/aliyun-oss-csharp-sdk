/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

namespace Aliyun.OSS
{
    /// <summary>
    /// 表示被授权者的信息。
    /// </summary>
    public interface IGrantee
    {
        /// <summary>
        /// 获取或设置被授权者的标识。
        /// </summary>
        string Identifier { get; set; }
    }

}
