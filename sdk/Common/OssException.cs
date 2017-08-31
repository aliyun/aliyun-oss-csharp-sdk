/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Aliyun.OSS.Common
{
    /// <summary>
    /// This is the expected exception that is thrown when accessing OSS.
    /// </summary>
    /// <seealso cref="ServiceException" />
    [Serializable]
    public class OssException : ServiceException
    {
        /// <summary>
        /// Initializes a new <see cref="OssException"/> instance
        /// </summary>
        public OssException()
        { }

        /// <summary>
        /// Initializes a new <see cref="OssException"/>instance
        /// </summary>
        /// <param name="message">Error message for the exception</param>
        public OssException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a <see cref="OssException"/> instance
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">The context information</param>
        protected OssException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        
        /// <summary>
        /// Initializes a new <see cref="OssException"/> instance
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Inner exceptions</param>
        public OssException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Override the<see cref="ISerializable.GetObjectData"/>methods
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/>，Serialization information about the object</param>
        /// <param name="context"><see cref="StreamingContext"/> Context information</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
