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
    /// <para>
    /// The exception returned from OSS server side.
    /// </para>
    /// <para>  
    /// <see cref="ServiceException" />is used for wrap the error messages from OSS server side.
    /// For example, if Access key Id does not exist, it will throw <see cref="ServiceException" />.
    /// The exception has the error codes for caller to handle.
    /// </para>
    /// <para>
    /// <see cref="System.Net.WebException" /> means there's network issue when OSS client sends request to OSS server.
    /// For example, if the network is not available, it will throw <see cref="System.Net.WebException" />.
    /// </para>
    /// <para>
    /// <see cref="InvalidOperationException" /> means the client code handnle parse or handle the response. In this case it might means the response is incomplete or the SDK 
    /// does not match the OSS's response, in which case the SDK needs the upgrade.
    /// </para>
    /// Generally speaking, caller only needs to handle <see cref="ServiceException" />. It means the request has been processed by OSS (so network is not an issue),
    /// but the request could not be processed by OSS correctly. The error code of ServiceException could help to understand the issue and the caller could handle it properly.
    /// <para>
    /// 
    /// </para>
    /// </summary>
    [Serializable]
    public class ServiceException : Exception
    {
        /// <summary>
        /// The error code getter
        /// </summary>
        public virtual string ErrorCode { get; internal set; }

        /// <summary>
        /// The requestId getter
        /// </summary>
        public virtual string RequestId { get; internal set; }

        /// <summary>
        /// Host ID getter
        /// </summary>
        public virtual string HostId { get; internal set; }
        
        /// <summary>
        /// Creates a <see cref="ServiceException"/> instance.
        /// </summary>
        public ServiceException()
        { }

        /// <summary>
        /// Creates a new <see cref="ServiceException"/> instance.
        /// </summary>
        /// <param name="message">The error messag</param>
        public ServiceException(string message)
            : base(message)
        { }

        /// <summary>
        /// Creates a new <see cref="ServiceException"/>instance.
        /// </summary>
        /// <param name="message">Error messag</param>
        /// <param name="innerException">internal exception</param>
        public ServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Creates a new <see cref="ServiceException"/> instance.
        /// </summary>
        /// <param name="info">serialization information</param>
        /// <param name="context">context information</param>
        protected ServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// Overrides <see cref="ISerializable.GetObjectData"/> method
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/>serialization information instance</param>
        /// <param name="context"><see cref="StreamingContext"/>context information</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
