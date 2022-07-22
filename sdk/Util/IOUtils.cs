/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using System.Threading;
using System.Threading.Tasks;

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


        public static async Task WriteToAsync(Stream src, Stream dest)
        {
            var buffer = new byte[BufferSize];
            int bytesRead;
            while ((bytesRead = await src.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await dest.WriteAsync(buffer, 0, bytesRead);
            }
            await dest.FlushAsync();
        }

        public static async Task<long> WriteToAsync(Stream orignStream, Stream destStream, long totalSize, CancellationToken cancellationToken=default)
        {
            var buffer = new byte[BufferSize];

            long alreadyRead = 0;
            while (alreadyRead < totalSize)
            {
                var readSize = await orignStream.ReadAsync(buffer, 0, BufferSize, cancellationToken);
                if (readSize <= 0)
                    break;

                if (alreadyRead + readSize > totalSize)
                    readSize = (int)(totalSize - alreadyRead);
                alreadyRead += readSize;
                await destStream.WriteAsync(buffer, 0, readSize, cancellationToken);
            }
            await destStream.FlushAsync();

            return alreadyRead;
        }
    }
}
