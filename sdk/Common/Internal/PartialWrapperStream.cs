/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Aliyun.OSS.Common.Internal
{
    /// <summary>
    /// This class is used to wrap a stream for a particular segment of a stream.  It 
    /// makes that segment look like you are reading from beginning to end of the stream.
    /// </summary>
    public class PartialWrapperStream : WrapperStream
    {
        private long initialPosition;
        private long partSize;

        public PartialWrapperStream(Stream stream, long partSize)
            : base(stream)
        {
            if (!stream.CanSeek)
                throw new InvalidOperationException("Base stream of PartialWrapperStream must be seekable");

            this.initialPosition = stream.Position;
            long remainingData = stream.Length - stream.Position;
            if (partSize == 0 || remainingData < partSize)
            {
                this.partSize = remainingData;
            }
            else
            {
                this.partSize = partSize;
            }
        }

        private long RemainingPartSize
        {
            get
            {
                long remaining = this.partSize - this.Position;
                return remaining;
            }
        }

        #region Stream overrides

        public override long Length
        {
            get
            {
                long length = base.Length - this.initialPosition;
                if (length > this.partSize)
                {
                    length = this.partSize;
                }
                return length;
            }
        }

        public override long Position
        {
            get
            {
                return base.Position - this.initialPosition;
            }
            set
            {
                base.Position = this.initialPosition + value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesToRead = count < this.RemainingPartSize ? count : (int)this.RemainingPartSize;
            if (bytesToRead < 0)
                return 0;
            return base.Read(buffer, offset, bytesToRead);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long position = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = this.initialPosition + offset;
                    break;
                case SeekOrigin.Current:
                    position = base.Position + offset;
                    break;
                case SeekOrigin.End:
                    position = base.Position + this.partSize + offset;
                    break;
            }

            if (position < this.initialPosition)
            {
                position = this.initialPosition;
            }
            else if (position > this.initialPosition + this.partSize)
            {
                position = this.initialPosition + this.partSize;
            }

            base.Position = position;

            return this.Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }

#if !PCL && !UNITY && !CORECLR
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, Object state)
        {
            throw new NotSupportedException();
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, Object state)
        {
            throw new NotSupportedException();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }
#endif

        #endregion
    }
}
