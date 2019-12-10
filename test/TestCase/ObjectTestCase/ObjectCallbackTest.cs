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

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectCallbackTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _objectKey;
        private static string _bigObjectKey;
        private static string _callbackUrl;
        private static string _callbackBody;
        private static string _callbackOkResponse;

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

            //upload sample object
            _objectKey = OssTestUtils.GetObjectKey(_className);
            //upload multipart sample object
            _bigObjectKey = _objectKey + ".js";

            // call paramters
            _callbackUrl = Config.CallbackServer;
            _callbackBody = "bucket=${bucket}&object=${object}&etag=${etag}&size=${size}&mimeType=${mimeType}&my_var1=${x:var1}";
            _callbackOkResponse = "{\"Status\":\"OK\"}";
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        #region put object
        [Test]
        public void PutObjectCallbackTest()
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, _callbackBody).Build();

            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);

            var putObjectResult = _ossClient.PutObject(_bucketName, _objectKey, Config.UploadTestFile, metadata);
            Assert.IsTrue(putObjectResult.IsSetResponseStream());
            Assert.AreEqual(_callbackOkResponse, GetCallbackResponse(putObjectResult));
            Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(putObjectResult.ContentLength, _callbackOkResponse.Length);
            Assert.AreEqual(putObjectResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentType], "application/json");
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            _ossClient.DeleteObject(_bucketName, _objectKey);
        }

        [Test]
        public void PutObjectCallbackVarTest()
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, _callbackBody).Build();
            string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
                AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();

            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);

            var putObjectResult = _ossClient.PutObject(_bucketName, _objectKey, Config.UploadTestFile, metadata);
            Assert.IsTrue(putObjectResult.IsSetResponseStream());
            Assert.AreEqual(_callbackOkResponse, GetCallbackResponse(putObjectResult));
            Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(putObjectResult.ContentLength, _callbackOkResponse.Length);
            Assert.AreEqual(putObjectResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentType], "application/json");
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            _ossClient.DeleteObject(_bucketName, _objectKey);
        }

        [Test]
        public void PutObjectWithZeroCallbackVarTest()
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, _callbackBody).Build();
            string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().Build();

            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);

            var putObjectResult = _ossClient.PutObject(_bucketName, _objectKey, Config.UploadTestFile, metadata);
            Assert.IsTrue(putObjectResult.IsSetResponseStream());
            Assert.AreEqual(_callbackOkResponse, GetCallbackResponse(putObjectResult));
            Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(putObjectResult.ContentLength, _callbackOkResponse.Length);
            Assert.AreEqual(putObjectResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentType], "application/json");
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            _ossClient.DeleteObject(_bucketName, _objectKey);
        }

        [Test]
        public void PutObjectWithoutCallbackTest()
        {
            var metadata = new ObjectMetadata
            {
                ContentType = "text/rtf",
                CacheControl = "public",
                ServerSideEncryption = "AES256"
            };
            metadata.UserMetadata.Add("Author", "Mingdi");
            metadata.UserMetadata.Add("Category", "C#");

            var putObjectResult = _ossClient.PutObject(_bucketName, _objectKey, Config.UploadTestFile, metadata);
            Assert.IsFalse(putObjectResult.IsSetResponseStream());
            Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(putObjectResult.ContentLength, 0);
            Assert.AreEqual(putObjectResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.IsFalse(putObjectResult.ResponseMetadata.ContainsKey(HttpHeaders.ContentType));
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            var objectMetadata = _ossClient.GetObjectMetadata(_bucketName, _objectKey);
            Assert.AreEqual(objectMetadata.ContentType, "text/rtf");
            Assert.AreEqual(objectMetadata.CacheControl, "public");
            Assert.AreEqual(objectMetadata.ServerSideEncryption, "AES256");
            Assert.AreEqual(objectMetadata.UserMetadata["Author"], "Mingdi");
            Assert.AreEqual(objectMetadata.UserMetadata["Category"], "C#");

            var ossObject = _ossClient.GetObject(_bucketName, _objectKey);
            Assert.IsTrue(ossObject.IsSetResponseStream());
            Assert.IsTrue(ossObject.ContentLength > 0);
            Assert.AreEqual(ossObject.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(ossObject.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(ossObject.ResponseMetadata[HttpHeaders.ContentType], "text/rtf");
            Assert.AreEqual(ossObject.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.AreEqual(ossObject.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(ossObject.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(ossObject.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            _ossClient.DeleteObject(_bucketName, _objectKey);
        }

        [Test]
        public void PutObjectCallbackUriInvalidNegativeTest()
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder("https://wwww.aliyun.com/", _callbackBody).Build();

            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);

            try
            {
                _ossClient.PutObject(_bucketName, _objectKey, Config.UploadTestFile, metadata);
                Assert.Fail("Put object callback should be not successfully with invald callback uri");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.CallbackFailed, e.ErrorCode);
                Assert.AreEqual("Error status : 502.", e.Message);
            }
        }

        [Test]
        public void PutObjectCallbackVarInvalidNegativeTest()
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, _callbackBody).Build();
            try
            {
                string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
                    AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("var2", "value2").Build();
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void PutObjectCallbackHeaderArgumentTest()
        {
            var callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, _callbackBody, "test.demo.com", CallbackBodyType.Url).Build();
            callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, _callbackBody, "test.demo.com", CallbackBodyType.Json).Build();

            try
            {
                callbackHeaderBuilder = new CallbackHeaderBuilder(null, null, null, CallbackBodyType.Url).Build();
                Assert.IsTrue(false);
            }
            catch(Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Callback argument invalid"));
            }

            try
            {
                callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, null, null, CallbackBodyType.Url).Build();
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Callback argument invalid"));
            }
        }
        #endregion

        #region multipart upload
        [Test]
        public void MultipartUploadCallbackVarTest()
        {
            // Initiate a multipart upload
            var initRequest = new InitiateMultipartUploadRequest(_bucketName, _bigObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);

            // Sets the part size as 1M
            const int partSize = 1024 * 1024 * 1;
            var partFile = new FileInfo(Config.MultiUploadTestFile);
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
                    var uploadPartRequest = new UploadPartRequest(_bucketName, _bigObjectKey, initResult.UploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = (i + 1)
                    };
                    var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);

                    // adds the result to the list 
                    partETags.Add(uploadPartResult.PartETag);
                }
            }

            string callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, _callbackBody).Build();
            string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
                AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();
            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);

            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, _bigObjectKey, initResult.UploadId);
            completeRequest.Metadata = metadata;
            foreach (var partETag in partETags)
            {
                completeRequest.PartETags.Add(partETag);
            }
            var completeMultipartUploadResult = _ossClient.CompleteMultipartUpload(completeRequest);
            Assert.IsTrue(completeMultipartUploadResult.IsSetResponseStream());
            Assert.AreEqual(_callbackOkResponse, GetCallbackResponse(completeMultipartUploadResult));
            Assert.AreEqual(completeMultipartUploadResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(completeMultipartUploadResult.ContentLength, _callbackOkResponse.Length);
            Assert.AreEqual(completeMultipartUploadResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(completeMultipartUploadResult.ResponseMetadata[HttpHeaders.ContentType], "application/json");
            Assert.AreEqual(completeMultipartUploadResult.ResponseMetadata[HttpHeaders.ETag], completeMultipartUploadResult.ETag);
            Assert.IsFalse(completeMultipartUploadResult.ResponseMetadata.ContainsKey(HttpHeaders.ContentMd5));
            Assert.IsTrue(completeMultipartUploadResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(completeMultipartUploadResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(completeMultipartUploadResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            //delete the object
            _ossClient.DeleteObject(_bucketName, _bigObjectKey);
        }

        [Test]
        public void MultipartUploadWithoutCallbackTest()
        {
            // initiates a multipart upload
            var initRequest = new InitiateMultipartUploadRequest(_bucketName, _bigObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);
            Assert.AreEqual(initResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(initResult.ContentLength > 0);
            Assert.AreEqual(initResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(initResult.ResponseMetadata[HttpHeaders.ContentType], "application/xml");
            Assert.IsFalse(initResult.ResponseMetadata.ContainsKey(HttpHeaders.ETag));
            Assert.IsFalse(initResult.ResponseMetadata.ContainsKey(HttpHeaders.ContentMd5));
            Assert.IsFalse(initResult.ResponseMetadata.ContainsKey(HttpHeaders.HashCrc64Ecma));
            Assert.IsTrue(initResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(initResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            // Sets part size 1M
            const int partSize = 1024 * 1024 * 1;
            var partFile = new FileInfo(Config.MultiUploadTestFile);
            // calculates the part count
            var partCount = OssTestUtils.CalculatePartCount(partFile.Length, partSize);

            // creats the list for PartETag 
            var partETags = new List<PartETag>();
            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    // Skips to the start position of each part
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // calculates the part size 
                    var size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // creates a UploadPartRequest, uploading parts
                    var uploadPartRequest = new UploadPartRequest(_bucketName, _bigObjectKey, initResult.UploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = (i + 1)
                    };
                    var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);
                    Assert.AreEqual(uploadPartResult.HttpStatusCode, HttpStatusCode.OK);
                    Assert.AreEqual(uploadPartResult.ContentLength, 0);
                    Assert.AreEqual(uploadPartResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
                    Assert.IsFalse(uploadPartResult.ResponseMetadata.ContainsKey(HttpHeaders.ContentType));
                    Assert.AreEqual(uploadPartResult.ResponseMetadata[HttpHeaders.ETag], uploadPartResult.ETag);
                    Assert.AreEqual(uploadPartResult.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
                    Assert.IsTrue(uploadPartResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
                    Assert.AreEqual(uploadPartResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

                    // saves the result to the list
                    partETags.Add(uploadPartResult.PartETag);
                }
            }

            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, _bigObjectKey, initResult.UploadId);
            foreach (var partETag in partETags)
            {
                completeRequest.PartETags.Add(partETag);
            }
            var completeMultipartUploadResult = _ossClient.CompleteMultipartUpload(completeRequest);
            Assert.IsFalse(completeMultipartUploadResult.IsSetResponseStream());
            Assert.AreEqual(completeMultipartUploadResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(completeMultipartUploadResult.ContentLength > 0);
            Assert.AreEqual(completeMultipartUploadResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(completeMultipartUploadResult.ResponseMetadata[HttpHeaders.ContentType], "application/xml");
            Assert.AreEqual(completeMultipartUploadResult.ResponseMetadata[HttpHeaders.ETag], completeMultipartUploadResult.ETag);
            Assert.IsFalse(completeMultipartUploadResult.ResponseMetadata.ContainsKey(HttpHeaders.ContentMd5));
            Assert.IsTrue(completeMultipartUploadResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(completeMultipartUploadResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(completeMultipartUploadResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            //delete the object
            _ossClient.DeleteObject(_bucketName, _bigObjectKey);
        }

        [Test]
        public void MultipartUploadCallbackUriInvalidNegativeTest()
        {
            // initiates a multipart upload 
            var initRequest = new InitiateMultipartUploadRequest(_bucketName, _bigObjectKey);
            var initResult = _ossClient.InitiateMultipartUpload(initRequest);

            // set part size 1MB 
            const int partSize = 1024 * 1024 * 1;
            var partFile = new FileInfo(Config.MultiUploadTestFile);
            // sets part count 
            var partCount = OssTestUtils.CalculatePartCount(partFile.Length, partSize);

            var partETags = new List<PartETag>();
            //upload the file
            using (var fs = new FileStream(partFile.FullName, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    // skip to start position of each part
                    long skipBytes = partSize * i;
                    fs.Position = skipBytes;

                    // sets the part size
                    var size = partSize < partFile.Length - skipBytes
                        ? partSize
                        : partFile.Length - skipBytes;

                    // creates a UploadPartRequest, uploading parts
                    var uploadPartRequest = new UploadPartRequest(_bucketName, _bigObjectKey, initResult.UploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = (i + 1)
                    };
                    var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);

                    // saves the result to the list 
                    partETags.Add(uploadPartResult.PartETag);
                }
            }

            var completeRequest = new CompleteMultipartUploadRequest(_bucketName, _bigObjectKey, initResult.UploadId);
            foreach (var partETag in partETags)
            {
                completeRequest.PartETags.Add(partETag);
            }

            string callbackHeaderBuilder = new CallbackHeaderBuilder("https://wwww.aliyun.com/", _callbackBody).Build();
            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            completeRequest.Metadata = metadata;

            try
            {
                _ossClient.CompleteMultipartUpload(completeRequest);
                Assert.Fail("Multipart upload callback should be not successfully with invald callback uri");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.CallbackFailed, e.ErrorCode);
                Assert.AreEqual("Error status : 502.", e.Message);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, _bigObjectKey);
            }
        }
        #endregion

        #region resumable upload
        [Test]
        public void ResumableUploadCallbackTest()
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, _callbackBody).Build();
            string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
                AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();

            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);

            // use PutObject for small files
            var putObjectResult = _ossClient.ResumableUploadObject(_bucketName, _objectKey, Config.UploadTestFile, metadata, null);
            Assert.IsTrue(putObjectResult.IsSetResponseStream());
            Assert.AreEqual(_callbackOkResponse, GetCallbackResponse(putObjectResult));
            Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(putObjectResult.ContentLength, _callbackOkResponse.Length);
            Assert.AreEqual(putObjectResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentType], "application/json");
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            _ossClient.DeleteObject(_bucketName, _objectKey);

            // use Multipart upload for big files
            putObjectResult = _ossClient.ResumableUploadObject(_bucketName, _objectKey, Config.MultiUploadTestFile, metadata, null, 1024*1024);
            Assert.IsTrue(putObjectResult.IsSetResponseStream());
            Assert.AreEqual(_callbackOkResponse, GetCallbackResponse(putObjectResult));
            Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(putObjectResult.ContentLength, _callbackOkResponse.Length);
            Assert.AreEqual(putObjectResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentType], "application/json");
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.IsFalse(putObjectResult.ResponseMetadata.ContainsKey(HttpHeaders.ContentMd5));
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            _ossClient.DeleteObject(_bucketName, _objectKey);
        }

        [Test]
        public void ResumableUploadCallbackUriInvalidNegativeTest()
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder("https://wwww.aliyun.com/", _callbackBody).Build();

            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);

            try
            {
                // use PutObject for small files
                _ossClient.ResumableUploadObject(_bucketName, _objectKey, Config.UploadTestFile, metadata, null);
                Assert.Fail("Put object callback should be not successfully with invald callback uri");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.CallbackFailed, e.ErrorCode);
                Assert.AreEqual("Error status : 502.", e.Message);
            }

            try
            {
                // use Multipart upload for big files
                _ossClient.ResumableUploadObject(_bucketName, _objectKey, Config.MultiUploadTestFile, metadata, null, 1024*1024);
                Assert.Fail("Put object callback should be not successfully with invald callback uri");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.CallbackFailed, e.ErrorCode);
                Assert.AreEqual("Error status : 502.", e.Message);
            }
        }
        #endregion

        #region generate presigned uri
        [Test]
        public void GeneratePresignedUriCallbackTest()
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, _callbackBody).Build();
            string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
                AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();

            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, _objectKey, SignHttpMethod.Put)
            {
                ContentType = "text/rtf",
                Expiration = DateTime.Now.AddHours(1),
                Callback = callbackHeaderBuilder,
                CallbackVar = CallbackVariableHeaderBuilder
            };
            gpuRequest.UserMetadata.Add("Author", "Mingdi");
            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            var metadata = new ObjectMetadata();
            metadata.ContentType = "text/rtf";
            metadata.UserMetadata.Add("Author", "Mingdi");
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);

            var putObjectResult = _ossClient.PutObject(uri, Config.UploadTestFile, metadata);
            Assert.IsTrue(putObjectResult.IsSetResponseStream());
            Assert.AreEqual(_callbackOkResponse, GetCallbackResponse(putObjectResult));
            Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(putObjectResult.ContentLength, _callbackOkResponse.Length);
            Assert.AreEqual(putObjectResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentType], "application/json");
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            _ossClient.DeleteObject(_bucketName, _objectKey);
        }

        [Test]
        public void GeneratePresignedUriCallbackTestWithParameter()
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(_callbackUrl, _callbackBody).Build();
            string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
                AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();

            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, _objectKey, SignHttpMethod.Put)
            {
                ContentType = "text/rtf",
                Expiration = DateTime.Now.AddHours(1),
                Callback = callbackHeaderBuilder,
                CallbackVar = CallbackVariableHeaderBuilder
            };
            gpuRequest.UserMetadata.Add("Author", "Mingdi");
            gpuRequest.AddQueryParam("x-param-null", "");
            gpuRequest.AddQueryParam("x-param-space0", " ");
            gpuRequest.AddQueryParam("x-param-value", "value");
            gpuRequest.AddQueryParam("x-param-space1", " ");
            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            var metadata = new ObjectMetadata();
            metadata.ContentType = "text/rtf";
            metadata.UserMetadata.Add("Author", "Mingdi");
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);

            var putObjectResult = _ossClient.PutObject(uri, Config.UploadTestFile, metadata);
            Assert.IsTrue(putObjectResult.IsSetResponseStream());
            Assert.AreEqual(_callbackOkResponse, GetCallbackResponse(putObjectResult));
            Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(putObjectResult.ContentLength, _callbackOkResponse.Length);
            Assert.AreEqual(putObjectResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentType], "application/json");
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            _ossClient.DeleteObject(_bucketName, _objectKey);
        }

        [Test]
        public void GeneratePresignedUriMetadataTest()
        {
            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, _objectKey, SignHttpMethod.Put)
            {
                ContentType = "text/rtf",
                Expiration = DateTime.Now.AddHours(1),
            };
            gpuRequest.UserMetadata.Add("Author", "Mingdi");
            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            var metadata = new ObjectMetadata();
            metadata.ContentType = "text/rtf";
            metadata.UserMetadata.Add("Author", "Mingdi");

            var putObjectResult = _ossClient.PutObject(uri, Config.UploadTestFile, metadata);
            Assert.IsFalse(putObjectResult.IsSetResponseStream());
            Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(putObjectResult.ContentLength, 0);
            Assert.AreEqual(putObjectResult.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.IsFalse(putObjectResult.ResponseMetadata.ContainsKey(HttpHeaders.ContentType));
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(putObjectResult.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            uri = _ossClient.GeneratePresignedUri(_bucketName, _objectKey, SignHttpMethod.Get);
            var ossObject = _ossClient.GetObject(uri);
            Assert.IsTrue(ossObject.IsSetResponseStream());
            Assert.IsTrue(ossObject.ContentLength > 0);
            Assert.AreEqual(ossObject.Metadata.ContentType, "text/rtf");
            Assert.AreEqual(ossObject.Metadata.UserMetadata["Author"], "Mingdi");
            Assert.AreEqual(ossObject.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(ossObject.RequestId.Length, "58DB0ACB686D42D5B4163D75".Length);
            Assert.AreEqual(ossObject.ResponseMetadata[HttpHeaders.ContentType], "text/rtf");
            Assert.AreEqual(ossObject.ResponseMetadata[HttpHeaders.ETag], putObjectResult.ETag);
            Assert.AreEqual(ossObject.ResponseMetadata[HttpHeaders.ContentMd5].Length, "7GoU4OYaYroKXsbsG1f/lw==".Length);
            Assert.IsTrue(putObjectResult.ResponseMetadata[HttpHeaders.HashCrc64Ecma].Length > 0);
            Assert.IsTrue(ossObject.ResponseMetadata[HttpHeaders.ServerElapsedTime].Length > 0);
            Assert.AreEqual(ossObject.ResponseMetadata[HttpHeaders.Date].Length, "Wed, 29 Mar 2017 01:15:58 GMT".Length);

            _ossClient.DeleteObject(_bucketName, _objectKey);
        }

        [Test]
        public void GeneratePresignedUriCallbackUriInvalidNegativeTest()
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder("https://wwww.aliyun.com/", _callbackBody).Build();

            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, _objectKey, SignHttpMethod.Put)
            {
                Expiration = DateTime.Now.AddHours(1),
                Callback = callbackHeaderBuilder
            };
            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
   
            try
            {
                _ossClient.PutObject(uri, Config.UploadTestFile, metadata);
                Assert.Fail("Put object callback should be not successfully with invald callback uri");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.CallbackFailed, e.ErrorCode);
                Assert.AreEqual("Error status : 502.", e.Message);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, _objectKey);
            }
        }
        #endregion

        #region private
        private string GetCallbackResponse(PutObjectResult putObjectResult)
        {
            string callbackResponse = null;
            using (var stream = putObjectResult.ResponseStream)
            {
                var buffer = new byte[4 * 1024];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                callbackResponse = Encoding.Default.GetString(buffer, 0, bytesRead);
            }
            return callbackResponse;
        }
        #endregion
    }
}
