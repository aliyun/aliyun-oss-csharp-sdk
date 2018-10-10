using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.OtherTestClass
{
    [TestFixture]
    public class IpTest
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
            _ossClient = OssClientFactory.CreateOssClient();
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

        private static bool IsIp(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        [Test]
        public void IpTestObjectOperation()
        {
            var ip = OssTestUtils.GetIpByEndpoint(Config.Endpoint);
            var ossClient = new OssClient(ip, Config.AccessKeyId, Config.AccessKeySecret);
            var key = OssTestUtils.GetObjectKey(_className);
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            try {
                //put object
                ossClient.PutObject(_bucketName, key, Config.UploadTestFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(ossClient, _bucketName, key));

                //get object
                OssTestUtils.DownloadObject(ossClient, _bucketName, key, targetFile);
                var expectedETag = ossClient.GetObjectMetadata(_bucketName, key).ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());

                //list object
                var allObjects = ossClient.ListObjects(_bucketName, key);
                var allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                Assert.AreEqual(1, allObjectsSumm.Count);
            }
            catch
            {
                Assert.Fail("");
            }
            finally
            {
                FileUtils.DeleteFile(targetFile);
            }
        }

        [Test]
        public void IpTestUseDifferentConfigSetting()
        {
            var connectionLimit = ClientConfiguration.ConnectionLimit;
            ClientConfiguration.ConnectionLimit = 200;

            var conf = new ClientConfiguration();
            conf.MaxErrorRetry = 2;
            conf.Protocol = Protocol.Https;
            conf.ProgressUpdateInterval = 2 * conf.ProgressUpdateInterval;
            conf.DirectWriteStreamThreshold = 2 * conf.DirectWriteStreamThreshold;

            var ip = OssTestUtils.GetIpByEndpoint(Config.Endpoint);
            var ossClient = new OssClient(ip, Config.AccessKeyId, Config.AccessKeySecret, conf);

            var key = OssTestUtils.GetObjectKey(_className);
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            try
            {
                //put object
                ossClient.PutObject(_bucketName, key, Config.UploadTestFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(ossClient, _bucketName, key));

                //get object
                OssTestUtils.DownloadObject(ossClient, _bucketName, key, targetFile);
                var expectedETag = ossClient.GetObjectMetadata(_bucketName, key).ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            }
            catch
            {
                Assert.Fail("");
            }
            finally
            {
                ClientConfiguration.ConnectionLimit = connectionLimit;
                FileUtils.DeleteFile(targetFile);
            }
        }
    }
}
