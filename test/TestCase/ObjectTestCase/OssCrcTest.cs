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
        private static string _className;
        private static string _bucketName;
        private static IOss _ossClient;

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            //get current class name, which is prefix of bucket/object
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            _ossClient = OssClientFactory.CreateOssClient();
            _bucketName = OssTestUtils.GetBucketName(_className);
            _ossClient.CreateBucket(_bucketName);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

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

            Assert.AreEqual(crc1, Crc64.Combine(crc1, crc2, 0));

            ulong crc3 = Crc64.Compute(content2, 0, content2.Length, crc1);
            Assert.AreEqual(crc3, crc);
        }

        [Test]
        public void TestCrcCompute()
        {
            ulong[] crcVals = { 0x0/*0*/, 0x330284772E652B05, 0xBC6573200E84B046, 0x2CD8094A1A277627, 0x3C9D28596E5960BA, 
                                0x40BDF58FB0895F2/*5*/, 0xD08E9F8545A700F4, 0xEC20A3A8CC710E66, 0x67B4F30A647A0C59, 0x9966F6C89D56EF8E,
                                0x32093A2ECD5773F4/*10*/, 0x8A0825223EA6D221, 0x8562C0AC2AB9A00D, 0x3EE2A39C083F38B4, 0x1F603830353E518A,
                                0x2FD681D7B2421FD/*15*/, 0x790EF2B16A745A41, 0x3EF8F06DACCDCDDF, 0x49E41B2660B106D, 0x561CC0CFA235AC68,
                                0xD4FE9EF082E69F59/*20*/, 0xE3B5E46CD8D63A4D, 0x865AAF6B94F2A051, 0x7ECA10D2F8136EB4, 0xD7DD118C98E98727,
                                0x70FB33C119C29318/*25*/, 0x57C891E39A97D9B7, 0xA1F46BA20AD06EB7, 0x7AD25FAFA1710407, 0x73CEF1666185C13F,
                                0xB41858F73C389602/*30*/ };

            string[] textVals = { ""/*0*/, "a", "ab", "abc", "abcd", "abcde"/*5*/, "abcdef", "abcdefg", "abcdefgh", "abcdefghi", "abcdefghij"/*10*/,
                                  "Discard medicine more than two years old.",
                                  "He who has a shady past knows that nice guys finish last.",
                                  "I wouldn't marry him with a ten foot pole.",
                                  "Free! Free!/A trip/to Mars/for 900/empty jars/Burma Shave",
                                  "The days of the digital watch are numbered.  -Tom Stoppard"/*15*/,
                                  "Nepal premier won't resign.", 
                                  "For every action there is an equal and opposite government program.",
                                  "His money is twice tainted: 'taint yours and 'taint mine.",
                                  "There is no reason for any individual to have a computer in their home. -Ken Olsen, 1977",
                                  "It's a tiny change to the code and not completely disgusting. - Bob Manchek"/*20*/,
                                  "size:  a.out:  bad magic",
                                  "The major problem is with sendmail.  -Mark Horton",
                                  "Give me a rock, paper and scissors and I will move the world.  CCFestoon",
                                  "If the enemy is within range, then so are you.",
                                  "It's well we cannot hear the screams/That we create in others' dreams."/*25*/,
                                  "You remind me of a TV show, but that's all right: I watch it anyway.",
                                  "C is as portable as Stonehedge!!",
                                  "Even if I could be Shakespeare, I think I should still choose to be Faraday. - A. Huxley",
                                  "The fugacity of a constituent in a mixture of gases at a given temperature is proportional to its mole fraction.  Lewis-Randall Rule",
                                  "How can you write a big system without C++?  -Paul Glick"/*30*/ };

            Crc64.InitECMA();
            for (int i = 0; i < crcVals.Length; i++)
            {
                byte[] content = Encoding.ASCII.GetBytes(textVals[i]);
                ulong crc = Crc64.Compute(content, 0, content.Length, 0);
                System.Console.WriteLine("{0:X}", crc);
                Assert.AreEqual(crc, crcVals[i]);
            }
        }

        [Test]
        public void TestCrcCombine()
        {
            string text = "Even if I could be Shakespeare, I think I should still choose to be Faraday. - A. Huxley";
            byte[] content = Encoding.ASCII.GetBytes(text);
            Crc64.InitECMA();
            ulong crc = Crc64.Compute(content, 0, content.Length);

            for (int i = 1; i < text.Length; i++)
            {
                string before = text.Substring(0, i);
                string after = text.Substring(i);
                byte[] content1 = Encoding.ASCII.GetBytes(before);
                byte[] content2 = Encoding.ASCII.GetBytes(after);
               
                ulong crc1 = Crc64.Compute(content1, 0, content1.Length);
                ulong crc2 = Crc64.Compute(content2, 0, content2.Length);
                Assert.AreEqual(crc, Crc64.Combine(crc1, crc2, content2.Length));

                ulong crc3 = Crc64.Compute(content2, 0, content2.Length, crc1);
                Assert.AreEqual(crc3, crc);
            }
        }

        [Test]
        public void TestDisableCrc()
        {
            Common.ClientConfiguration config = new Common.ClientConfiguration();
            config.EnableCrcCheck = false;
            IOss ossClient = OssClientFactory.CreateOssClient(config);
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            var objectKeyName = "test-object-disable-crc";

            try
            {
                // put & get
                PutObjectResult putObjectResult = ossClient.PutObject(_bucketName, objectKeyName, Config.UploadTestFile);
                var actualCrc = putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma];

                OssObject ossObject = ossClient.GetObject(_bucketName, objectKeyName);
                var expectedCrc = OssUtils.ComputeContentCrc64(ossObject.Content, ossObject.ContentLength);

                Assert.AreEqual(expectedCrc, actualCrc);
                ossClient.DeleteObject(_bucketName, objectKeyName);

                // put & get by uri
                var testStr = FileUtils.GenerateOneKb();
                var content = Encoding.ASCII.GetBytes(testStr);
                var now = DateTime.Now;
                var expireDate = now.AddSeconds(120);
                var uri = _ossClient.GeneratePresignedUri(_bucketName, objectKeyName, expireDate, SignHttpMethod.Put);
                var putResult = ossClient.PutObject(uri, new MemoryStream(content));

                expectedCrc = putResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma];
                actualCrc = OssUtils.ComputeContentCrc64(new MemoryStream(content), content.Length);
                Assert.AreEqual(expectedCrc, actualCrc);

                uri = _ossClient.GeneratePresignedUri(_bucketName, objectKeyName, expireDate, SignHttpMethod.Get);
                ossObject =  ossClient.GetObject(uri);
                expectedCrc = OssUtils.ComputeContentCrc64(ossObject.Content, ossObject.ContentLength);

                Assert.AreEqual(expectedCrc, actualCrc);
                ossClient.DeleteObject(_bucketName, objectKeyName);

                // upload & download
                var uploadObjectResult = ossClient.ResumableUploadObject(_bucketName, objectKeyName, Config.MultiUploadTestFile, null,
                                           Config.DownloadFolder);
                actualCrc = uploadObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma];

                DownloadObjectRequest downloadObjectRequest = new DownloadObjectRequest(_bucketName, objectKeyName, targetFile);
                var metadata = ossClient.ResumableDownloadObject(downloadObjectRequest);

                Assert.AreEqual(metadata.Crc64, actualCrc);
                ossClient.DeleteObject(_bucketName, objectKeyName);

                // append
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var appendObjectRequest = new AppendObjectRequest(_bucketName, objectKeyName)
                    {
                        Content = fs,
                    };
                    var appendObjectResult = _ossClient.AppendObject(appendObjectRequest);
                    fs.Seek(0, SeekOrigin.Begin);
                    actualCrc = OssUtils.ComputeContentCrc64(fs, fs.Length);
                    Assert.AreEqual(appendObjectResult.HashCrc64Ecma.ToString(), actualCrc);
                    ossClient.DeleteObject(_bucketName, objectKeyName);
                }
            }
            finally
            {
                try
                {
                    FileUtils.DeleteFile(targetFile);
                }
                catch (Exception e)
                {
                    LogUtility.LogWarning("Delete file {0} failed with Exception {1}", targetFile, e.Message);
                }
            }
        }
    }
}
