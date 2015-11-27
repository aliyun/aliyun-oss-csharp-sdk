using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.OtherTestClass
{
    [TestFixture]
    public class ConcurrencyTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;

        [TestFixtureSetUp]
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

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void ConcurrencyPutAndGetUsingUniqueOssClientTest()
        {
            const string name = "UniqueTest10Concurrency";
            const int numberOfConcurrency = 10;
            const int fileSize = 40960;

            RunTest(name, numberOfConcurrency, fileSize, true);
        }
 
        [Test]
        public void ConcurrencyPutAndGetUsingDifferentOssClientTest()
        {
            const string name = "DifferentTest10Concurrency";
            const int numberOfConcurrency = 10;
            const int fileSize = 40960;

            RunTest(name, numberOfConcurrency, fileSize, false);
        }

        private static void RunTest(string name, int numberOfConcurrency, int fileSize, bool isUniqueOssClient)
        {
            var allTasks = new List<Task>();

            for (var i = 0; i < numberOfConcurrency; i++)
            {
                var fileName = string.Format("{0}_{1}", name, i);
                fileName = Path.Combine(Config.UploadSampleFolder, fileName);
                var task = Task.Factory.StartNew(() => FileUtils.PrepareSampleFile(fileName, fileSize));
                allTasks.Add(task);
            }

            Task.WaitAll(allTasks.ToArray());

            allTasks.Clear();
            for (var i = 0; i < numberOfConcurrency; i++)
            {
                var fileName = string.Format("{0}_{1}", name, i);
                fileName = Path.Combine(Config.UploadSampleFolder, fileName);
                var index = i;
                var task = Task.Factory.StartNew(() => PutAndGetObject(
                    isUniqueOssClient ? _ossClient : OssClientFactory.CreateOssClient(), fileName, index));
                allTasks.Add(task);
            }

            Task.WaitAll(allTasks.ToArray());
        }

        private static void PutAndGetObject(IOss ossClient, string originalFile, int i)
        {
            //prepare the object key
            var key = OssTestUtils.GetObjectKey(_className);
            key = string.Format("{0}_{1}", key, i);
            //prepare the file for download (GET)
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = string.Format("{0}_{1}", targetFile, i);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            try
            {
                LogUtility.LogMessage("Thread {0}--Begin to put object", i);
                var sWatch = new Stopwatch();
                sWatch.Start();
                OssTestUtils.UploadObject(ossClient, _bucketName, key, originalFile);
                sWatch.Stop();
                var duration = sWatch.Elapsed;
                LogUtility.LogMessage("Thread {0}--Put Duration is {1:00}:{2:00}:{3:00}",
                    i, duration.Hours, duration.Minutes, duration.Seconds);
                LogUtility.LogMessage("Finish to put object");
                Assert.IsTrue(OssTestUtils.ObjectExists(ossClient, _bucketName, key));

                LogUtility.LogMessage("Thread {0}--Begin to get object", i);
                sWatch = new Stopwatch();
                sWatch.Start();
                OssTestUtils.DownloadObject(ossClient, _bucketName, key, targetFile);
                sWatch.Stop();
                duration = sWatch.Elapsed;
                LogUtility.LogMessage("Thread {0}--Get Duration is {1:00}:{2:00}:{3:00}",
                    i, duration.Hours, duration.Minutes, duration.Seconds);
                LogUtility.LogMessage("Thread {0}--Finish to get object", i);
            }
            finally
            {
                FileUtils.DeleteFile(originalFile);
                FileUtils.DeleteFile(targetFile);
                if (OssTestUtils.ObjectExists(ossClient, _bucketName, key))
                {
                    ossClient.DeleteObject(_bucketName, key);
                }
            }
        }
    }
}
