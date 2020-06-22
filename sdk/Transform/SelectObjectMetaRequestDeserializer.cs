/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Util;
using System.IO;

namespace Aliyun.OSS.Transform
{
    internal class SelectObjectMetaRequestDeserializer : ResponseDeserializer<CreateSelectObjectMetaResult, CreateSelectObjectMetaResult>
    {
        private readonly CreateSelectObjectMetaRequest _createSelectObjectMetaRequest;

        public SelectObjectMetaRequestDeserializer(CreateSelectObjectMetaRequest createSelectObjectMetaRequest)
            : base(null)
        {
            _createSelectObjectMetaRequest = createSelectObjectMetaRequest;
        }

        public override CreateSelectObjectMetaResult Deserialize(ServiceResponse xmlStream)
        {
            var result = new CreateSelectObjectMetaResult();

            //Version | Frame - Type | Payload Length | Header Checksum | Payload | Payload Checksum
            //<1 bytes> <--3 bytes-->   <-- 4 bytes --> <------4 bytes--> <variable><----4bytes------>
            //Payload 
            //<offset | data>
            //<8 bytes><variable>
            byte[] Version = new byte[1];
            tryRead(xmlStream.Content, Version, 0, 1);

            byte[] FrameType = new byte[3];
            tryRead(xmlStream.Content, FrameType, 0, 3);
            int frameType = bytes3ToInt(FrameType);

            byte[] PayloadLength = new byte[4];
            tryRead(xmlStream.Content, PayloadLength, 0, 4);
            long payloadLength = bytes4ToInt(PayloadLength);

            byte[] HeaderChecksum = new byte[4];
            tryRead(xmlStream.Content, HeaderChecksum, 0, 4);

            switch (frameType)
            {
                case 0x800006:      //Meta End Frame(Csv)
                case 0x800007:      //Meta End Frame(Json)
                    uint payloadCrc32 = 0; 
                    payloadLength -= 32;
                    // 32 bytes
                    byte[] csvbuffer = new byte[32];
                    tryRead(xmlStream.Content, csvbuffer, 0, 32);
                    payloadCrc32 = CRC32.GetCRC32Byte(csvbuffer, payloadCrc32);

                    // offset 8 bytes
                    byte[] Payloadoffset = BitConverter.GetBytes(BitConverter.ToUInt64(csvbuffer, 0));
                    result.Offset = bytes8ToUlong(Payloadoffset);

                    // total scaned 8bytes
                    byte[] totalscaned = BitConverter.GetBytes(BitConverter.ToUInt64(csvbuffer, 8));
                    result.TotalScannedbytes = bytes8ToUlong(totalscaned);

                    // status 4 bytes
                    byte[] status = BitConverter.GetBytes(BitConverter.ToUInt32(csvbuffer, 16));
                    result.Status = bytes4ToInt(status);

                    // splitsCount 4 bytes
                    byte[] splitsCount = BitConverter.GetBytes(BitConverter.ToUInt32(csvbuffer, 20));
                    result.SplitsCount = bytes4ToInt(splitsCount);

                    // rowsCount 8 bytes
                    byte[] rowsCount = BitConverter.GetBytes(BitConverter.ToUInt64(csvbuffer, 24));
                    result.RowsCount = bytes8ToUlong(rowsCount);
                    // 32 bytes
                    
                    if (frameType == 0x800006)
                    {
                        payloadLength -= 4;

                        // colsCount 4 bytes
                        byte[] colsCount = new byte[4];
                        tryRead(xmlStream.Content, colsCount, 0, 4);
                        payloadCrc32 = CRC32.GetCRC32Byte(colsCount, payloadCrc32);
                        result.ColsCount = bytes4ToInt(colsCount);
                    }

                    var payload = new MemoryStream();
                    int totalRead = 0;
                    byte[] buffer = new byte[1024];
                    while (payloadLength > totalRead)
                    {
                        int readLength = (int)Math.Min(payloadLength - totalRead, 1024);
                        int readBytes = tryRead(xmlStream.Content, buffer, 0, readLength);
                        payload.Write(buffer, 0, readBytes);
                        totalRead += readBytes;
                    }

                    result.ErrorMessage = readToString(payload);

                    uint clientCrc32 = CRC32.GetCRC32Str(result.ErrorMessage, payloadCrc32);


                    //Payload Checksum                
                    byte[] PayloadChecksum = new byte[4];
                    tryRead(xmlStream.Content, PayloadChecksum, 0, 4);
                    uint serverCrc32 = bytes4ToInt(PayloadChecksum);

                    //Payload Crc32 check  
                    if (serverCrc32 != clientCrc32)
                    {
                        throw new System.IO.IOException("Payload Checksum check fail !");
                    }

                    break;
                default:
                    break;
            }

            DeserializeGeneric(xmlStream, result);
            return result;
        }

        private int tryRead(System.IO.Stream inputStream, byte[] buffer, int offset, int count)
        {
            int read = 0;
            int maxRead = 10;
            int remainReadCount = count;

            while (remainReadCount > 0 && maxRead-- > 0 & inputStream.CanRead)
            {
                read = inputStream.Read(buffer, count - remainReadCount, remainReadCount);
                remainReadCount -= read;
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

        private uint bytes4ToInt(byte[] data)
        {
            return (uint)(((data[0] & 0xFF) << 24) | ((data[1] & 0xFF) << 16)
                | ((data[2] & 0xFF) << 8) | (data[3] & 0xFF));
        }

        private ulong bytes8ToUlong(byte[] data)
        {
            return (ulong)(((data[0] & 0xFF) << 56) | ((data[1] & 0xFF) << 48)
                | ((data[2] & 0xFF) << 40) | ((data[3] & 0xFF) << 32)
                |((data[4] & 0xFF) << 24) | ((data[5] & 0xFF) << 16)
                | ((data[6] & 0xFF) << 8) | (data[7] & 0xFF));
        }

        private string readToString(System.IO.Stream inputStream)
        {
            string content = null;
            inputStream.Position = 0;
            using (StreamReader reader = new StreamReader(inputStream))
            {
                content = reader.ReadToEnd();
            }
            return content;
        }
    }
}

