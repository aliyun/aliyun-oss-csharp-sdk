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
    public class ObjectProgressTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _objectKey;
        private static string _bigObjectKey;
        private static string _tmpLocalFile;

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
        public void PutObjectProgressTest()
        {
            // put object
            using (var inputStream = File.Open(Config.MultiUploadTestFile, FileMode.Open))
            {
                var putObjectRequest = new PutObjectRequest(_bucketName, _objectKey, inputStream);
                putObjectRequest.Metadata = new ObjectMetadata();
                putObjectRequest.StreamTransferProgress += uploadProgressCallback;
                _ossClient.PutObject(putObjectRequest);
                inputStream.Close();
            }

            // get object
            var getObjectRequest = new GetObjectRequest(_bucketName, _objectKey);
            getObjectRequest.StreamTransferProgress += uploadProgressCallback;
            var ossObject = _ossClient.GetObject(getObjectRequest);
            using (var requestStream = ossObject.Content)
            {
                using (var localfile = File.Open(_tmpLocalFile, FileMode.OpenOrCreate))
                {
                    OssTestUtils.WriteTo(requestStream, localfile);
                }
            }

            // check md5
            var expectedHashDigest = FileUtils.ComputeContentMd5(Config.MultiUploadTestFile);
            var actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
            Assert.AreEqual(expectedHashDigest, actualHashDigest);

            _ossClient.DeleteObject(_bucketName, _objectKey);
            File.Delete(_tmpLocalFile);
        }
        #endregion

        #region append object
        [Test]
        public void AppendObjectProgressTest()
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
                        Position = position,
                    };
                    request.StreamTransferProgress += uploadProgressCallback;

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
                    request.StreamTransferProgress += uploadProgressCallback;

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
        #endregion

        #region multipart upload
        [Test]
        public void MultipartUploadProgressTest()
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
                    uploadPartRequest.StreamTransferProgress += uploadProgressCallback;
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
        #endregion

        #region resumable upload
        [Test]
        public void ResumableUploadObjectProgressTest()
        {
            var metadata = new ObjectMetadata();
            // resumable upload
            _ossClient.ResumableUploadObject(_bucketName, _objectKey, Config.MultiUploadTestFile, metadata, null, 102410, uploadProgressCallback);

            // check md5
            OssTestUtils.DownloadObject(_ossClient, _bucketName, _objectKey, _tmpLocalFile);
            var expectedHashDigest = FileUtils.ComputeContentMd5(Config.MultiUploadTestFile);
            var actualHashDigest = FileUtils.ComputeContentMd5(_tmpLocalFile);
            Assert.AreEqual(expectedHashDigest, actualHashDigest);

            _ossClient.DeleteObject(_bucketName, _bigObjectKey);
            File.Delete(_tmpLocalFile);
        }
        #endregion

        #region private
        private void uploadProgressCallback(object sender, StreamTransferProgressArgs args)
        {
            System.Console.WriteLine("UploadProgressCallback - TotalBytes:{0}, TransferredBytes:{1}",
                args.TotalBytes, args.TransferredBytes);
        }
        #endregion
    }
}
