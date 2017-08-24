/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS
{
    /// <summary>
    /// Referer Configuration
    /// </summary>
  [XmlRoot("RefererConfiguration")]
  public  class RefererConfiguration
  {
        /// <summary>
        /// Flag of allowing empty referer
        /// </summary>
        [XmlElement("AllowEmptyReferer")]
        public bool AllowEmptyReferer { get; set; }

        /// <summary>
        /// Gets or sets the referer list
        /// </summary>
        [XmlElement("RefererList")]
        public RefererListModel RefererList { get; set; }

        internal RefererConfiguration()
        { }

        /// <summary>
        /// referer list model
        /// </summary>
        [XmlRoot("RefererList")]
        public class RefererListModel 
        {
            /// <summary>
            /// referer list
            /// </summary>
            [XmlElement("Referer")]
            public string[] Referers { get; set; }
        }
}

}
