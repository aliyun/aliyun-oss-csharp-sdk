/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Aliyun.OSS
{
    public class StreamTransferProgressArgs : EventArgs
    {
        private long _incrementTransferred;
        private long _total;
        private long _transferred;

        /// <summary>
        /// The constructor takes the number of
        /// currently transferred bytes and the
        /// total number of bytes to be transferred
        /// </summary>
        /// <param name="incrementTransferred">The number of bytes transferred since last event</param>
        /// <param name="transferred">The number of bytes transferred</param>
        /// <param name="total">The total number of bytes to be transferred</param>
        public StreamTransferProgressArgs(long incrementTransferred, long transferred, long total)
        {
            this._incrementTransferred = incrementTransferred;
            this._transferred = transferred;
            this._total = total;
        }

        /// <summary>
        /// Gets the percentage of transfer completed
        /// </summary>
        public int PercentDone
        {
            get { return (int)((_transferred * 100) / _total); }
        }

        /// <summary>
        /// Gets the number of bytes transferred since last event
        /// </summary>
        public long IncrementTransferred
        {
            get { return this._incrementTransferred; }
        }

        /// <summary>
        /// Gets the number of bytes transferred
        /// </summary>
        public long TransferredBytes
        {
            get { return _transferred; }
        }

        /// <summary>
        /// Gets the total number of bytes to be transferred
        /// </summary>
        public long TotalBytes
        {
            get { return _total; }
        }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Concat(
                "Transfer Statistics. Percentage completed: ", PercentDone,
                ", Bytes transferred: ", _transferred,
                ", Total bytes to transfer: ", _total
                );
        }    
    }
}
