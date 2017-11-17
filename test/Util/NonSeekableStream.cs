using System;
using System.IO;
namespace Aliyun.OSS.Test.Util
{
    public class NonSeekableStream : MemoryStream
    {
        private MemoryStream baseStream;

        public NonSeekableStream()
        {
        }

        public NonSeekableStream(MemoryStream baseStream)
        {
            this.baseStream = baseStream;
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return baseStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            baseStream.Flush();
        }
    }
}
