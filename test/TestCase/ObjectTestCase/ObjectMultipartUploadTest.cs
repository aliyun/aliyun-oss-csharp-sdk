using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Common;
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
                Config.UploadTestFile, metadata);
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
            MultipartUploadComplexStepTest(_ossClient);
        }

        [Test]
        public void MultipartUploadComplexStepTestWithWrongCrc()
        {
            try
            {
                MultipartUploadComplexStepTest(_ossClient, true);
                Assert.Fail();
            }
            catch(ClientException e)
            {
                Assert.IsTrue(e.Message.Contains("Crc64"));
            }
        }

        [Test]
        public void MultipartUploadComplexStepTestWithoutCrc()
        {
            Common.ClientConfiguration config = new Common.ClientConfiguration();
            config.EnableCrcCheck = false;
            IOss ossClient = OssClientFactory.CreateOssClient(config);
            MultipartUploadComplexStepTest(ossClient);
        }

        [Test]
        public void CompleteMultipartUploadWithListParts()
        {
            var sourceFile = Config.MultiUploadTestFile;
            //get target object name
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);

            // Set the part size 
            const int partSize = 1024 * 1024 * 1;

            var partFile = new FileInfo(sourceFile);
            // Calculate the part count
            var partCount = OssTestUtils.CalculatePartCount(partFile.Length, partSize);

            LogUtility.LogMessage("File {0} is splitted to {1} parts for multipart upload",
                sourceFile, partCount);

            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    // Skip to the start position
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // calculate the part size
                    var size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // Create a UploadPartRequest, uploading parts
                    var uploadPartRequest = new UploadPartRequest(_bucketName, targetObjectKey, initResult.UploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = (i + 1)
                    };
                    _ossClient.UploadPart(uploadPartRequest);
                }
            }

            var lmuRequest = new ListMultipartUploadsRequest(_bucketName);
            var lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
            string mpUpload = null;
            foreach (var t in lmuListing.MultipartUploads)
            {
                if (t.UploadId == initResult.UploadId)
                    mpUpload = t.UploadId;
            }

            Assert.IsNotNull(mpUpload, "The multipart uploading should be in progress");

            PartListing partList = _ossClient.ListParts(new ListPartsRequest(_bucketName, targetObjectKey, mpUpload));

            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, targetObjectKey, initResult.UploadId);
            foreach (var part in partList.Parts)
            {
                completeRequest.PartETags.Add(part.PartETag);

            }
            _ossClient.CompleteMultipartUpload(completeRequest);

            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

            //delete the object
            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }

        private void MultipartUploadComplexStepTest(IOss ossClient, bool overrideCrc = false)
        {
            var sourceFile = Config.MultiUploadTestFile;
            //get target object name
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            var initResult = ossClient.InitiateMultipartUpload(initRequest);

            // Set the part size 
            const int partSize = 1024 * 1024 * 1;

            var partFile = new FileInfo(sourceFile);
            // Calculate the part count
            var partCount = OssTestUtils.CalculatePartCount(partFile.Length, partSize);

            LogUtility.LogMessage("File {0} is splitted to {1} parts for multipart upload",
                sourceFile, partCount);

            // Create a list to save result 
            var partETags = new List<PartETag>();
            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    // Skip to the start position
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // calculate the part size
                    var size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // Create a UploadPartRequest, uploading parts
                    var uploadPartRequest = new UploadPartRequest(_bucketName, targetObjectKey, initResult.UploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = (i + 1)
                    };
                    var uploadPartResult = ossClient.UploadPart(uploadPartRequest);

                    // Save the result
                    partETags.Add(uploadPartResult.PartETag);

                    Assert.AreNotEqual(uploadPartResult.Crc64, 0);
                    Assert.AreNotEqual(uploadPartResult.Length, 0);
                }
            }

            var lmuRequest = new ListMultipartUploadsRequest(_bucketName);
            var lmuListing = ossClient.ListMultipartUploads(lmuRequest);
            string mpUpload = null;
            foreach (var t in lmuListing.MultipartUploads)
            {
                if (t.UploadId == initResult.UploadId)
                    mpUpload = t.UploadId;
            }

            Assert.IsNotNull(mpUpload, "The multipart uploading should be in progress");

            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, targetObjectKey, initResult.UploadId);
            foreach (var partETag in partETags)
            {
                if (overrideCrc)
                {
                    partETag.Crc64 = "0";
                }

                completeRequest.PartETags.Add(partETag);

            }
            ossClient.CompleteMultipartUpload(completeRequest);

            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

            //delete the object
            ossClient.DeleteObject(_bucketName, targetObjectKey);
        }

        [Test]
        public void MultipartUploadAbortInMiddleTest()
        {
            var sourceFile = Config.MultiUploadTestFile;
            //get target object name
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);

            // Set the part size 
            const int partSize = 1024 * 1024 * 1;

            var partFile = new FileInfo(sourceFile);
            // Calculate the part count
            var partCount = OssTestUtils.CalculatePartCount(partFile.Length, partSize);

            Assert.IsTrue(partCount > 1, "Source file is too small to perform multipart upload");
            LogUtility.LogMessage("File {0} is splitted to {1} parts for multipart upload",
                sourceFile, partCount);

             // Create a list to save result 
            var partETags = new List<PartETag>();
            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                //use partCount - 1, so that the last part is left
                for (var i = 0; i < partCount - 1; i++)
                {
                    // Skip to the start position
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // calculate the part size
                    var size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                      // Create a UploadPartRequest, uploading parts
                    var uploadPartRequest = new UploadPartRequest(_bucketName, targetObjectKey, initResult.UploadId);
                    uploadPartRequest.InputStream = fs;
                    uploadPartRequest.PartSize = size;
                    uploadPartRequest.PartNumber = (i + 1);
                    var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);

                    // Save the result
                    partETags.Add(uploadPartResult.PartETag);

                    //list parts which are uploaded
                    var listPartsRequest = new ListPartsRequest(_bucketName, targetObjectKey, initResult.UploadId);
                    var listPartsResult = _ossClient.ListParts(listPartsRequest);
                    //there should be only 1 part was not uploaded
                    Assert.AreEqual(i + 1, OssTestUtils.ToArray<Part>(listPartsResult.Parts).Count, "uploaded parts is not expected");
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

            // Set the part size 
            const int partSize = 1024 * 512 * 1;

            var sourceObjectMeta = _ossClient.GetObjectMetadata(_bucketName, _sourceObjectKey);
            // Calculate the part count
            var partCount = OssTestUtils.CalculatePartCount(sourceObjectMeta.ContentLength, partSize);

            LogUtility.LogMessage("Object {0} is splitted to {1} parts for multipart upload part copy",
                _sourceObjectKey, partCount);

            // Create a list to save result 
            var partETags = new List<PartETag>();

            for (var i = 0; i < partCount; i++)
            {
                // Skip to the start position
                long skipBytes = partSize * i;

               // calculate the part size
                var size = partSize < sourceObjectMeta.ContentLength - skipBytes
                    ? partSize
                    : sourceObjectMeta.ContentLength - skipBytes;

                // Create a UploadPartRequest, uploading parts
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

                // Save the result
                partETags.Add(uploadPartCopyResult.PartETag);
            }

            var lmuRequest = new ListMultipartUploadsRequest(_bucketName);
            var lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
           
            string mpUpload = null;
            foreach (var t in lmuListing.MultipartUploads)
            {
                if (t.UploadId == initResult.UploadId)
                {
                    mpUpload = t.UploadId;
                    break;
                }
            }

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
