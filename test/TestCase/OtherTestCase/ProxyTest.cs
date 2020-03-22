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
    public class ProxyTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClientWithProxy();
            //get current class name, which is prefix of bucket/object
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName(_className);
            IOss ossClient = OssClientFactory.CreateOssClient();
            ossClient.CreateBucket(_bucketName);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            var client = OssClientFactory.CreateOssClient();
            OssTestUtils.CleanBucket(client, _bucketName);
        }

        [Test]
        public void ProxyAuthTest()
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
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
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

        [Test]
        public void ProxyAuthTimeoutTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                //put object
                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                OssTestUtils.WaitForCacheExpire();

                //list object
                var allObjects = _ossClient.ListObjects(_bucketName, key);
                var allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                Assert.AreEqual(1, allObjectsSumm.Count);

                OssTestUtils.WaitForCacheExpire();

                //list object
                allObjects = _ossClient.ListObjects(_bucketName, key);
                allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                Assert.AreEqual(1, allObjectsSumm.Count);

                OssTestUtils.WaitForCacheExpire();

                //list object
                allObjects = _ossClient.ListObjects(_bucketName, key);
                allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                Assert.AreEqual(1, allObjectsSumm.Count);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void ProxyAuthInvalidTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                IOss ossClient = OssClientFactory.CreateOssClientWithProxy(Config.Endpoint, 
                    Config.DisabledAccessKeyId, Config.DisabledAccessKeySecret,
                    Config.ProxyHost, Int32.Parse(Config.ProxyPort), null, null);

                OssTestUtils.ObjectExists(ossClient, _bucketName, key);

                Assert.Fail("Object Exists should not pass when it was invalid authorization");
            }
            catch (WebException e)
            {
                Assert.AreEqual(e.Status, WebExceptionStatus.ProtocolError);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

#if TEST_DOTNETCORE
        [Test]
#endif
        public void ProxyAuthInvalidTestWithDiffrentHttpClient()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                IOss ossClient = OssClientFactory.CreateOssClientWithProxy(Config.Endpoint,
                    Config.DisabledAccessKeyId, Config.DisabledAccessKeySecret,
                    Config.ProxyHost, Int32.Parse(Config.ProxyPort) + 2, null, null);

                OssTestUtils.ObjectExists(ossClient, _bucketName, key);

                Assert.Fail("Object Exists should not pass when it was invalid authorization");
            }
            catch (WebException e)
            {
                Assert.AreEqual(e.Status, WebExceptionStatus.ProtocolError);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }
    }
}
