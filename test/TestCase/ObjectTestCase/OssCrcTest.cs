using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;
using System.Text;

using NUnit.Framework;

using Aliyun.OSS.Util;
namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class OssCrcTest
    {
        [Test]
        public void TestCrcCore()
        {
            string testStr = "This is a test.";
            byte[] content = Encoding.ASCII.GetBytes(testStr);
            Crc64.InitECMA();
            ulong crc = Crc64.Compute(content, 0, content.Length, 0);
            Assert.AreEqual(crc, 2186167744391481992);
        }

        [Test]
        public void TestCombine()
        {
            string str1 = "this is a test.";
            string str2 = "hello world.";
            string str = str1 + str2;

            byte[] content1 = Encoding.ASCII.GetBytes(str1);
            byte[] content2 = Encoding.ASCII.GetBytes(str2);
            byte[] content = Encoding.ASCII.GetBytes(str);

            Crc64.InitECMA();
            ulong crc1 = Crc64.Compute(content1, 0, content1.Length);
            ulong crc2 = Crc64.Compute(content2, 0, content2.Length);
            ulong crc = Crc64.Compute(content, 0, content.Length);
            Assert.AreEqual(crc, Crc64.Combine(crc1, crc2, content2.Length));
        }
    }
}
