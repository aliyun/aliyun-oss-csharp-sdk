using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Common.Internal
{
    class SelectObjectStream : WrapperStream 
    {
        protected int PayloadRemain { get; private set; }

        protected uint Payloadcrc32 { get; private set; }

        protected int PayloadLength { get; private set; }

        protected bool bEndFrame { get; private set; }

        public SelectObjectStream(Stream baseStream)
        : base(baseStream)
        {
            PayloadRemain = 0;
            Payloadcrc32 = 0;
            PayloadLength = 0;
            bEndFrame = false;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int remain = count;
            int result = 0;
            while (remain > 0 && !bEndFrame)
            {
                int temp = DepackFrameAndReadPayload(buffer, offset, remain);

                result += temp;
                remain -= temp;
                offset += temp;
            }
            return result;
        }

        public void PayloadChecksum(byte[] buffer,int start, int len)
        {
            //Payload Checksum                
            byte[] PayloadChecksum = new byte[4];
            TryRead(PayloadChecksum, 0, 4);
            uint serverCrc32 = bytes4ToUInt(PayloadChecksum);

            // Crc32                    
            uint clientCrc32 = CRC32.GetCRC32Byte(buffer, start,len, Payloadcrc32);

            if (serverCrc32 != clientCrc32)
            {
                throw new System.IO.IOException("Payload Checksum check fail !");
            }
        }
        public int DepackFrameAndReadPayload(byte[] buffer, int offset, int count)
        {
            //Version | Frame - Type | Payload Length | Header Checksum | Payload | Payload Checksum
            //<1 bytes> <--3 bytes-->   <-- 4 bytes --> <------4 bytes--> <variable><----4bytes------>
            //Payload 
            //<offset | data>
            //<8 bytes><variable>

            if (PayloadRemain > 0)
            {
                int readLength = (int)Math.Min(PayloadRemain, count);
                int Read = TryRead(buffer, offset, readLength);

                PayloadRemain -= Read;
                if (PayloadRemain == 0)
                {
                    PayloadChecksum(buffer, 0, PayloadLength);
                }
                return Read;
            }

            byte[] Version = new byte[1];
            TryRead(Version, 0, 1);

            byte[] FrameType = new byte[3];
            TryRead(FrameType, 0, 3);
            int frameType = bytes3ToInt(FrameType);

            byte[] payloadLength = new byte[4];
            TryRead(payloadLength, 0, 4);
            PayloadLength = bytes4ToInt(payloadLength);

            byte[] HeaderChecksum = new byte[4];
            TryRead(HeaderChecksum, 0, 4);

            int totalRead = 0;

            switch (frameType)
            {
                case 0x800001:      //data frame
                    byte[] Payloadoffset = new byte[8];
                    TryRead(Payloadoffset, 0, 8);
                    Payloadcrc32 = CRC32.GetCRC32Byte(Payloadoffset, 0);

                    //delete offset bytes
                    PayloadLength -= 8;

                    int readLength = (int)Math.Min(PayloadLength, count);
                    totalRead = TryRead(buffer, offset, readLength);
                    if (totalRead < PayloadLength)
                    {
                        PayloadRemain = PayloadLength - totalRead;
                        return totalRead;
                    }

                    // Crc32 check                   
                    PayloadChecksum(buffer, offset, PayloadLength);
                    break;

                case 0x800004:      //Continuous Frame
                    break;

                case 0x800005:      //Select object End Frame
                    bEndFrame = true;
                    break;

                default:
                    break;
            }
            return totalRead;
        }

        private int TryRead(byte[] buffer, int offset, int count)
        {
            int read = 0;
            int maxRead = 10;
            int remainReadCount = count;

            while (remainReadCount > 0 && maxRead-- > 0 & base.CanRead)
            {
                read = BaseStream.Read(buffer, offset, remainReadCount);
                remainReadCount -= read;
                offset += read;
            }
            if (remainReadCount > 0)
            {
                throw new System.IO.IOException("input stream is end unexpectly !");
            }
            return count - remainReadCount;
        }

        private int bytes3ToInt(byte[] data)
        {
            return ((data[0] & 0xFF) << 16)
                | ((data[1] & 0xFF) << 8) | (data[2] & 0xFF);
        }

        private uint bytes4ToUInt(byte[] data)
        {
            return (uint)(((data[0] & 0xFF) << 24) | ((data[1] & 0xFF) << 16)
                | ((data[2] & 0xFF) << 8) | (data[3] & 0xFF));
        }

        private int bytes4ToInt(byte[] data)
        {
            return (((data[0] & 0xFF) << 24) | ((data[1] & 0xFF) << 16)
                | ((data[2] & 0xFF) << 8) | (data[3] & 0xFF));
        }
    }
}
