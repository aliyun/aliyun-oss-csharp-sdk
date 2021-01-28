using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Aliyun.OSS.Test.Util
{
    public static class FileUtils
    {
        public static string ComputeContentMd5(string inputFile)
        {
            using (Stream inputStream = File.OpenRead(inputFile))
            {
                using (var md5 = MD5.Create())
                {
                    // Compute hash data of the input stream.

                    var data = md5.ComputeHash(inputStream);
                    // Create a new Stringbuilder to collect the bytes
                    // and create a string.
                    var sBuilder = new StringBuilder();

                    // Loop through each byte of the hashed data
                    // and format each one as a hexadecimal string.
                    foreach (var t in data)
                    {
                        sBuilder.Append(t.ToString("x2"));
                    }

                    // Return the hexadecimal string.
                    return sBuilder.ToString();
                }
            }
        }

        public static string ComputeContentMd5(Stream inputStream)
        {
            using (var md5 = MD5.Create())
            {
                var data = md5.ComputeHash(inputStream);
                var sBuilder = new StringBuilder();
                foreach (var t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static void PrepareSampleFile(string filePath, int numberOfKbs)
        {
            if (File.Exists(filePath)) return;

            if (Directory.Exists(Path.GetDirectoryName(filePath)) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            using (var file = new StreamWriter(filePath))
            {
                for (var i = 0; i < numberOfKbs; i++)
                {
                    file.WriteLine(GenerateOneKb());
                }
                file.Flush();
            }
        }

        private const string AvailableCharacters = "01234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int OneKbCount = 1024;
        public static string GenerateOneKb()
        {
            var r = new Random();
            var sb = new StringBuilder();
            var i = 0;
            while (i++ < OneKbCount)
            {
                var pos = r.Next(AvailableCharacters.Length);
                sb.Append(AvailableCharacters[pos]);
            }
            return sb.ToString();
        }
    }
}
