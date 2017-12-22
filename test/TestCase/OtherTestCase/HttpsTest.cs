using System;
using System.IO;
using System.Net;

using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.OtherTestClass
{
    [TestFixture]
    public class HttpsTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;

#if NETCOREAPP2_0
        [OneTimeSetUp]
#else
        [TestFixtureSetUp]
#endif
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClientUseHttps();
            //get current class name, which is prefix of bucket/object
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName(_className);
            _ossClient.CreateBucket(_bucketName);
        }

#if NETCOREAPP2_0
        [OneTimeTearDown]
#else
        [TestFixtureTearDown]
#endif
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void SimpleHttpsTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            try {
                //put object
                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                //get object
                OssTestUtils.DownloadObject(_ossClient, _bucketName, key, targetFile);
                var expectedETag = _ossClient.GetObjectMetadata(_bucketName, key).ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());

                //list object
                var allObjects = _ossClient.ListObjects(_bucketName, key);
                var allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                Assert.AreEqual(1, allObjectsSumm.Count);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);

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
