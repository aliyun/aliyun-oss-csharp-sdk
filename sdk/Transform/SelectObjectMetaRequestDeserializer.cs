/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using Aliyun.OSS.Common;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Transform
{
    internal class SelectObjectMetaRequestDeserializer : ResponseDeserializer<CreateSelectObjectMetaResult, CreateSelectObjectMetaResult>
    {
        private bool _gotEndofStream;
        public SelectObjectMetaRequestDeserializer()
            : base(null)
        {
        }

        public override CreateSelectObjectMetaResult Deserialize(ServiceResponse xmlStream)
        {
            var result = new CreateSelectObjectMetaResult();
            var buff = new byte[128];
            uint payloadCrc32 = 0;
            int readCnt = 0;
            bool gotEndFrame = false;
            _gotEndofStream = false;
            CRC32.Init();

            while (!gotEndFrame)
            {
                //Version | Frame - Type | Payload Length | Header Checksum | Payload | Payload Checksum
                //<1 bytes> <--3 bytes-->   <-- 4 bytes --> <------4 bytes--> <variable><----4bytes------>
                //Payload 
                //<offset | data>
                //<8 bytes><variable>

                //read Header 12 bytes
                readCnt = TryRead(xmlStream.Content, buff, 0, 12);

                if (_gotEndofStream)
                    break;

                ensureReadSize(12, readCnt);

                //version

                //Frame - Type
                int frameType = OssUtils.ConvertBytesToInt(buff, 1, 3);

                //Payload Length
                int payloadLength = OssUtils.ConvertBytesToInt(buff, 4, 4);

                //Header Checksum
                int headerCheckSum = OssUtils.ConvertBytesToInt(buff, 8, 4);

                //read Playload
                switch (frameType)
                {
                    case 0x800006:      //Meta End Frame(Csv)
                    case 0x800007:      //Meta End Frame(Json)
                        payloadLength -= 32;
                        // 32 bytes
                        readCnt = TryRead(xmlStream.Content, buff, 0, 32);
                        ensureReadSize(32, readCnt);
                        payloadCrc32 = CRC32.GetCRC32Byte(buff, 32, payloadCrc32);

                        // offset 8 bytes
                        result.Offset = OssUtils.ConvertBytesToLong(buff, 0, 8);

                        // total scaned 8bytes
                        result.TotalScannedBytes = OssUtils.ConvertBytesToLong(buff, 8, 8);

                        // status 4 bytes
                        result.Status = OssUtils.ConvertBytesToInt(buff, 16, 4);

                        // splitsCount 4 bytes
                        result.SplitsCount = OssUtils.ConvertBytesToLong(buff, 20, 4);

                        // rowsCount 8 bytes
                        result.RowsCount = OssUtils.ConvertBytesToLong(buff, 24, 8);
                        // 32 bytes

                        if (frameType == 0x800006)
                        {
                            payloadLength -= 4;
                            // colsCount 4 bytes
                            readCnt = TryRead(xmlStream.Content, buff, 0, 4);
                            ensureReadSize(4, readCnt);
                            result.ColumnsCount = OssUtils.ConvertBytesToLong(buff, 0, 4);
                            payloadCrc32 = CRC32.GetCRC32Byte(buff, 4, payloadCrc32);
                        }

                        while (payloadLength > 0)
                        {
                            int len = Math.Min(buff.Length, payloadLength);
                            readCnt = TryRead(xmlStream.Content, buff, 0, len);
                            ensureReadSize(len, readCnt);
                            if (string.IsNullOrEmpty(result.ErrorMessage))
                            { 
                                result.ErrorMessage = System.Text.Encoding.Default.GetString(buff, 0, readCnt);
                            }
                            payloadCrc32 = CRC32.GetCRC32Byte(buff, readCnt, payloadCrc32);
                            payloadLength -= readCnt;
                        }

                        //Payload Checksum                
                        readCnt = TryRead(xmlStream.Content, buff, 0, 4);
                        ensureReadSize(4, readCnt);
                        uint serverCrc32 = OssUtils.ConvertBytesToUint(buff, 0, 4);

                        //Payload Crc32 check  
                        if (serverCrc32 != 0 && serverCrc32 != payloadCrc32)
                        {
                            throw new ClientException(string.Format("Payload checksum check fail server CRC {0}, client CRC {1}",
                                serverCrc32, payloadCrc32));
                        }
                        gotEndFrame = true;
                        break;
                    default:
                        //skip playload
                        readCnt = TrySkip(xmlStream.Content, payloadLength);
                        ensureReadSize(payloadLength, readCnt);

                        //skip payload Checksum
                        readCnt = TryRead(xmlStream.Content, buff, 0, 4);
                        ensureReadSize(4, readCnt);
                        break;
                }
            }

            if (!gotEndFrame)
            {
                throw new ClientException("No end frame exists.");
            }

            DeserializeGeneric(xmlStream, result);
            return result;
        }

        private int TryRead(System.IO.Stream stream, byte[] buffer, int offset, int count)
        {
            int remain = count;
            while (remain > 0 && stream.CanRead)
            {
                int read = stream.Read(buffer, offset, remain);
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

        private int TrySkip(System.IO.Stream stream, int count)
        {
            byte[] buffer = new byte[256];
            int remain = count;
            while (remain > 0)
            {
                int len = Math.Min(buffer.Length, remain);
                int read = stream.Read(buffer, 0, len);
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
                throw new ClientException(string.Format("Extract frame fail. expect {0}, but got {1}", expect, got));
            }
        }
    }
}

