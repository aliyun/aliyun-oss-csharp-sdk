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
using Aliyun.OSS.Model;
using System.Threading;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectRequestPaymentTest
    {
        private static IOss _ossClient;
        private static IOss _ossPayerClient;
        private static string _className;
        private static string _bucketName;
        private static string _archiveBucketName;

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            _ossPayerClient = new OssClient(Config.Endpoint, Config.PayerAccessKeyId, Config.PayerAccessKeySecret);

            //get current class name, which is prefix of bucket/object
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName(_className);
            _ossClient.CreateBucket(_bucketName);

            string policy = "{\"Version\":\"1\",\"Statement\":[{\"Action\":[\"oss:*\"],\"Effect\": \"Allow\"," +
               "\"Principal\":[\"" + Config.PayerUid + "\"]," +
               "\"Resource\": [\"acs:oss:*:*:" + _bucketName + "\",\"acs:oss:*:*:" + _bucketName + "/*\"]}]}";

            _ossClient.SetBucketPolicy(new SetBucketPolicyRequest(_bucketName, policy));
            _ossClient.SetBucketRequestPayment(new SetBucketRequestPaymentRequest(_bucketName, RequestPayer.Requester));


            _archiveBucketName = _bucketName + "archive";
            _ossClient.CreateBucket(_bucketName);
            _ossClient.CreateBucket(_archiveBucketName, StorageClass.Archive);
            policy = "{\"Version\":\"1\",\"Statement\":[{\"Action\":[\"oss:*\"],\"Effect\": \"Allow\"," +
               "\"Principal\":[\"" + Config.PayerUid + "\"]," +
               "\"Resource\": [\"acs:oss:*:*:" + _archiveBucketName + "\",\"acs:oss:*:*:" + _archiveBucketName + "/*\"]}]}";
            _ossClient.SetBucketPolicy(new SetBucketPolicyRequest(_archiveBucketName, policy));
            _ossClient.SetBucketRequestPayment(new SetBucketRequestPaymentRequest(_archiveBucketName, RequestPayer.Requester));
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
            OssTestUtils.CleanBucket(_ossClient, _archiveBucketName);
        }

        [Test]
        public void ObjectBasicTest()
        {
            //Get Case
            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, key, new MemoryStream(Encoding.ASCII.GetBytes("hello world")));

            var getRequest = new GetObjectRequest(_bucketName, key);
            Assert.AreEqual(getRequest.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.GetObject(getRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            getRequest.RequestPayer = RequestPayer.Requester;
            var getResult = _ossPayerClient.GetObject(getRequest);
            Assert.AreEqual(getResult.ContentLength, 11);
            _ossClient.DeleteObject(_bucketName, key);

            //Put Case
            key = OssTestUtils.GetObjectKey(_className);
            var content = new MemoryStream(Encoding.ASCII.GetBytes("hello world"));
            var putRequest = new PutObjectRequest(_bucketName, key, content);
            Assert.AreEqual(putRequest.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.PutObject(putRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            content = new MemoryStream(Encoding.ASCII.GetBytes("hello world"));
            putRequest = new PutObjectRequest(_bucketName, key, content);
            putRequest.RequestPayer = RequestPayer.Requester;
            var putResult = _ossPayerClient.PutObject(putRequest);
            Assert.AreEqual(putResult.HttpStatusCode, HttpStatusCode.OK);

            //head object
            var headRequest = new GetObjectMetadataRequest(_bucketName, key);
            Assert.AreEqual(headRequest.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.GetObjectMetadata(headRequest);
                Assert.Fail("should not here.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            headRequest.RequestPayer = RequestPayer.Requester;
            var headResult = _ossPayerClient.GetObjectMetadata(headRequest);
            Assert.AreEqual(headResult.ContentLength, 11);

            //Delete Case
            Assert.IsTrue(_ossClient.DoesObjectExist(_bucketName, key));

            var delReqeust = new DeleteObjectRequest(_bucketName, key);
            Assert.AreEqual(delReqeust.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.DeleteObject(delReqeust);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            delReqeust = new DeleteObjectRequest(_bucketName, key);
            delReqeust.RequestPayer = RequestPayer.Requester;
            _ossPayerClient.DeleteObject(delReqeust);

            Assert.IsFalse(_ossClient.DoesObjectExist(_bucketName, key));

            //delete objects
            var keys = new List<string>();
            keys.Add(key);
            var delsReqeust = new DeleteObjectsRequest(_bucketName, keys);
            Assert.AreEqual(delsReqeust.RequestPayer, RequestPayer.BucketOwner);
            try
            {
                _ossPayerClient.DeleteObjects(delsReqeust);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            delsReqeust.RequestPayer = RequestPayer.Requester;
            _ossPayerClient.DeleteObjects(delsReqeust);

            //delete object versions
            var objects = new List<ObjectIdentifier>();
            objects.Add(new ObjectIdentifier(key));
            var delvsReqeust = new DeleteObjectVersionsRequest(_bucketName, objects);
            Assert.AreEqual(delvsReqeust.RequestPayer, RequestPayer.BucketOwner);
            try
            {
                _ossPayerClient.DeleteObjectVersions(delvsReqeust);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            delvsReqeust.RequestPayer = RequestPayer.Requester;
            _ossPayerClient.DeleteObjectVersions(delvsReqeust);


            //list objets
            var lsRequest = new ListObjectsRequest(_bucketName);
            Assert.AreEqual(lsRequest.RequestPayer, RequestPayer.BucketOwner);
            try
            {
                _ossPayerClient.ListObjects(lsRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            lsRequest.RequestPayer = RequestPayer.Requester;
            _ossPayerClient.ListObjects(lsRequest);
        }

        [Test]
        public void MultipartUploadNegativeTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            // Initiate a multipart upload
            var initRequest = new InitiateMultipartUploadRequest(_bucketName, key);
            Assert.AreEqual(initRequest.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                var initResult = _ossPayerClient.InitiateMultipartUpload(initRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            // uploads the part
            var uploadPartRequest = new UploadPartRequest(_bucketName, key, "initId")
            {
                InputStream = new MemoryStream(Encoding.ASCII.GetBytes("hello world")),
                PartSize = 11,
                PartNumber = 1
            };
            Assert.AreEqual(uploadPartRequest.RequestPayer, RequestPayer.BucketOwner);
            try
            {
                var uploadPartResult = _ossPayerClient.UploadPart(uploadPartRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            //list part
            var listpartRequest = new ListPartsRequest(_bucketName, key, "initId");
            Assert.AreEqual(listpartRequest.RequestPayer, RequestPayer.BucketOwner);
            try
            {
                var listPartResult = _ossPayerClient.ListParts(listpartRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            //abort 
            var abortRequest = new AbortMultipartUploadRequest(_bucketName, key, "initId");
            Assert.AreEqual(abortRequest.RequestPayer, RequestPayer.BucketOwner);
            try
            {
                _ossPayerClient.AbortMultipartUpload(abortRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            //complete
            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, key, "initId");
            Assert.AreEqual(completeRequest.RequestPayer, RequestPayer.BucketOwner);
            try
            {
                var listPartResult = _ossPayerClient.CompleteMultipartUpload(completeRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            //list upload 
            var listUploadsRequest = new ListMultipartUploadsRequest(_bucketName);
            Assert.AreEqual(listUploadsRequest.RequestPayer, RequestPayer.BucketOwner);
            try
            {
                var listUploadsResult = _ossPayerClient.ListMultipartUploads(listUploadsRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            //uploadpart copy
            var uploadpartcopyRequet = new UploadPartCopyRequest(_bucketName, key, _bucketName, key, "uploadId")
            {
                BeginIndex = 0,
                PartNumber = 1,
                PartSize = 100 * 1024
            };
            Assert.AreEqual(uploadpartcopyRequet.RequestPayer, RequestPayer.BucketOwner);
            try
            {
                var uploadpartcopyResult = _ossPayerClient.UploadPartCopy(uploadpartcopyRequet);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }
        }

        [Test]
        public void MultipartUploadPositiveTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            // Initiate a multipart upload
            var initRequest = new InitiateMultipartUploadRequest(_bucketName, key);
            initRequest.RequestPayer = RequestPayer.Requester;
            var initResult = _ossPayerClient.InitiateMultipartUpload(initRequest);

            // Sets the part size as 100K
            const int partSize = 100 * 1024;
            var partFile = new FileInfo(Config.UploadTestFile);
            // Calculates the part count
            var partCount = OssTestUtils.CalculatePartCount(partFile.Length, partSize);

            // creates a list of PartETag
            var partETags = new List<PartETag>();
            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    // skip to the start of each part
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // calculates the part size 
                    var size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // uploads the part
                    var uploadPartRequest = new UploadPartRequest(_bucketName, key, initResult.UploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = (i + 1)
                    };
                    uploadPartRequest.RequestPayer = RequestPayer.Requester;
                    var uploadPartResult = _ossPayerClient.UploadPart(uploadPartRequest);

                    // adds the result to the list 
                    partETags.Add(uploadPartResult.PartETag);
                }
            }

            var listpartRequest = new ListPartsRequest(_bucketName, key, initResult.UploadId);
            listpartRequest.RequestPayer = RequestPayer.Requester;

            var listpartResult = _ossPayerClient.ListParts(listpartRequest);
            Assert.AreEqual(listpartResult.UploadId, initResult.UploadId);

            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, key, initResult.UploadId);
            completeRequest.RequestPayer = RequestPayer.Requester;
            foreach (var partETag in partETags)
            {
                completeRequest.PartETags.Add(partETag);
            }
            var completeMultipartUploadResult = _ossPayerClient.CompleteMultipartUpload(completeRequest);

            //delete the object
            string contentCRC;
            using (var fs = new FileStream(Config.UploadTestFile, FileMode.Open))
            {
                contentCRC = OssUtils.ComputeContentCrc64(fs, fs.Length);
            }
            var getResult = _ossClient.GetObject(_bucketName, key);
            Assert.AreEqual(getResult.Metadata.Crc64, contentCRC);
            _ossClient.DeleteObject(_bucketName, key);

            //listuploads
            // Initiate a multipart upload
            initRequest = new InitiateMultipartUploadRequest(_bucketName, key);
            initResult = _ossClient.InitiateMultipartUpload(initRequest);
            var uploadPartRequest1 = new UploadPartRequest(_bucketName, key, initResult.UploadId)
            {
                InputStream = new MemoryStream(Encoding.ASCII.GetBytes("hello world")),
                PartSize = 11,
                PartNumber = 1
            };
            _ossClient.UploadPart(uploadPartRequest1);

            var listUploadsRequest = new ListMultipartUploadsRequest(_bucketName);
            listUploadsRequest.RequestPayer = RequestPayer.Requester;
            listUploadsRequest.Prefix = key;
            var listUploadsResult = _ossPayerClient.ListMultipartUploads(listUploadsRequest);
            int cnt = 0;
            foreach (var multipartUpload in listUploadsResult.MultipartUploads)
            {
                Assert.AreEqual(multipartUpload.UploadId, initResult.UploadId);
                Assert.AreEqual(multipartUpload.Key, key);
                cnt++;
            }
            Assert.AreEqual(cnt, 1);

            //abort
            var abortRequest = new AbortMultipartUploadRequest(_bucketName, key, initResult.UploadId);
            abortRequest.RequestPayer = RequestPayer.Requester;
            _ossPayerClient.AbortMultipartUpload(abortRequest);


            listUploadsRequest = new ListMultipartUploadsRequest(_bucketName);
            listUploadsRequest.Prefix = key;
            listUploadsResult = _ossClient.ListMultipartUploads(listUploadsRequest);
            cnt = 0;
            foreach (var multipartUpload in listUploadsResult.MultipartUploads)
            {
                Assert.AreEqual(multipartUpload.UploadId, initResult.UploadId);
                Assert.AreEqual(multipartUpload.Key, key);
                cnt++;
            }
            Assert.AreEqual(cnt, 0);
        }

        [Test]
        public void MultipartUploadCopyTest()
        {
            //get target object name
            var sourceObjectKey = OssTestUtils.GetObjectKey(_className);
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            OssTestUtils.UploadObject(_ossClient, _bucketName, sourceObjectKey, Config.UploadTestFile);

            var initRequest = new InitiateMultipartUploadRequest(_bucketName, targetObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);

            // Set the part size 
            const int partSize = 100*1024;

            var sourceObjectMeta = _ossClient.GetObjectMetadata(_bucketName, sourceObjectKey);
            // Calculate the part count
            var partCount = OssTestUtils.CalculatePartCount(sourceObjectMeta.ContentLength, partSize);

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
                    new UploadPartCopyRequest(_bucketName, targetObjectKey, _bucketName, sourceObjectKey, initResult.UploadId)
                    {
                        BeginIndex = skipBytes,
                        PartSize = size,
                        PartNumber = (i + 1),
                        ModifiedSinceConstraint = DateTime.Now.AddDays(-1),
                        RequestPayer = RequestPayer.Requester
                    };
                uploadPartCopyRequest.MatchingETagConstraints.Add(sourceObjectMeta.ETag);
                var uploadPartCopyResult = _ossPayerClient.UploadPartCopy(uploadPartCopyRequest);

                // Save the result
                partETags.Add(uploadPartCopyResult.PartETag);
            }

            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, targetObjectKey, initResult.UploadId);
            foreach (var partETag in partETags)
            {
                completeRequest.PartETags.Add(partETag);
            }
            _ossClient.CompleteMultipartUpload(completeRequest);

            var targetObjectMeta = _ossClient.GetObjectMetadata(_bucketName, targetObjectKey);
            Assert.AreEqual(targetObjectMeta.Crc64, sourceObjectMeta.Crc64);

            //delete the object
            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }

        [Test]
        public void CopyObjectTest()
        {
            var sourceObjectKey = OssTestUtils.GetObjectKey(_className);
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);
            OssTestUtils.UploadObject(_ossClient, _bucketName, sourceObjectKey, Config.UploadTestFile);

            //construct metadata
            var metadata = new ObjectMetadata();
            const string userMetaKey = "myKey";
            const string userMetaValue = "myValue";
            metadata.UserMetadata.Add(userMetaKey, userMetaValue);
            metadata.CacheControl = "No-Cache";

            var coRequest = new CopyObjectRequest(_bucketName, sourceObjectKey, _bucketName, targetObjectKey)
            {
                NewObjectMetadata = metadata
            };
            Assert.AreEqual(coRequest.RequestPayer, RequestPayer.BucketOwner);
            //copy object
            try
            {
                var result = _ossPayerClient.CopyObject(coRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            coRequest.RequestPayer = RequestPayer.Requester;
            var copyResult = _ossPayerClient.CopyObject(coRequest);

            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));
            var resMetadata = _ossClient.GetObjectMetadata(_bucketName, targetObjectKey);
            Assert.AreEqual(userMetaValue, resMetadata.UserMetadata[userMetaKey]);

            _ossClient.DeleteObject(_bucketName, targetObjectKey);
            _ossClient.DeleteObject(_bucketName, sourceObjectKey);
        }

        [Test]
        public void AppendObjectTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            var request = new AppendObjectRequest(_bucketName, key);
            Assert.AreEqual(request.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                request = new AppendObjectRequest(_bucketName, key)
                {
                    Content = new MemoryStream(Encoding.ASCII.GetBytes("hello world")),
                    Position = 0
                };
                var result = _ossPayerClient.AppendObject(request);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            try
            {
                long position = 0;
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    request = new AppendObjectRequest(_bucketName, key)
                    {
                        Content = fs,
                        Position = position,
                        RequestPayer = RequestPayer.Requester
                    };

                    var result = _ossPayerClient.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                    position = result.NextAppendPosition;
                }

                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    request = new AppendObjectRequest(_bucketName, key)
                    {
                        Content = fs,
                        Position = position,
                        RequestPayer = RequestPayer.Requester
                    };

                    var result = _ossPayerClient.AppendObject(request);
                    Assert.AreEqual(fileLength * 2, result.NextAppendPosition);
                    Assert.IsTrue(result.HashCrc64Ecma != 0);
                }

                var meta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("Appendable", meta.ObjectType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void SymlinkObjectTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            string symlink = key + "_link";
            OssTestUtils.UploadObject(_ossClient, _bucketName, key, Config.UploadTestFile);

            var request = new CreateSymlinkRequest(_bucketName, symlink, key);
            Assert.AreEqual(request.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.CreateSymlink(request);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            request.RequestPayer = RequestPayer.Requester;
            _ossPayerClient.CreateSymlink(request);

            Assert.IsTrue(_ossClient.DoesObjectExist(_bucketName, symlink));

            //
            var gRequest = new GetSymlinkRequest(_bucketName, symlink);
            Assert.AreEqual(gRequest.RequestPayer, RequestPayer.BucketOwner);
            try
            {
                _ossPayerClient.GetSymlink(gRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            gRequest.RequestPayer = RequestPayer.Requester;
            OssSymlink ossSymlink = _ossPayerClient.GetSymlink(gRequest);
            Assert.AreEqual(ossSymlink.Target, key);
        }

        [Test]
        public void RestoreObjectTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            //upload the object
            OssTestUtils.UploadObject(_ossClient, _archiveBucketName, key, Config.UploadTestFile);
            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _archiveBucketName, key));

            var request = new RestoreObjectRequest(_archiveBucketName, key);
            Assert.AreEqual(request.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.RestoreObject(request);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            request.RequestPayer = RequestPayer.Requester;
            var result = _ossPayerClient.RestoreObject(request);
            Assert.IsTrue(result.HttpStatusCode == HttpStatusCode.Accepted);

            try
            {
                result = _ossPayerClient.RestoreObject(request);
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "RestoreAlreadyInProgress");
            }

            int wait = 60;
            while (wait > 0)
            {
                var meta = _ossClient.GetObjectMetadata(_archiveBucketName, key);
                string restoreStatus = meta.HttpMetadata["x-oss-restore"] as string;
                if (restoreStatus != null && restoreStatus.StartsWith("ongoing-request=\"false\"", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }

                Thread.Sleep(1000 * 30);
                wait--;
            }

            result = _ossPayerClient.RestoreObject(request);
            Assert.IsTrue(result.HttpStatusCode == HttpStatusCode.OK);

            _ossClient.DeleteObject(_archiveBucketName, key);
        }

        [Test]
        public void ObjectAclTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, key, Config.UploadTestFile);

            var getRequest = new GetObjectAclRequest(_bucketName, key);
            Assert.AreEqual(getRequest.RequestPayer, RequestPayer.BucketOwner);

            var setRequest = new SetObjectAclRequest(_bucketName, key, CannedAccessControlList.PublicRead);
            Assert.AreEqual(setRequest.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.GetObjectAcl(getRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            try
            {
                _ossPayerClient.SetObjectAcl(setRequest);
                Assert.Fail("should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            setRequest.RequestPayer = RequestPayer.Requester;
            getRequest.RequestPayer = RequestPayer.Requester;
            _ossPayerClient.SetObjectAcl(setRequest);
            var getResult = _ossPayerClient.GetObjectAcl(getRequest);
            Assert.AreEqual(getResult.ACL, CannedAccessControlList.PublicRead);

            setRequest = new SetObjectAclRequest(_bucketName, key, CannedAccessControlList.PublicReadWrite);
            setRequest.RequestPayer = RequestPayer.Requester;
            _ossPayerClient.SetObjectAcl(setRequest);
            getResult = _ossPayerClient.GetObjectAcl(getRequest);
            Assert.AreEqual(getResult.ACL, CannedAccessControlList.PublicReadWrite);
        }

        [Test]
        public void ResumableUploadPositiveTest()
        {
            var fileName = Config.UploadTestFile;
            var fileInfo = new FileInfo(fileName);
            var fileSize = fileInfo.Length;

            //  < PartSize
            var key = OssTestUtils.GetObjectKey(_className);

            var request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = fileSize + 1,
                RequestPayer = RequestPayer.Requester
            };

            var result = _ossPayerClient.ResumableUploadObject(request);
            Assert.IsTrue(result.ETag.Length > 0);
            var headResult = _ossClient.GetObjectMetadata(_bucketName, key);
            Assert.AreEqual(headResult.ContentLength, fileSize);

            // > PartSize with multi thread
            key = OssTestUtils.GetObjectKey(_className);

            request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = 100*1024,
                RequestPayer = RequestPayer.Requester
            };

            result = _ossPayerClient.ResumableUploadObject(request);
            Assert.IsTrue(result.ETag.Length > 0);
            headResult = _ossClient.GetObjectMetadata(_bucketName, key);
            Assert.AreEqual(headResult.ContentLength, fileSize);

            // > PartSize with single thread
            key = OssTestUtils.GetObjectKey(_className);

            request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = 100 * 1024,
                RequestPayer = RequestPayer.Requester,
                ParallelThreadCount  = 1
            };

            result = _ossPayerClient.ResumableUploadObject(request);
            Assert.IsTrue(result.ETag.Length > 0);
            headResult = _ossClient.GetObjectMetadata(_bucketName, key);
            Assert.AreEqual(headResult.ContentLength, fileSize);
        }

        [Test]
        public void ResumableUploadNegativeTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            // < Default PartSize Case
            var request = new UploadObjectRequest(_bucketName, key, Config.UploadTestFile);
            Assert.AreEqual(request.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.ResumableUploadObject(request);
                Assert.Fail("should not be here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }

            // > Default PartSize Case
            request = new UploadObjectRequest(_bucketName, key, Config.MultiUploadTestFile);
            Assert.AreEqual(request.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.ResumableUploadObject(request);
                Assert.Fail("should not be here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "AccessDenied");
            }
        }

        [Test]
        public void ResumableDownloadPositiveTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            var key = OssTestUtils.GetObjectKey(_className);

            var fileName = Config.UploadTestFile;
            var fileInfo = new FileInfo(fileName);
            var fileSize = fileInfo.Length;

            _ossClient.PutObject(_bucketName, key, fileName);

            //  < PartSize
            var request = new DownloadObjectRequest(_bucketName, key, targetFile)
            {
                PartSize = fileSize +1,
                RequestPayer = RequestPayer.Requester
            };
            var metadata = _ossPayerClient.ResumableDownloadObject(request);
            var expectedETag = metadata.ETag;
            var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
            Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            FileUtils.DeleteFile(targetFile);


            // > PartSize with multi thread
            request = new DownloadObjectRequest(_bucketName, key, targetFile)
            {
                PartSize = 100*1024,
                RequestPayer = RequestPayer.Requester
            };
            metadata = _ossPayerClient.ResumableDownloadObject(request);
            expectedETag = metadata.ETag;
            downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
            Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            FileUtils.DeleteFile(targetFile);

            // > PartSize with single thread
            request = new DownloadObjectRequest(_bucketName, key, targetFile)
            {
                PartSize = 100 * 1024,
                RequestPayer = RequestPayer.Requester,
                ParallelThreadCount = 1
            };
            metadata = _ossPayerClient.ResumableDownloadObject(request);
            expectedETag = metadata.ETag;
            downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
            Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            FileUtils.DeleteFile(targetFile);
        }

        [Test]
        public void ResumableDownloadNegativeTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            var key = OssTestUtils.GetObjectKey(_className);

            var request = new DownloadObjectRequest(_bucketName, key, targetFile);
            Assert.AreEqual(request.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.ResumableDownloadObject(request);
                Assert.Fail("should not be here.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void ResumableCopyPositiveTest()
        {
            var targetKey = OssTestUtils.GetObjectKey(_className);
            var sourcekey = OssTestUtils.GetObjectKey(_className);

            _ossClient.PutObject(_bucketName, sourcekey, Config.UploadTestFile);
            var sourceMeta = _ossClient.GetObjectMetadata(_bucketName, sourcekey);


            var copyRequest = new CopyObjectRequest(_bucketName, sourcekey, _bucketName, targetKey)
            {
                RequestPayer = RequestPayer.Requester
            };

            //  < PartSize
            _ossPayerClient.ResumableCopyObject(copyRequest, null, sourceMeta.ContentLength + 1);
            var targetMeta = _ossClient.GetObjectMetadata(_bucketName, targetKey);
            Assert.AreEqual(targetMeta.Crc64, sourceMeta.Crc64);

            // > PartSize
            _ossPayerClient.ResumableCopyObject(copyRequest, null, 100*1024);
            targetMeta = _ossClient.GetObjectMetadata(_bucketName, targetKey);
            Assert.AreEqual(targetMeta.Crc64, sourceMeta.Crc64);
        }

        [Test]
        public void ResumableCopyNegativeTest()
        {
            var sourcekey = OssTestUtils.GetObjectKey(_className);
            var targetkey = OssTestUtils.GetObjectKey(_className);
            var request = new CopyObjectRequest(_bucketName, sourcekey, _bucketName, targetkey);
            Assert.AreEqual(request.RequestPayer, RequestPayer.BucketOwner);

            try
            {
                _ossPayerClient.ResumableCopyObject(request, Config.DownloadFolder);
                Assert.Fail("should not be here.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }
    }
}