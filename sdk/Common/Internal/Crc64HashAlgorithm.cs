using System;
using Aliyun.OSS.Util;
namespace Aliyun.OSS.Common.Internal
{
    public class Crc64HashAlgorithm : System.Security.Cryptography.HashAlgorithm
    {
        private ulong crc64 = 0; 

        public Crc64HashAlgorithm()
        {
            Crc64.InitECMA();
        }

        public override void Initialize()
        {
            Crc64.InitECMA();
        }

        public void SetInitCrc(ulong initCrc)
        {
            crc64 = initCrc;
        }

        public override int HashSize
        {
            get {
                return 64;
            }
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            crc64 = Crc64.Compute(array, ibStart, cbSize, crc64);
        }

        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(crc64);
        }
    }
}
