using System;
using System.Net;
using System.Text;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;
using Aliyun.OSS.Util;

using NUnit.Framework;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Common.Internal;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectHashCheckTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _objectKey;
        private static string _bigObjectKey;
        private static string _tmpLocalFile;
        private static IOss _ossClientDisableMD5;

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            _ossClientDisableMD5 = OssClientFactory.CreateOssClientEnableMD5(false);
            //get current class name, which is prefix of bucket/object
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName(_className);
            _ossClient.CreateBucket(_bucketName);

            //upload sample object
            _objectKey = OssTestUtils.GetObjectKey(_className);
            //upload multipart sample object
            _bigObjectKey = _objectKey + ".js";

            // temporary local file
            _tmpLocalFile = _className + ".tmp";
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        #region put object
        [Test]
        public void PutObjectEnableMD5CheckTest()
        {
            // put object
            _ossClient.PutObject(_bucketName, _objectKey, Config.UploadTestFile);

            // get object
            var ossObject = _ossClient.GetObject(_bucketName, _objectKey);
            using (var requestStream = ossObject.Content)
            {
                using (var localfile = File.Open(_tmpLocalFile, FileMode.OpenOrCreate))
                {
                    OssTestUtils.WriteTo(requestStream, localfile);
                }
            }

            // check md5
            var expectedHashDigest = FileUtils.ComputeContentMd5(Config.UploadTestFile);
            var actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
            Assert.AreEqual(expectedHashDigest, actualHashDigest);

            _ossClient.DeleteObject(_bucketName, _objectKey);
            File.Delete(_tmpLocalFile);
        }

        [Test]
        public void PutObjectDisableMD5CheckTest()
        {
            // put object
            _ossClientDisableMD5.PutObject(_bucketName, _objectKey, Config.UploadTestFile);

            // get object
            var ossObject = _ossClientDisableMD5.GetObject(_bucketName, _objectKey);
            using (var requestStream = ossObject.Content)
            {
                using (var localfile = File.Open(_tmpLocalFile, FileMode.OpenOrCreate))
                {
                    OssTestUtils.WriteTo(requestStream, localfile);
                }
            }

            // check md5
            var expectedHashDigest = FileUtils.ComputeContentMd5(Config.UploadTestFile);
            var actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
            Assert.AreEqual(expectedHashDigest, actualHashDigest);

            _ossClientDisableMD5.DeleteObject(_bucketName, _objectKey);
            File.Delete(_tmpLocalFile);
        }
        #endregion

        #region append object
        [Test]
        public void AppendObjectEnableMD5CheckTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                long position = 0;
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    // the first time append 
                    var fileLength = fs.Length / 2;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = new PartialWrapperStream(fs, fileLength),
                        Position = position
                    };

                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                    position = result.NextAppendPosition;

                    // the second time append 
                    fs.Position = fs.Length / 2;
                    fileLength = fs.Length - fs.Length / 2;
                    request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = new PartialWrapperStream(fs, fileLength),
                        Position = position
                    };

                    result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fs.Length, result.NextAppendPosition);
                    Assert.IsTrue(result.HashCrc64Ecma != 0);
                }

                // check md5
                OssTestUtils.DownloadObject(_ossClient, _bucketName, key, _tmpLocalFile);
                var expectedHashDigest = FileUtils.ComputeContentMd5(Config.UploadTestFile);
                var actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
                Assert.AreEqual(expectedHashDigest, actualHashDigest);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
                File.Delete(_tmpLocalFile);
            }
        }

        [Test]
        public void AppendObjectDisableMD5CheckTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                long position = 0;
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    // the first time append 
                    var fileLength = fs.Length / 2;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = new PartialWrapperStream(fs, fileLength),
                        Position = position
                    };

                    var result = _ossClientDisableMD5.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                    position = result.NextAppendPosition;

                    // the second time append 
                    fs.Position = fs.Length / 2;
                    fileLength = fs.Length - fs.Length / 2;
                    request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = new PartialWrapperStream(fs, fileLength),
                        Position = position
                    };

                    result = _ossClientDisableMD5.AppendObject(request);
                    Assert.AreEqual(fs.Length, result.NextAppendPosition);
                    Assert.IsTrue(result.HashCrc64Ecma != 0);
                }

                // check md5
                OssTestUtils.DownloadObject(_ossClientDisableMD5, _bucketName, key, _tmpLocalFile);
                var expectedHashDigest = FileUtils.ComputeContentMd5(Config.UploadTestFile);
                var actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
                Assert.AreEqual(expectedHashDigest, actualHashDigest);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClientDisableMD5, _bucketName, key))
                {
                    _ossClientDisableMD5.DeleteObject(_bucketName, key);
                }
                File.Delete(_tmpLocalFile);
            }
        }
        #endregion

        #region multipart upload
        [Test]
        public void MultipartUploadEnableMD5CheckTest()
        {
            // 初始化分片上传任务
            var initRequest = new InitiateMultipartUploadRequest(_bucketName, _bigObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);

            // 设置每块为 1M
            const int partSize = 1024 * 1024 * 1;
            var partFile = new FileInfo(Config.MultiUploadTestFile);
            // 计算分块数目
            var partCount = OssTestUtils.CalculatePartCount(partFile.Length, partSize);

            // 新建一个List保存每个分块上传后的ETag和PartNumber
            var partETags = new List<PartETag>();
            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    // 跳到每个分块的开头
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // 计算每个分块的大小
                    var size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // 创建UploadPartRequest，上传分块
                    var uploadPartRequest = new UploadPartRequest(_bucketName, _bigObjectKey, initResult.UploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = (i + 1)
                    };
                    var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);

                    // 将返回的PartETag保存到List中。
                    partETags.Add(uploadPartResult.PartETag);
                }
            }

            // 提交上传任务
            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, _bigObjectKey, initResult.UploadId);
            foreach (var partETag in partETags)
            {
                completeRequest.PartETags.Add(partETag);
            }
            _ossClient.CompleteMultipartUpload(completeRequest);

            // 校验文件内容
            OssTestUtils.DownloadObject(_ossClient, _bucketName, _bigObjectKey, _tmpLocalFile);
            var expectedHashDigest = FileUtils.ComputeContentMd5(Config.MultiUploadTestFile);
            var actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
            Assert.AreEqual(expectedHashDigest, actualHashDigest);

            _ossClient.DeleteObject(_bucketName, _bigObjectKey);
            File.Delete(_tmpLocalFile);
        }

        [Test]
        public void MultipartUploadDisableMD5CheckTest()
        {
            // 初始化分片上传任务
            var initRequest = new InitiateMultipartUploadRequest(_bucketName, _bigObjectKey);
            var initResult = _ossClientDisableMD5.InitiateMultipartUpload(initRequest);

            // 设置每块为 1M
            const int partSize = 1024 * 1024 * 1;
            var partFile = new FileInfo(Config.MultiUploadTestFile);
            // 计算分块数目
            var partCount = OssTestUtils.CalculatePartCount(partFile.Length, partSize);

            // 新建一个List保存每个分块上传后的ETag和PartNumber
            var partETags = new List<PartETag>();
            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    // 跳到每个分块的开头
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // 计算每个分块的大小
                    var size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // 创建UploadPartRequest，上传分块
                    var uploadPartRequest = new UploadPartRequest(_bucketName, _bigObjectKey, initResult.UploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = (i + 1)
                    };
                    var uploadPartResult = _ossClientDisableMD5.UploadPart(uploadPartRequest);

                    // 将返回的PartETag保存到List中。
                    partETags.Add(uploadPartResult.PartETag);
                }
            }

            // 提交上传任务
            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, _bigObjectKey, initResult.UploadId);
            foreach (var partETag in partETags)
            {
                completeRequest.PartETags.Add(partETag);
            }
            _ossClientDisableMD5.CompleteMultipartUpload(completeRequest);

            // 校验文件内容
            OssTestUtils.DownloadObject(_ossClientDisableMD5, _bucketName, _bigObjectKey, _tmpLocalFile);
            var expectedHashDigest = FileUtils.ComputeContentMd5(Config.MultiUploadTestFile);
            var actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
            Assert.AreEqual(expectedHashDigest, actualHashDigest);

            _ossClientDisableMD5.DeleteObject(_bucketName, _bigObjectKey);
            File.Delete(_tmpLocalFile);
        }
        #endregion

        #region resumable upload
        [Test]
        public void ResumableUploadEnableMD5Test()
        {
            // put little file
            _ossClient.ResumableUploadObject(_bucketName, _objectKey, Config.UploadTestFile, null, null);
            // check content md5
            OssTestUtils.DownloadObject(_ossClient, _bucketName, _objectKey, _tmpLocalFile);
            var expectedHashDigest = FileUtils.ComputeContentMd5(Config.UploadTestFile);
            var actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
            Assert.AreEqual(expectedHashDigest, actualHashDigest);

            _ossClient.DeleteObject(_bucketName, _objectKey);
            File.Delete(_tmpLocalFile);

            // put big file
            _ossClient.ResumableUploadObject(_bucketName, _bigObjectKey, Config.MultiUploadTestFile, null, null, 1024 * 1024);
            // check content md5
            OssTestUtils.DownloadObject(_ossClient, _bucketName, _bigObjectKey, _tmpLocalFile);
            expectedHashDigest = FileUtils.ComputeContentMd5(Config.MultiUploadTestFile);
            actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
            Assert.AreEqual(expectedHashDigest, actualHashDigest);

            _ossClient.DeleteObject(_bucketName, _bigObjectKey);
            File.Delete(_tmpLocalFile);
        }

        [Test]
        public void ResumableUploadDisableMD5Test()
        {
            // put little file
            _ossClientDisableMD5.ResumableUploadObject(_bucketName, _objectKey, Config.UploadTestFile, null, null);
            // check content md5
            OssTestUtils.DownloadObject(_ossClientDisableMD5, _bucketName, _objectKey, _tmpLocalFile);
            var expectedHashDigest = FileUtils.ComputeContentMd5(Config.UploadTestFile);
            var actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
            Assert.AreEqual(expectedHashDigest, actualHashDigest);

            _ossClientDisableMD5.DeleteObject(_bucketName, _objectKey);
            File.Delete(_tmpLocalFile);

            // put big file
            _ossClientDisableMD5.ResumableUploadObject(_bucketName, _bigObjectKey, Config.MultiUploadTestFile, null, null, 1024 * 1024);
            // check content md5
            OssTestUtils.DownloadObject(_ossClientDisableMD5, _bucketName, _bigObjectKey, _tmpLocalFile);
            expectedHashDigest = FileUtils.ComputeContentMd5(Config.MultiUploadTestFile);
            actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
            Assert.AreEqual(expectedHashDigest, actualHashDigest);

            _ossClientDisableMD5.DeleteObject(_bucketName, _bigObjectKey);
            File.Delete(_tmpLocalFile);
        }
        #endregion
    }
}
