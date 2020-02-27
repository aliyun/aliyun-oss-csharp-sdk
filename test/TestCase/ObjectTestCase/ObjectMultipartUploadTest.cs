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
            //upload sample object as source object
            _sourceObjectKey = OssTestUtils.GetObjectKey(_className);
            var metadata = new ObjectMetadata();
            var poResult = OssTestUtils.UploadObject(_ossClient, _bucketName, _sourceObjectKey,
                Config.UploadTestFile, metadata);
            _objectETag = poResult.ETag;
        }

        [OneTimeTearDown]
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
            lmuRequest.EncodingType = null;
            var lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
            string mpUpload = null;
            foreach (var t in lmuListing.MultipartUploads)
            {
                if (t.UploadId == initResult.UploadId)
                    mpUpload = t.UploadId;
            }

            Assert.IsNotNull(mpUpload, "The multipart uploading should be in progress");

            var listRequest = new ListPartsRequest(_bucketName, targetObjectKey, mpUpload);
            listRequest.MaxParts = partCount * 2;
            listRequest.PartNumberMarker = 0;
            PartListing partList = _ossClient.ListParts(listRequest);

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
            string mpUploadInfo = null;
            foreach (var t in lmuListing.MultipartUploads)
            {
                if (t.UploadId == initResult.UploadId)
                {
                    mpUpload = t.UploadId;
                    mpUploadInfo = t.ToString();
                }
            }

            Assert.IsNotNull(mpUpload, "The multipart uploading should be in progress");
            Assert.IsTrue(mpUploadInfo.Contains(mpUpload));

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
            IOss client = OssClientFactory.CreateOssClientEnableMD5(true);

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
                    var uploadPartResult = client.UploadPart(uploadPartRequest);

                    // Save the result
                    partETags.Add(uploadPartResult.PartETag);

                    //list parts which are uploaded
                    var listPartsRequest = new ListPartsRequest(_bucketName, targetObjectKey, initResult.UploadId);
                    listPartsRequest.EncodingType = null;
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

        [Test]
        public void MultipartUploadListMultipartUploadsTest()
        {
            //get target object name
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);
            var initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);

            //get target object name
            targetObjectKey = OssTestUtils.GetObjectKey(_className);
            initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            initResult = _ossClient.InitiateMultipartUpload(initRequest);

            //get target object name
            targetObjectKey = OssTestUtils.GetObjectKey(_className);
            initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            initResult = _ossClient.InitiateMultipartUpload(initRequest);
            var targetObjectKey_3th = targetObjectKey;

            //get target object name
            targetObjectKey = OssTestUtils.GetObjectKey(_className);
            initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            initResult = _ossClient.InitiateMultipartUpload(initRequest);
            var minUploadId = initResult.UploadId;


            var lmuRequest = new ListMultipartUploadsRequest(_bucketName);

            //list 1
            lmuRequest.Prefix = targetObjectKey;
            var lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
            var uploadSumm = OssTestUtils.ToArray<MultipartUpload>(lmuListing.MultipartUploads);
            Assert.AreEqual(1, uploadSumm.Count);

            //list 4
            lmuRequest.Prefix = _className;
            lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
            uploadSumm = OssTestUtils.ToArray<MultipartUpload>(lmuListing.MultipartUploads);
            Assert.AreEqual(4, uploadSumm.Count);

            //list max 2
            lmuRequest.Prefix = _className;
            lmuRequest.MaxUploads = 2;
            lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
            uploadSumm = OssTestUtils.ToArray<MultipartUpload>(lmuListing.MultipartUploads);
            Assert.AreEqual(2, uploadSumm.Count);

            //list max 10， key-marker is the 3th object
            lmuRequest.Prefix = _className;
            lmuRequest.MaxUploads = 10;
            lmuRequest.KeyMarker = targetObjectKey_3th;
            lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
            uploadSumm = OssTestUtils.ToArray<MultipartUpload>(lmuListing.MultipartUploads);
            Assert.AreEqual(1, uploadSumm.Count);
            foreach (var item in lmuListing.MultipartUploads)
            {
                Assert.AreEqual(targetObjectKey, item.Key);
            }

            //get target object name
            initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            initResult = _ossClient.InitiateMultipartUpload(initRequest);
            if (minUploadId.CompareTo(initResult.UploadId) > 0)
            {
                minUploadId = initResult.UploadId;
            }

            //list max 10， set UploadIdMarker 
            lmuRequest.Prefix = _className;
            lmuRequest.MaxUploads = 10;
            lmuRequest.Delimiter = "";
            lmuRequest.KeyMarker = targetObjectKey_3th;
            lmuRequest.UploadIdMarker = "";
            lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
            uploadSumm = OssTestUtils.ToArray<MultipartUpload>(lmuListing.MultipartUploads);
            Assert.AreEqual(2, uploadSumm.Count);

            //list max 10， set UploadIdMarker 
            lmuRequest.Prefix = _className;
            lmuRequest.MaxUploads = 10;
            lmuRequest.Delimiter = "";
            lmuRequest.KeyMarker = targetObjectKey;
            lmuRequest.UploadIdMarker = minUploadId;
            lmuListing = _ossClient.ListMultipartUploads(lmuRequest);
            uploadSumm = OssTestUtils.ToArray<MultipartUpload>(lmuListing.MultipartUploads);
            Assert.AreEqual(1, uploadSumm.Count);
        }

        [Test]
        public void MultipartUploadUploadPartRequestArgumentCheck()
        {
            try
            {
                var uploadPartRequest = new UploadPartRequest(_bucketName, "", "")
                {
                    PartNumber = null,
                    PartSize = null,
                    InputStream = null
                };
                var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("The parameter is empty or null"));
            }

            try
            {
                var uploadPartRequest = new UploadPartRequest(_bucketName, "test_object", "")
                {
                    PartNumber = null,
                    PartSize = null,
                    InputStream = null
                };
                var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("uploadId should be specified"));
            }

            try
            {
                var uploadPartRequest = new UploadPartRequest(_bucketName, "test_object", "test-upload-id")
                {
                    PartNumber = null,
                    PartSize = null,
                    InputStream = null
                };
                var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("partNumber should be specified"));
            }

            try
            {
                var uploadPartRequest = new UploadPartRequest(_bucketName, "test_object", "test-upload-id")
                {
                    PartNumber = 1,
                    PartSize = null,
                    InputStream = null
                };
                var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("partSize should be specified"));
            }

            try
            {
                var uploadPartRequest = new UploadPartRequest(_bucketName, "test_object", "test-upload-id")
                {
                    PartNumber = 1,
                    PartSize = 1024*1024*5,
                    InputStream = null
                };
                var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("inputStream should be specified"));
            }

            try
            {
                var uploadPartRequest = new UploadPartRequest(_bucketName, "test_object", "test-upload-id")
                {
                    PartNumber = 1,
                    PartSize = -1,
                    InputStream = new MemoryStream(new byte[0])
                };
                var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("partSize not live in valid range"));
            }

            try
            {
                var uploadPartRequest = new UploadPartRequest(_bucketName, "test_object", "test-upload-id")
                {
                    PartNumber = 1,
                    PartSize = 5 * 1024 * 1024 * 1024L + 1L,
                    InputStream = new MemoryStream(new byte[0])
                };
                var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("partSize not live in valid range"));
            }

            try
            {
                var uploadPartRequest = new UploadPartRequest(_bucketName, "test_object", "test-upload-id")
                {
                    PartNumber = 10001,
                    PartSize = 1024 * 1024 * 5,
                    InputStream = new MemoryStream(new byte[0])
                };
                var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("partNumber not live in valid range"));
            }
        }

        [Test]
        public void MultipartUploadUploadPartCopyRequestArgumentCheck()
        {
            try
            {
                var uploadPartCopyRequest = new UploadPartCopyRequest("targetbucket",
                    "targetKey", "sourcebucket", "sourceKey", "upload-id")
                {
                    PartNumber = null,
                    PartSize = null,
                    BeginIndex = null
                };
                var uploadPartResult = _ossClient.UploadPartCopy(uploadPartCopyRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("partNumber should be specfied"));
            }

            try
            {
                var uploadPartCopyRequest = new UploadPartCopyRequest("targetbucket",
                    "targetKey", "sourcebucket", "sourceKey", "upload-id")
                {
                    PartNumber = 1,
                    PartSize = null,
                    BeginIndex = null
                };
                var uploadPartResult = _ossClient.UploadPartCopy(uploadPartCopyRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("partSize should be specfied"));
            }

            try
            {
                var uploadPartCopyRequest = new UploadPartCopyRequest("targetbucket",
                    "targetKey", "sourcebucket", "sourceKey", "upload-id")
                {
                    PartNumber = 1,
                    PartSize = 1024*1024*5,
                    BeginIndex = null
                };
                var uploadPartResult = _ossClient.UploadPartCopy(uploadPartCopyRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("beginIndex should be specfied"));
            }

            try
            {
                var uploadPartCopyRequest = new UploadPartCopyRequest("targetbucket",
                    "targetKey", "sourcebucket", "sourceKey", "upload-id")
                {
                    PartNumber = 1,
                    PartSize = -1,
                    BeginIndex = 1
                };
                var uploadPartResult = _ossClient.UploadPartCopy(uploadPartCopyRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("partSize not live in valid range"));
            }

            try
            {
                var uploadPartCopyRequest = new UploadPartCopyRequest("targetbucket",
                    "targetKey", "sourcebucket", "sourceKey", "upload-id")
                {
                    PartNumber = 1,
                    PartSize = 5 * 1024 * 1024 * 1024L + 1L,
                    BeginIndex = 1
                };
                var uploadPartResult = _ossClient.UploadPartCopy(uploadPartCopyRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("partSize not live in valid range"));
            }

            try
            {
                var uploadPartCopyRequest = new UploadPartCopyRequest("targetbucket",
                    "targetKey", "sourcebucket", "sourceKey", "upload-id")
                {
                    PartNumber = 10001,
                    PartSize = 1024 * 1024 * 5,
                    BeginIndex = 1
                };
                var uploadPartResult = _ossClient.UploadPartCopy(uploadPartCopyRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("partNumber not live in valid range"));
            }
        }

        [Test]
        public void CompleteMultipartUploadRequestArgumentCheck()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            try
            {
                var completeRequest = new CompleteMultipartUploadRequest(_bucketName, targetObjectKey, null);
                _ossClient.CompleteMultipartUpload(completeRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("uploadId"));
            }
        }

        [Test]
        public void AbortMultipartUploadRequestArgumentCheck()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            try
            {
                var abortRequest = new AbortMultipartUploadRequest(_bucketName, targetObjectKey, null);
                _ossClient.AbortMultipartUpload(abortRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("uploadId"));
            }
        }

        [Test]
        public void ListPartsRequestArgumentCheck()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            try
            {
                var listRequest = new ListPartsRequest(_bucketName, targetObjectKey, null);
                _ossClient.ListParts(listRequest);
                Assert.Fail("the arg is null, should throw exception.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("uploadId"));
            }
        }

        [Test]
        public void MultipartUploadPartCopyNGTest()
        {
            //get target object name
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            try
            {
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
                            UnmodifiedSinceConstraint = DateTime.Now.AddDays(-1)
                        };
                    uploadPartCopyRequest.NonmatchingETagConstraints.Add(_objectETag);
                    var uploadPartCopyResult = _ossClient.UploadPartCopy(uploadPartCopyRequest);

                    // Save the result
                    partETags.Add(uploadPartCopyResult.PartETag);
                }
                Assert.IsTrue(false);
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "PreconditionFailed");
            }
  
            //delete the object
            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }
    }
}
