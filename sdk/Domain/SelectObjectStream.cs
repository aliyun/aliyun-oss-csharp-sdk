using System;
using System.IO;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Internal
{
    class SelectObjectStream : WrapperStream
    {
        public int StatusCode { get; private set; }

        public string ErrorMessage { get; private set; }

        private int _payloadRemain;

        private uint _payloadcrc32;

        private int _lastFrameType;

        private bool _gotEndFrame;

        private bool _gotEndofStream;

        public SelectObjectStream(Stream baseStream) :
            base(baseStream)
        {
            _payloadRemain = 0;
            _payloadcrc32 = 0;
            _lastFrameType = 0;
            _gotEndFrame = false;
            _gotEndofStream = false;
            CRC32.Init();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int remain = count;
            while (remain > 0)
            {
                DepackFrameHeaderIfNesseary();
                int len = Math.Min(_payloadRemain, remain);
                int got = TryRead(buffer, offset, len);
                _payloadcrc32 = CRC32.GetCRC32Byte(buffer, offset, got, _payloadcrc32);

                offset += got;
                remain -= got;
                _payloadRemain -= got;

                if (_gotEndFrame || _gotEndofStream)
                    break;
            }

            return count - remain;
        }

        public void DepackFrameHeaderIfNesseary()
        {
            var buff = new byte[128];
            int readCnt = 0;

            //Version | Frame - Type | Payload Length | Header Checksum | Payload | Payload Checksum
            //<1 bytes> <--3 bytes-->   <-- 4 bytes --> <------4 bytes--> <variable><----4bytes------>
            //Payload 
            //<offset | data>
            //<8 bytes><variable>

            if (_payloadRemain > 0)
                return;

            if (_lastFrameType == 0x800001)
            {
                //Payload Checksum                
                readCnt = TryRead(buff, 0, 4);
                ensureReadSize(4, readCnt);
                uint serverCrc32 = OssUtils.ConvertBytesToUint(buff, 0, 4);

                //Payload Crc32 check  
                if (serverCrc32 != 0 && serverCrc32 != _payloadcrc32)
                {
                    throw new ClientException(string.Format("Payload checksum check fail server CRC {0}, client CRC {1}",
                        serverCrc32, _payloadcrc32));
                }
            }

            _lastFrameType = 0;
            _payloadRemain = 0;

            readCnt = TryRead(buff, 0, 12);

            if (_gotEndofStream)
                return;

            ensureReadSize(12, readCnt);

            //version

            //Frame - Type
            int frameType = OssUtils.ConvertBytesToInt(buff, 1, 3);
            _lastFrameType = frameType;

            //Payload Length
            int payloadLength = OssUtils.ConvertBytesToInt(buff, 4, 4);

            //Payload Length
            int headerCheckSum = OssUtils.ConvertBytesToInt(buff, 8, 4);

            switch (frameType)
            {
                case 0x800001:      //data frame
                    //offset
                    readCnt = TryRead(buff, 0, 8);
                    ensureReadSize(8, readCnt);
                    _payloadcrc32 = CRC32.GetCRC32Byte(buff, 8, 0);

                    //skip offset bytes
                    _payloadRemain = payloadLength - 8;
                    break;
                case 0x800005:      //Select object End Frame
                    //offset | total scanned bytes
                    readCnt = TryRead(buff, 0, 16);
                    ensureReadSize(16, readCnt);

                    //http status
                    readCnt = TryRead(buff, 0, 4);
                    ensureReadSize(4, readCnt);
                    StatusCode = OssUtils.ConvertBytesToInt(buff, 0, 4);

                    //error message
                    payloadLength = payloadLength - 20;

                    if (payloadLength > 0)
                    {
                        int len = Math.Min(buff.Length, payloadLength);
                        readCnt = TryRead(buff, 0, len);
                        ensureReadSize(len, readCnt);
                        ErrorMessage = System.Text.Encoding.Default.GetString(buff, 0, readCnt);

                        payloadLength -= len;
                        if (payloadLength > 0)
                        {
                            readCnt = TrySkip(payloadLength);
                            ensureReadSize(payloadLength, readCnt);
                        }
                    }

                    //skip payload Checksum
                    readCnt = TryRead(buff, 0, 4);
                    ensureReadSize(4, readCnt);

                    _gotEndFrame = true;
                    break;
                case 0x800004:      //Continuous Frame
                default:
                    //skip playload
                    readCnt = TrySkip(payloadLength);
                    ensureReadSize(payloadLength, readCnt);

                    //skip payload Checksum
                    readCnt = TryRead(buff, 0, 4);
                    ensureReadSize(4, readCnt);
                    break;
            }
        }

        private int TryRead(byte[] buffer, int offset, int count)
        {
            int remain = count;
            while (remain > 0)
            {
                int read = BaseStream.Read(buffer, offset, remain);
                remain -= read;
                offset += read;

                if (read == 0)
                {
                    _gotEndofStream = true;
                    break;
                }
            }
            return count - remain;
        }

        private int TrySkip(int count)
        {
            byte[] buffer = new byte[256];
            int remain = count;
            while (remain > 0)
            {
                int len = Math.Min(buffer.Length, remain);
                int read = BaseStream.Read(buffer, 0, len);
                remain -= read;

                if (read == 0)
                {
                    _gotEndofStream = true;
                    break;
                }
            }
            return count - remain;
        }

        private void ensureReadSize(int expect, int got)
        {
            if (expect != got)
            {
                throw new ClientException(string.Format("Extract frame fail. expect {0}, but got {1}.", expect, got));
            }
        }
    }
}
