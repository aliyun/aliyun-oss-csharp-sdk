/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Aliyun.OSS.Common
{
    /// <summary>
    /// Exception thrown by the SDK for errors that occur within the SDK.
    /// </summary>
#if !PCL && !CORECLR
    [Serializable]
#endif
    public class ClientException : Exception
    {
        public ClientException(string message) : base(message) { }

        public ClientException(string message, Exception innerException) : base(message, innerException) { }

#if !PCL && !CORECLR
        /// <summary>
        /// Constructs a new instance of the ClientException class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0). </exception>
        protected ClientException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
