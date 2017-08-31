/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Aliyun.OSS.Model
{
    /// <summary>
    /// Base class for responses that return a stream.
    /// </summary>
    public abstract class StreamResult : GenericResult, IDisposable
    {
        private bool disposed;
        private Stream responseStream;

        #region Dispose Pattern

        /// <summary>
        /// Disposes of all managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            if (!this.disposed)
            {
                GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Remove Unmanaged Resources
                    // I.O.W. remove resources that have to be explicitly
                    // "Dispose"d or Closed. For an OSS Response, these are:
                    // 1. The Response Stream for GET Object requests
                    // 2. The HttpResponse object for GET Object requests
                    if (responseStream != null)
                    {
                        responseStream.Dispose();
                    }
                }

                responseStream = null;
                disposed = true;
            }
        }

        #endregion

        /// <summary>
        /// An open stream read from to get the data from OSS. In order to
        /// use this stream without leaking the underlying resource, please
        /// wrap access to the stream within a using block.
        /// </summary>
        public Stream ResponseStream
        {
            get { return this.responseStream; }
            set { this.responseStream = value; }
        }

        /// <summary>
        /// Check to see if Body property is set
        /// </summary>
        public bool IsSetResponseStream()
        {
            return this.responseStream != null;
        }
    }
}
