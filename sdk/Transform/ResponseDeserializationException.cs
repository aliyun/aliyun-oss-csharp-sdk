/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Runtime.Serialization;

namespace Aliyun.OSS.Transform
{
    [Serializable]
    internal class ResponseDeserializationException : InvalidOperationException, ISerializable
    {
        public ResponseDeserializationException()
        { }

        public ResponseDeserializationException(string message) : base(message)
        { }

        public ResponseDeserializationException(string message, Exception innerException) 
            : base(message, innerException)
        { }

        protected ResponseDeserializationException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}