/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

using Aliyun.OSS.Util;
using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    /// <summary>
    /// Description of DeleteObjectsResult.
    /// </summary>
    [XmlRoot("DeleteResult")]
    public class DeleteObjectsResult : GenericResult
    {
        private DeletedObject[] _keys;

        /// <summary>
        /// gets or sets deleted keys
        /// </summary>
        [XmlElement("Deleted")]
        public DeletedObject[] Keys 
        {
            get
            {
                if (EncodingType == null)
                    return _keys;

                bool isUrlEncoding = EncodingType.ToLowerInvariant().Equals(HttpUtils.UrlEncodingType);
                foreach (var key in _keys)
                {
                    key.Key = isUrlEncoding ? HttpUtils.DecodeUri(key.Key) : key.Key;
                }
                return _keys;
            }
            set
            {
                this._keys = value;
            }
        }

        /// <summary>
        /// gets or sets EncodingType
        /// </summary>
        [XmlElement("EncodingType")]
        public string EncodingType { get; set; }

        internal DeleteObjectsResult()
        {
        }

        /// <summary>
        /// Deleted object class. Key is its only property.
        /// </summary>
        [XmlRoot("Deleted")]
        public class DeletedObject
        {
            /// <summary>
            /// Gets or sets deleted key
            /// </summary>
            [XmlElement("Key")]
            public string Key { get; set; }
        }
    }
}
