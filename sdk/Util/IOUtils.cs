/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;

namespace Aliyun.OSS.Util
{
    public static class IoUtils
    {
        private const int BufferSize = 4 * 1024;
        
        public static void WriteTo(Stream src, Stream dest)
        {            
            var buffer = new byte[BufferSize];
            int bytesRead;
            while((bytesRead = src.Read(buffer, 0, buffer.Length)) > 0)
            {
                dest.Write(buffer, 0, bytesRead);
            }
            dest.Flush();
        }
        
        public static long WriteTo(Stream orignStream, Stream destStream, long totalSize)
        {
            var buffer = new byte[BufferSize];

            long alreadyRead = 0;
            while (alreadyRead < totalSize)
            {
                var readSize = orignStream.Read(buffer, 0, BufferSize);
                if (readSize <= 0)
                    break;

                if (alreadyRead + readSize > totalSize)
                    readSize = (int)(totalSize - alreadyRead);
                alreadyRead += readSize;
                destStream.Write(buffer, 0, readSize);
            }
            destStream.Flush();

            return alreadyRead;
        }
    }
}
