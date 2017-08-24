/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Runtime.Serialization;

namespace Aliyun.OSS.Transform
{
    [Serializable]
    internal class RequestSerializationException : InvalidOperationException, ISerializable
    {
        public RequestSerializationException()
        { }

        public RequestSerializationException(string message) : base(message)
        { }

        public RequestSerializationException(string message, Exception innerException) 
            : base(message, innerException)
        { }

        protected RequestSerializationException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}
