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

        class PrepareFileThread
        {
            private string _fileName = null;
            private int _fileSize = 0;
            private ManualResetEvent _doneEvent;

            public PrepareFileThread(string fileName, int fileSize, ManualResetEvent doneEvent)
            {
                _fileName = fileName;
                _fileSize = fileSize;
                _doneEvent = doneEvent;
            }

            public void ThreadPoolCallback(Object threadIndex)
            {
                FileUtils.PrepareSampleFile(_fileName, _fileSize);

                _doneEvent.Set();
            }

        };

        class ObjectOperateThread
        {
            private string _fileName = null;
            private int _index = 0;
            private IOss _client = null;
            private ManualResetEvent _doneEvent;

            public ObjectOperateThread(IOss client, string fileName, int index, ManualResetEvent doneEvent)
            {
                _client = client;
                _fileName = fileName;
                _index = index;
                _doneEvent = doneEvent;
            }

            public void ThreadPoolCallback(Object threadIndex)
            {
                PutAndGetObject(_client, _fileName, _index);

                _doneEvent.Set();
            }

        };

        private static void RunTest(string name, int numberOfConcurrency, int fileSize, bool isUniqueOssClient)
        {
            var doneEvents = new List<ManualResetEvent>();
            var prepareFileThreads = new List<PrepareFileThread>();

            for (int i = 0; i < numberOfConcurrency; i++)
            {
                var resetEvent = new ManualResetEvent(false);
                doneEvents.Add(resetEvent);
                var fileName = Path.Combine(Config.UploadSampleFolder, string.Format("{0}_{1}", name, i));
                var threadWrapper = new PrepareFileThread(fileName, fileSize, doneEvents[i]);
                prepareFileThreads.Add(threadWrapper);
                ThreadPool.QueueUserWorkItem(threadWrapper.ThreadPoolCallback, i);
            }
            WaitHandle.WaitAll(doneEvents.ToArray());
            doneEvents.Clear();
            prepareFileThreads.Clear();

            var objectOperateThreads = new List<ObjectOperateThread>();

            for (int i = 0; i < numberOfConcurrency; i++)
            {
                var resetEvent = new ManualResetEvent(false);
                doneEvents.Add(resetEvent);
                var fileName = Path.Combine(Config.UploadSampleFolder, string.Format("{0}_{1}", name, i));
                var client = isUniqueOssClient ? _ossClient : OssClientFactory.CreateOssClient();
                var threadWrapper = new ObjectOperateThread(client, fileName, i, doneEvents[i]);
                objectOperateThreads.Add(threadWrapper);
                ThreadPool.QueueUserWorkItem(threadWrapper.ThreadPoolCallback, i);
            }
            WaitHandle.WaitAll(doneEvents.ToArray());
            doneEvents.Clear();
            objectOperateThreads.Clear();
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
