using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;
using Aliyun.OSS.Util;
using Aliyun.OSS.Common.Internal;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectTrafficLimitTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _data_200KB;

        [OneTimeSetUp]
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

            _data_200KB = OssTestUtils.GetRandomString(200 * 1024);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void PutAndGetObjectTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var content = new MemoryStream(Encoding.ASCII.GetBytes(_data_200KB));
            var putRequest = new PutObjectRequest(_bucketName, key, content);
            Assert.AreEqual(putRequest.TrafficLimit, 0);
            var putResult = _ossClient.PutObject(putRequest);
            Assert.AreEqual(putResult.ResponseMetadata.ContainsKey(HttpHeaders.QosDelayTime), false);


            content = new MemoryStream(Encoding.ASCII.GetBytes(_data_200KB));
            putRequest = new PutObjectRequest(_bucketName, key, content)
            {
                TrafficLimit = 100*1024*8
            };
            Assert.AreEqual(putRequest.TrafficLimit, 819200);
            putResult = _ossClient.PutObject(putRequest);
            Assert.AreEqual(putResult.ResponseMetadata.ContainsKey(HttpHeaders.QosDelayTime), true);


            var getRequest = new GetObjectRequest(_bucketName, key);
            Assert.AreEqual(getRequest.TrafficLimit, 0);
            var getResult = _ossClient.GetObject(getRequest);
            Assert.AreEqual(getResult.Metadata.HttpMetadata.ContainsKey(HttpHeaders.QosDelayTime), false);

            getRequest = new GetObjectRequest(_bucketName, key)
            {
                TrafficLimit = 100 * 1024 * 8
            };
            Assert.AreEqual(getRequest.TrafficLimit, 819200);
            getResult = _ossClient.GetObject(getRequest);
            Assert.AreEqual(getResult.Metadata.HttpMetadata.ContainsKey(HttpHeaders.QosDelayTime), true);
        }

        [Test]
        public void AppendObjectTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            long position = 0;
            var request = new AppendObjectRequest(_bucketName, key)
            {
                Content = new MemoryStream(Encoding.ASCII.GetBytes(_data_200KB)),
                Position = position
            };
            Assert.AreEqual(request.TrafficLimit, 0);

            var result = _ossClient.AppendObject(request);
            Assert.AreEqual(_data_200KB.Length, result.NextAppendPosition);
            position = result.NextAppendPosition;
            Assert.AreEqual(result.ResponseMetadata.ContainsKey(HttpHeaders.QosDelayTime), false);


            request = new AppendObjectRequest(_bucketName, key)
            {
                Content = new MemoryStream(Encoding.ASCII.GetBytes(_data_200KB)),
                Position = position,
                TrafficLimit = 100 * 1024 * 8
            };
            Assert.AreEqual(request.TrafficLimit, 819200);
            result = _ossClient.AppendObject(request);
            Assert.IsTrue(result.HashCrc64Ecma != 0);
            Assert.AreEqual(result.ResponseMetadata.ContainsKey(HttpHeaders.QosDelayTime), true);
        }

        [Test]
        public void CopyObjectTest()
        {
            var sourceObjectKey = OssTestUtils.GetObjectKey(_className);
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, sourceObjectKey, new MemoryStream(Encoding.ASCII.GetBytes(_data_200KB)));

            var coRequest = new CopyObjectRequest(_bucketName, sourceObjectKey, _bucketName, targetObjectKey);
            Assert.AreEqual(coRequest.TrafficLimit, 0);
            var copyResult = _ossClient.CopyObject(coRequest);
            Assert.AreEqual(copyResult.ResponseMetadata.ContainsKey(HttpHeaders.QosDelayTime), false);

            coRequest = new CopyObjectRequest(_bucketName, sourceObjectKey, _bucketName, targetObjectKey)
            {
                TrafficLimit = 100 * 1024 * 8
            };

            Assert.AreEqual(coRequest.TrafficLimit, 819200);
            copyResult = _ossClient.CopyObject(coRequest);
            //Assert.AreEqual(copyResult.ResponseMetadata.ContainsKey(HttpHeaders.QosDelayTime), true);

            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));
        }

        [Test]
        public void UploadPartTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var request = new UploadPartRequest(_bucketName, key, "uploadId");
            Assert.AreEqual(request.TrafficLimit, 0);

            request = new UploadPartRequest(_bucketName, key, "uploadId")
            {
                TrafficLimit = 100 * 1024 * 8
            };
            Assert.AreEqual(request.TrafficLimit, 819200);
        }

        [Test]
        public void UploadPartCopyTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var request = new UploadPartCopyRequest(_bucketName, key, _bucketName, key, "uploadId");
            Assert.AreEqual(request.TrafficLimit, 0);

            request = new UploadPartCopyRequest(_bucketName, key, _bucketName, key, "uploadId")
            {
                TrafficLimit = 100 * 1024 * 8
            };
            Assert.AreEqual(request.TrafficLimit, 819200);
        }

        [Test]
        public void ResumableUploadDownloadTest()
        {
            var filePath = OssTestUtils.GetTargetFileName(_className);
            filePath = Path.Combine(Config.DownloadFolder, filePath);
            FileUtils.PrepareSampleFile(filePath, 800);

            var fileName = filePath;
            var fileInfo = new FileInfo(fileName);
            var fileSize = fileInfo.Length;

            //  < PartSize
            var key = OssTestUtils.GetObjectKey(_className);
            var request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = fileSize + 1,
            };
            Assert.AreEqual(request.TrafficLimit, 0);
            DateTime beforDT = System.DateTime.Now;
            var result = _ossClient.ResumableUploadObject(request);
            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforDT);
            Assert.IsTrue(ts.TotalSeconds < 2);

            request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = fileSize + 1,
                TrafficLimit = 100 * 1024 * 8
            };
            Assert.AreEqual(request.TrafficLimit, 819200);
            beforDT = System.DateTime.Now;
            result = _ossClient.ResumableUploadObject(request);
            afterDT = System.DateTime.Now;
            ts = afterDT.Subtract(beforDT);
            Assert.IsTrue(ts.TotalSeconds > 6);


            // > PartSize with single thread
            key = OssTestUtils.GetObjectKey(_className);
            request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = 400 * 1024,
                ParallelThreadCount = 1
            };
            Assert.AreEqual(request.TrafficLimit, 0);
            beforDT = DateTime.Now;
            result = _ossClient.ResumableUploadObject(request);
            afterDT = DateTime.Now;
            ts = afterDT.Subtract(beforDT).Duration();
            Assert.IsTrue(ts.TotalSeconds < 3);

            key = OssTestUtils.GetObjectKey(_className);
            request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = 400 * 1024,
                ParallelThreadCount = 1,
                TrafficLimit = 100 * 1024 * 8
            };
            Assert.AreEqual(request.TrafficLimit, 819200);
            beforDT = System.DateTime.Now;
            result = _ossClient.ResumableUploadObject(request);
            afterDT = System.DateTime.Now;
            ts = afterDT.Subtract(beforDT);
            Assert.IsTrue(ts.TotalSeconds > 6 );


            // > PartSize with 2 thread
            key = OssTestUtils.GetObjectKey(_className);
            request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = 200 * 1024,
                ParallelThreadCount = 2
            };
            Assert.AreEqual(request.TrafficLimit, 0);
            beforDT = DateTime.Now;
            result = _ossClient.ResumableUploadObject(request);
            afterDT = DateTime.Now;
            ts = afterDT.Subtract(beforDT).Duration();
            Assert.IsTrue(ts.TotalSeconds < 2);

            key = OssTestUtils.GetObjectKey(_className);
            request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = 200 * 1024,
                ParallelThreadCount = 2,
                TrafficLimit = 100 * 1024 * 8
            };
            Assert.AreEqual(request.TrafficLimit, 819200);
            beforDT = System.DateTime.Now;
            result = _ossClient.ResumableUploadObject(request);
            afterDT = System.DateTime.Now;
            ts = afterDT.Subtract(beforDT);
            Assert.IsTrue(ts.TotalSeconds > 6);


            //download
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            //  < PartSize
            var dRequest = new DownloadObjectRequest(_bucketName, key, targetFile)
            {
                PartSize = fileSize + 1,
            };
            Assert.AreEqual(dRequest.TrafficLimit, 0);
            beforDT = System.DateTime.Now;
            var metadata = _ossClient.ResumableDownloadObject(dRequest);
            afterDT = System.DateTime.Now;
            ts = afterDT.Subtract(beforDT);
            Assert.IsTrue(ts.TotalSeconds < 2);
            FileUtils.DeleteFile(targetFile);

            // > PartSize with multi thread
            dRequest = new DownloadObjectRequest(_bucketName, key, targetFile)
            {
                PartSize = 200 * 1024,
                ParallelThreadCount = 2,
                TrafficLimit = 100 * 1024 * 8
            };
            Assert.AreEqual(dRequest.TrafficLimit, 819200);
            afterDT = System.DateTime.Now;
            metadata = _ossClient.ResumableDownloadObject(dRequest);
            afterDT = System.DateTime.Now;
            ts = afterDT.Subtract(beforDT);
            Assert.IsTrue(ts.TotalSeconds > 4);
            FileUtils.DeleteFile(targetFile);

            // > PartSize with single thread
            dRequest = new DownloadObjectRequest(_bucketName, key, targetFile)
            {
                PartSize = 200 * 1024,
                ParallelThreadCount = 1,
                TrafficLimit = 100 * 1024 * 8
            };
            Assert.AreEqual(dRequest.TrafficLimit, 819200);
            afterDT = System.DateTime.Now;
            metadata = _ossClient.ResumableDownloadObject(dRequest);
            afterDT = System.DateTime.Now;
            ts = afterDT.Subtract(beforDT);
            Assert.IsTrue(ts.TotalSeconds > 6);
            FileUtils.DeleteFile(targetFile);


            //FileUtils.DeleteFile(filePath);
        }

        [Test]
        public void SignedUrlTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var content = new MemoryStream(Encoding.ASCII.GetBytes(_data_200KB));
            _ossClient.PutObject(new PutObjectRequest(_bucketName, key, content));

            var request = new GeneratePresignedUriRequest(_bucketName, key, SignHttpMethod.Get);
            request.AddQueryParam("x-oss-traffic-limit", "819200");
            var uri = _ossClient.GeneratePresignedUri(request);
            Assert.AreNotEqual(uri.ToString().IndexOf("x-oss-traffic-limit=819200"), -1);
            var result = _ossClient.GetObject(uri);
            Assert.AreEqual(result.ResponseMetadata.ContainsKey(HttpHeaders.QosDelayTime), true);
        }
    }
}
