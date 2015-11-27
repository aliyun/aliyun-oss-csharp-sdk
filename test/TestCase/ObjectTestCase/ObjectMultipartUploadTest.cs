using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aliyun.OSS;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectMultipartUploadTest
    {

        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _sourceObjectKey;
        private static string _objectETag;

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
            //upload sample object as source object
            _sourceObjectKey = OssTestUtils.GetObjectKey(_className);
            var metadata = new ObjectMetadata();
            var poResult = OssTestUtils.UploadObject(_ossClient, _bucketName, _sourceObjectKey,
                Config.UploadSampleFile, metadata);
            _objectETag = poResult.ETag;
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void MultipartUploadComplexStepTest()
        {
            var sourceFile = Config.MultiUploadSampleFile;
            //get target object name
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);

            // 设置每块为 1M
            const int partSize = 1024 * 1024 * 1;

            var partFile = new FileInfo(sourceFile);
            // 计算分块数目
            var partCount = OssTestUtils.CalculatePartCount(partFile.Length, partSize);

            LogUtility.LogMessage("File {0} is splitted to {1} parts for multipart upload",
                sourceFile, partCount);

            // 新建一个List保存每个分块上传后的ETag和PartNumber
            var partETags = new List<PartETag>();
            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    // 跳到每个分块的开头
                    long skipBytes = partSize*i;
                    fs.Position = skipBytes;

                    // 计算每个分块的大小
                    var size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // 创建UploadPartRequest，上传分块
                    var uploadPartRequest = new UploadPartRequest(_bucketName, targetObjectKey, initResult.UploadId)
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

            var lmuRequest = new ListMultipartUploadsRequest(_bucketName);
            var lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
            var mpUpload = lmuListing.MultipartUploads.Single(t => t.UploadId == initResult.UploadId);
            Assert.IsNotNull(mpUpload, "The multipart uploading should be in progress");

            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, targetObjectKey, initResult.UploadId);
            foreach (var partETag in partETags)
            {
                completeRequest.PartETags.Add(partETag);
            }
            _ossClient.CompleteMultipartUpload(completeRequest);
            
            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

            //delete the object
            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }

        [Test]
        public void MultipartUploadAbortInMiddleTest()
        {
            var sourceFile = Config.MultiUploadSampleFile;
            //get target object name
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);

            // 设置每块为 1M
            const int partSize = 1024 * 1024 * 1;

            var partFile = new FileInfo(sourceFile);
            // 计算分块数目
            var partCount = OssTestUtils.CalculatePartCount(partFile.Length, partSize);

            Assert.IsTrue(partCount > 1, "Source file is too small to perform multipart upload");
            LogUtility.LogMessage("File {0} is splitted to {1} parts for multipart upload",
                sourceFile, partCount);

            // 新建一个List保存每个分块上传后的ETag和PartNumber
            var partETags = new List<PartETag>();
            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                //use partCount - 1, so that the last part is left
                for (var i = 0; i < partCount - 1; i++)
                {
                    // 跳到每个分块的开头
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // 计算每个分块的大小
                    var size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // 创建UploadPartRequest，上传分块
                    var uploadPartRequest = new UploadPartRequest(_bucketName, targetObjectKey, initResult.UploadId);
                    uploadPartRequest.InputStream = fs;
                    uploadPartRequest.PartSize = size;
                    uploadPartRequest.PartNumber = (i + 1);
                    var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);

                    // 将返回的PartETag保存到List中。
                    partETags.Add(uploadPartResult.PartETag);

                    //list parts which are uploaded
                    var listPartsRequest = new ListPartsRequest(_bucketName, targetObjectKey, initResult.UploadId);
                    var listPartsResult = _ossClient.ListParts(listPartsRequest);
                    //there should be only 1 part was not uploaded
                    Assert.AreEqual(i + 1, listPartsResult.Parts.Count(), "uploaded parts is not expected");
                }
            }
            //abort the upload
            var abortRequest = new AbortMultipartUploadRequest(_bucketName, targetObjectKey, initResult.UploadId);
            _ossClient.AbortMultipartUpload(abortRequest);
        }

        [Test]
        public void MultipartUploadPartCopyComplexStepTest()
        {
            //get target object name
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);

            // 设置每块为 512K
            const int partSize = 1024 * 512 * 1;

            var sourceObjectMeta = _ossClient.GetObjectMetadata(_bucketName, _sourceObjectKey);
            // 计算分块数目
            var partCount = OssTestUtils.CalculatePartCount(sourceObjectMeta.ContentLength, partSize);

            LogUtility.LogMessage("Object {0} is splitted to {1} parts for multipart upload part copy",
                _sourceObjectKey, partCount);

            // 新建一个List保存每个分块上传后的ETag和PartNumber
            var partETags = new List<PartETag>();

            for (var i = 0; i < partCount; i++)
            {
                // 跳到每个分块的开头
                long skipBytes = partSize * i;

                // 计算每个分块的大小
                var size = partSize < sourceObjectMeta.ContentLength - skipBytes
                    ? partSize
                    : sourceObjectMeta.ContentLength - skipBytes;

                // 创建UploadPartRequest，上传分块
                var uploadPartCopyRequest = 
                    new UploadPartCopyRequest(_bucketName, targetObjectKey, _bucketName, _sourceObjectKey, initResult.UploadId)
                    {
                        BeginIndex = skipBytes,
                        PartSize = size,
                        PartNumber = (i + 1),
                        ModifiedSinceConstraint = DateTime.Now.AddDays(-1)
                    };
                uploadPartCopyRequest.MatchingETagConstraints.Add(_objectETag);
                var uploadPartCopyResult = _ossClient.UploadPartCopy(uploadPartCopyRequest);

                // 将返回的PartETag保存到List中。
                partETags.Add(uploadPartCopyResult.PartETag);
            }

            var lmuRequest = new ListMultipartUploadsRequest(_bucketName);
            var lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
            var mpUpload = lmuListing.MultipartUploads.Single(t => t.UploadId == initResult.UploadId);
            Assert.IsNotNull(mpUpload, "The multipart uploading should be in progress");

            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, targetObjectKey, initResult.UploadId);
            foreach (var partETag in partETags)
            {
                completeRequest.PartETags.Add(partETag);
            }
            _ossClient.CompleteMultipartUpload(completeRequest);

            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

            //delete the object
            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }
    }
}
