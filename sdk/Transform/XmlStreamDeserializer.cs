/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Aliyun.OSS.Transform
{
    /// <summary>
    /// Deserialize an object of type T from XML stream.
    /// </summary>
    internal class XmlStreamDeserializer<T> : IDeserializer<Stream, T>
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(T));

        /// <summary>
        /// Deserialize an object of type T, then close the underlying stream.
        /// </summary>
        public T Deserialize(Stream xmlStream)
        {
            using (xmlStream)
            {
                try
                {
                    return (T)Serializer.Deserialize(xmlStream);
                }
                catch (XmlException ex)
                {
                    throw new ResponseDeserializationException(ex.Message, ex);
                }
                catch (InvalidOperationException ex)
                {
                    throw new ResponseDeserializationException(ex.Message, ex);
                }
            }
        }
    }
}
