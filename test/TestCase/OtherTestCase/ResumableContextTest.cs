using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;
using System.Threading;
using System;
namespace Aliyun.OSS.Test.TestClass.OtherTestClass
{
    [TestFixture]
    public class ResumableContextTest
    {
        [Test]
        public void ResumablePartContextSerializationTest()
        {
            ResumablePartContext partContext = new ResumablePartContext();
            partContext.Position = 1000;
            partContext.Length = 1024 * 100;
            partContext.IsCompleted = true;
            partContext.Crc64 = 49417943;
            partContext.PartId = 1;
            partContext.PartETag = new PartETag(3, "1234567890");

            string s = partContext.ToString();

            ResumablePartContext partContext2 = new ResumablePartContext();
            partContext2.FromString(s);
            Assert.AreEqual(partContext.Position, partContext2.Position);
            Assert.AreEqual(partContext.Length, partContext2.Length);
            Assert.AreEqual(partContext.IsCompleted, partContext2.IsCompleted);
            Assert.AreEqual(partContext.Crc64, partContext2.Crc64);
            Assert.AreEqual(partContext.PartETag.ETag, partContext2.PartETag.ETag);
            Assert.AreEqual(partContext.PartETag.PartNumber, partContext2.PartETag.PartNumber);  
        }

        [Test]
        public void ResumableContextSerializationTest()
        {
            ResumableContextSerializationTest("testMd5");
        }

        [Test]
        public void ResumableContextWithoutMd5SerializationTest()
        {
            ResumableContextSerializationTest(null);
        }

        public void ResumableContextSerializationTest(string md5)
        {
            ResumableContext resumableContext = new ResumableContext("bk1", "key1", "chkdir");
            resumableContext.UploadId = "UploadId";
            resumableContext.PartContextList = new List<ResumablePartContext>();
            ResumablePartContext partContext = new ResumablePartContext();
            partContext.Position = 1000;
            partContext.Length = 1024 * 100;
            partContext.IsCompleted = true;
            partContext.Crc64 = 49417943;
            partContext.PartId = 1;
            partContext.PartETag = new PartETag(3, "1234567890");

            resumableContext.PartContextList.Add(partContext);
            resumableContext.ContentMd5 = md5;
            resumableContext.Crc64 = "1234567890";

            string s = resumableContext.ToString();

            ResumableContext resumableContext2 = new ResumableContext("bk2", "key2", "chdir2");
            resumableContext2.FromString(s);
            Assert.AreEqual(resumableContext.ContentMd5, resumableContext2.ContentMd5);
            Assert.AreEqual(resumableContext.Crc64, resumableContext2.Crc64);
            Assert.AreEqual(resumableContext.PartContextList.Count, resumableContext2.PartContextList.Count);

            ResumablePartContext partContext2 = resumableContext2.PartContextList[0];
            Assert.AreEqual(partContext.Position, partContext2.Position);
            Assert.AreEqual(partContext.Length, partContext2.Length);
            Assert.AreEqual(partContext.IsCompleted, partContext2.IsCompleted);
            Assert.AreEqual(partContext.Crc64, partContext2.Crc64);
            Assert.AreEqual(partContext.PartETag.ETag, partContext2.PartETag.ETag);
            Assert.AreEqual(partContext.PartETag.PartNumber, partContext2.PartETag.PartNumber);
        }
    }
}
