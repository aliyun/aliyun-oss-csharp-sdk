using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;
using Aliyun.OSS.Util;
using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.OtherTestClass
{
    [TestFixture]
    public class SignerTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _localImageFile;
        private static List<string> _headersToSign; 

#if NETCOREAPP2_0
        [OneTimeSetUp]
#else
        [TestFixtureSetUp]
#endif
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            //get current class name, which is prefix of bucket/object
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName(_className);
            IOss ossClient = OssClientFactory.CreateOssClient();
            ossClient.CreateBucket(_bucketName);
            //sample file
            _localImageFile = Config.ImageTestFile;
            _headersToSign = new List<string>
            {
                HttpHeaders.CacheControl, HttpHeaders.ContentDisposition,
                HttpHeaders.ContentEncoding, HttpHeaders.ContentLength,
                HttpHeaders.ContentMd5, HttpHeaders.ContentType,
                HttpHeaders.Date, HttpHeaders.Expires,
                HttpHeaders.LastModified, HttpHeaders.Range,
                HttpHeaders.CopySource,
            };
        }

#if NETCOREAPP2_0
        [OneTimeTearDown]
#else
        [TestFixtureTearDown]
#endif
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void SignerTestWithoutHeaderToSign()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var conf = new ClientConfiguration();
            conf.SignerVersion = SignUtils.SignerVerion2;
            var ossClient = OssClientFactory.CreateOssClient(conf);

            try {
                //put object
                ossClient.PutObject(_bucketName, key, Config.UploadTestFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(ossClient, _bucketName, key));

                //get object
                OssTestUtils.DownloadObject(ossClient, _bucketName, key, targetFile);
                var expectedETag = ossClient.GetObjectMetadata(_bucketName, key).ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());

                //list object
                var allObjects = ossClient.ListObjects(_bucketName, key);
                var allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                Assert.AreEqual(1, allObjectsSumm.Count);
            }
            finally
            {
                ossClient.DeleteObject(_bucketName, key);

                try
                {
                    FileUtils.DeleteFile(targetFile);
                }
                catch (Exception e)
                {
                    LogUtility.LogWarning("Delete file {0} failed with Exception {1}", targetFile, e.Message);
                }
            }
        }

        [Test]
        public void SignerTestWithHeaderToSign()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var conf = new ClientConfiguration();
            conf.SignerVersion = SignUtils.SignerVerion2;
            conf.HeadersToSign = _headersToSign;
            var ossClient= OssClientFactory.CreateOssClient(conf);

            try
            {
                //put object
                ossClient.PutObject(_bucketName, key, Config.UploadTestFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(ossClient, _bucketName, key));

                //get object
                OssTestUtils.DownloadObject(ossClient, _bucketName, key, targetFile);
                var expectedETag = ossClient.GetObjectMetadata(_bucketName, key).ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());

                //get object by range
                OssTestUtils.DownloadObjectUsingRange(ossClient, _bucketName, key, targetFile);
                expectedETag = ossClient.GetObjectMetadata(_bucketName, key).ETag;
                downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreNotEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());

                //copy object
                //construct metadata
                var metadata = new ObjectMetadata();
                const string userMetaKey = "myKey";
                const string userMetaValue = "myValue";
                metadata.UserMetadata.Add(userMetaKey, userMetaValue);
                metadata.CacheControl = "no-cache";
                var targetObjectKey = key + "_copy";
                var coRequest = new CopyObjectRequest(_bucketName, key, _bucketName, targetObjectKey)
                {
                    NewObjectMetadata = metadata
                };
                ossClient.CopyObject(coRequest);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));
                var resMetadata = ossClient.GetObjectMetadata(_bucketName, targetObjectKey);
                Assert.AreEqual(userMetaValue, resMetadata.UserMetadata[userMetaKey]);

                //put object without crc
                conf.EnableCrcCheck = false;
                var ossClient2 = OssClientFactory.CreateOssClient(conf);
                var key2 = key + "_2";
                ossClient2.PutObject(_bucketName, key2, Config.UploadTestFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(ossClient, _bucketName, key2));

                // put example image &  processed image
                var key_image = key + "_image.jpg";
                var process_image = "image/resize,m_fixed,w_100,h_100";
                ossClient.PutObject(_bucketName, key_image, _localImageFile);
                GetObjectRequest request = new GetObjectRequest(_bucketName, key_image, process_image);
                OssObject ossObject = ossClient.GetObject(request);
                Assert.AreEqual(ossObject.ContentLength, 3347);

                //list object
                var allObjects = ossClient.ListObjects(_bucketName, key);
                var allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                Assert.AreEqual(4, allObjectsSumm.Count);
            }
            finally
            {
                ossClient.DeleteObject(_bucketName, key);

                try
                {
                    FileUtils.DeleteFile(targetFile);
                }
                catch (Exception e)
                {
                    LogUtility.LogWarning("Delete file {0} failed with Exception {1}", targetFile, e.Message);
                }
            }
        }

        [Test]
        public void SignerTestUsePreSignedUri()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            long fileLength = 0;

            var conf = new ClientConfiguration();
            conf.SignerVersion = SignUtils.SignerVerion2;
            var ossClient = OssClientFactory.CreateOssClient(conf);

            //put object
            var uri = ossClient.GeneratePresignedUri(_bucketName, key, DateTime.Now.AddHours(1), SignHttpMethod.Put);
            try
            {
                var putResult = ossClient.PutObject(uri, Config.UploadTestFile);
                Assert.AreEqual(putResult.HttpStatusCode, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }

            //get object
            uri = ossClient.GeneratePresignedUri(_bucketName, key, DateTime.Now.AddHours(1), SignHttpMethod.Get);
            try
            {
                var getResult = ossClient.GetObject(uri);
                var expectedETag = getResult.Metadata.ETag;
                fileLength = getResult.Metadata.ContentLength;
                using (var stream = getResult.Content)
                {
                    var actualETag = FileUtils.ComputeContentMd5(stream);
                    Assert.AreEqual(expectedETag.ToLowerInvariant(), actualETag.ToLowerInvariant());
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }

            //get object by range
            uri = ossClient.GeneratePresignedUri(_bucketName, key, DateTime.Now.AddHours(1), SignHttpMethod.Get);
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            try
            {
                req = WebRequest.Create(uri) as HttpWebRequest;
                req.Method = "GET";
                req.AddRange(0, fileLength / 2);
                res = req.GetResponse() as HttpWebResponse;
                Assert.AreEqual(HttpStatusCode.PartialContent, res.StatusCode, "response status code is not expected.");
                using (var stream = res.GetResponseStream())
                {
                    var actualETag = FileUtils.ComputeContentMd5(stream);
                    Assert.AreNotEqual(res.GetResponseHeader("ETag").ToLowerInvariant(), actualETag.ToLowerInvariant());
                }
            }
            catch
            {
                //TODO:
                Assert.Fail("get object by range fail");
            }
            finally
            {
                if (req != null) req.Abort();
                if (res != null) res.Close();
            }

            // put example image &  processed image
            // step 1 put image
            var key_image = key + "_image.jpg";
            uri = ossClient.GeneratePresignedUri(_bucketName, key_image, DateTime.Now.AddHours(1), SignHttpMethod.Put);
            try
            {
                var putResult = ossClient.PutObject(uri, Config.ImageTestFile);
                Assert.AreEqual(putResult.HttpStatusCode, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            // step 2 processed image
            var preReq = new GeneratePresignedUriRequest(_bucketName, key_image, SignHttpMethod.Get)
            {
                Expiration = DateTime.Now.AddHours(1),
                Process = "image/resize,m_fixed,w_100,h_100"
            };
            uri = ossClient.GeneratePresignedUri(preReq);
            try
            {
                var getResult = ossClient.GetObject(uri);
                Assert.AreEqual(getResult.HttpStatusCode, HttpStatusCode.OK);
                Assert.AreEqual(getResult.ContentLength, 3347);
            }
            catch
            {
                Assert.Fail("get processed image fail.");
            }

            //get object with user paramter
            preReq = new GeneratePresignedUriRequest(_bucketName, key_image, SignHttpMethod.Get)
            {
                Expiration = DateTime.Now.AddHours(1),
            };
            preReq.AddQueryParam("x-param-test", "test-value");
            preReq.AddQueryParam("x-param-test1", "test-value1");
            uri = ossClient.GeneratePresignedUri(preReq);
            try
            {
                var getResult = ossClient.GetObject(uri);
                Assert.AreEqual(getResult.HttpStatusCode, HttpStatusCode.OK);
                using (var stream = getResult.Content)
                {
                    var actualETag = FileUtils.ComputeContentMd5(stream);
                    Assert.AreEqual(getResult.Metadata.ETag.ToLowerInvariant(), actualETag.ToLowerInvariant());
                }
            }
            catch
            {
                Assert.Fail("get object with user paramter fail.");
            }

            //get object with user paramter NG case
            preReq = new GeneratePresignedUriRequest(_bucketName, key_image, SignHttpMethod.Get)
            {
                Expiration = DateTime.Now.AddHours(1),
            };
            preReq.AddQueryParam("x-param-test", "test-value");
            preReq.AddQueryParam("x-param-test1", "test-value1");
            uri = ossClient.GeneratePresignedUri(preReq);
            try
            {
                uri = new Uri(uri.ToString() + "&x-param-test2=test-value2");
                var getResult = ossClient.GetObject(uri);
                Assert.Fail("get object with user paramter NG case");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.SignatureDoesNotMatch, e.ErrorCode);
            }
            catch
            {
                Assert.Fail("get object with user paramter NG case");
            }
        }
    }
}
