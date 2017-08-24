/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Diagnostics;
using System.Net;

namespace Aliyun.OSS.Common.Communication
{
    internal abstract class ServiceResponse : ServiceMessage, IDisposable
    {
        public abstract HttpStatusCode StatusCode { get; }
        
        public abstract Exception Failure { get; }
        
        public virtual bool IsSuccessful()
        {
            return (int)StatusCode / 100 == (int)HttpStatusCode.OK / 100;
        }
        
        /// <summary>
        /// Throws the exception from communication if the status code is not 2xx.
        /// </summary>
        public virtual void EnsureSuccessful()
        {
            if (!IsSuccessful())
            {
                // Disposing the content should help users: If users call EnsureSuccessStatusCode(), an exception is
                // thrown if the response status code is != 2xx. I.e. the behavior is similar to a failed request (e.g.
                // connection failure). Users don't expect to dispose the content in this case: If an exception is
                // thrown, the object is responsible fore cleaning up its state.
                if (Content != null)
                {
                    Content.Dispose();
                }

                Debug.Assert(Failure != null);
                throw Failure;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
