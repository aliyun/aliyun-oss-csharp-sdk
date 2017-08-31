/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Internal
{
    /// <summary>
    /// A wrapper stream.
    /// </summary>
    public class WrapperStream : Stream
    {
        /// <summary>
        /// Base stream.
        /// </summary>
        protected Stream BaseStream { get; private set; }

        /// <summary>
        /// Initializes WrapperStream with a base stream.
        /// </summary>
        /// <param name="baseStream"></param>
        public WrapperStream(Stream baseStream)
        {
            if (baseStream == null)
                throw new ArgumentNullException("baseStream");

            BaseStream = baseStream;
        }

        /// <summary>
        /// Returns the first base non-WrapperStream.
        /// </summary>
        /// <returns>First base stream that is non-WrapperStream.</returns>
        public Stream GetNonWrapperBaseStream()
        {
            Stream baseStream = this;
            do
            {
                var partialStream = baseStream as PartialWrapperStream;
                if (partialStream != null)
                    return partialStream;

                baseStream = (baseStream as WrapperStream).BaseStream;
            } while (baseStream is WrapperStream);
            return baseStream;
        }

        /// <summary>
        /// Returns the first base non-WrapperStream.
        /// </summary>
        /// <returns>First base stream that is non-WrapperStream.</returns>
        public Stream GetSeekableBaseStream()
        {
            Stream baseStream = this;
            do
            {
                if (baseStream.CanSeek)
                    return baseStream;

                baseStream = (baseStream as WrapperStream).BaseStream;
            } while (baseStream is WrapperStream);

            if (!baseStream.CanSeek)
                throw new InvalidOperationException("Unable to find seekable stream");

            return baseStream;
        }

        /// <summary>
        /// Returns the first base non-WrapperStream.
        /// </summary>
        /// <param name="stream">Potential WrapperStream</param>
        /// <returns>Base non-WrapperStream.</returns>
        public static Stream GetNonWrapperBaseStream(Stream stream)
        {
            WrapperStream wrapperStream = stream as WrapperStream;
            if (wrapperStream == null)
                return stream;
            return wrapperStream.GetNonWrapperBaseStream();
        }

        public Stream SearchWrappedStream(OssFunc<Stream, bool> condition)
        {
            Stream baseStream = this;
            do
            {
                if (condition(baseStream))
                    return baseStream;

                if (!(baseStream is WrapperStream))
                    return null;

                baseStream = (baseStream as WrapperStream).BaseStream;
            } while (baseStream != null);

            return baseStream;
        }

        public static Stream SearchWrappedStream(Stream stream, OssFunc<Stream, bool> condition)
        {
            WrapperStream wrapperStream = stream as WrapperStream;
            if (wrapperStream == null)
                return condition(stream) ? stream : null;
            return wrapperStream.SearchWrappedStream(condition);
        }

        #region Stream overrides

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// True if the stream supports reading; otherwise, false.
        /// </summary>
        public override bool CanRead
        {
            get { return BaseStream.CanRead; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// True if the stream supports seeking; otherwise, false.
        /// </summary>
        public override bool CanSeek
        {
            get { return BaseStream.CanSeek; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// True if the stream supports writing; otherwise, false.
        /// </summary>
        public override bool CanWrite
        {
            get { return BaseStream.CanWrite; }
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and
        /// file handles) associated with the current stream.
        /// </summary>
#if !PCL && !CORECLR
        public override void Close()
        {
            BaseStream.Close();
        }
#else
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            BaseStream.Dispose();
        }
#endif

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get { return BaseStream.Length; }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get
            {
                return BaseStream.Position;
            }
            set
            {
                BaseStream.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets a value, in miliseconds, that determines how long the stream
        /// will attempt to read before timing out.
        /// </summary>
        public override int ReadTimeout
        {
            get
            {
                return BaseStream.ReadTimeout;
            }
            set
            {
                BaseStream.ReadTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value, in miliseconds, that determines how long the stream
        /// will attempt to write before timing out.
        /// </summary>
        public override int WriteTimeout
        {
            get
            {
                return BaseStream.WriteTimeout;
            }
            set
            {
                BaseStream.WriteTimeout = value;
            }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written
        /// to the underlying device.
        /// </summary>
        public override void Flush()
        {
            BaseStream.Flush();
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position
        /// within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between offset and (offset + count - 1) replaced
        /// by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin storing the data read
        /// from the current stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the current stream.
        /// </param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the
        /// number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">
        /// A value of type System.IO.SeekOrigin indicating the reference point used
        /// to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current
        /// position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies count bytes from buffer to the current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin copying bytes to the
        /// current stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        #endregion
    }

}
