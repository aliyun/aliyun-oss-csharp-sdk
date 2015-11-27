/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.Xml.Serialization;

namespace Aliyun.OSS
{
    /// <summary>
    /// Referer反盗链相关配置。
    /// </summary>
  [XmlRoot("RefererConfiguration")]
  public  class RefererConfiguration
  {
        /// <summary>
        /// 指定是否允许referer字段为空的请求访问。
        /// </summary>
        [XmlElement("AllowEmptyReferer")]
        public bool AllowEmptyReferer { get; set; }

        /// <summary>
        /// 保存referer访问白名单列表。
        /// </summary>
        [XmlElement("RefererList")]
        public RefererListModel RefererList { get; set; }

        internal RefererConfiguration()
        { }

        /// <summary>
        /// referer列表的配置
        /// </summary>
        [XmlRoot("RefererList")]
        public class RefererListModel 
        {
            /// <summary>
            /// referer列表
            /// </summary>
            [XmlElement("Referer")]
            public string[] Referers { get; set; }
        }
}

}
