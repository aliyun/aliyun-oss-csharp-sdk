/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("Delete")]
    public class DeleteObjectsRequestModel
    {
        [XmlElement("Quiet")]
        public bool Quiet { get; set; }

        [XmlElement("Object")]
        public ObjectToDel[] Keys { get; set; }

        [XmlRoot("Object")]
        public class ObjectToDel
        {
            [XmlElement("Key")]
            public string Key { get; set; }
        }
    }
}
