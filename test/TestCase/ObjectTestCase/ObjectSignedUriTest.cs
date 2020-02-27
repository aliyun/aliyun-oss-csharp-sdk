using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Model;
using Aliyun.OSS.Test.Util;
using Aliyun.OSS.Util;
using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectSignedUriTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _objectKey;
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
            //create sample object
            _objectKey = OssTestUtils.GetObjectKey(_className);
            var poResult = OssTestUtils.UploadObject(_ossClient, _bucketName, _objectKey,
                Config.UploadTestFile, new ObjectMetadata());
            _objectETag = poResult.ETag;
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        #region GET methods
        [Test]
        public void GetPreSignedUriDefaultPositiveTest()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            var now = DateTime.Now;
            //set expiration time to 5 seconds later
            var expireDate = now.AddSeconds(5);
            var uri = _ossClient.GeneratePresignedUri(_bucketName, _objectKey, expireDate);
            try
            {
                req = WebRequest.Create(uri) as HttpWebRequest;
                req.Method = "GET";
                res = req.GetResponse() as HttpWebResponse;
                Assert.AreEqual(HttpStatusCode.OK, res.StatusCode, "response status code is not expected.");
                using (var stream = res.GetResponseStream())
                {
                    var actualETag = FileUtils.ComputeContentMd5(stream);
                    Assert.AreEqual(_objectETag.ToLowerInvariant(), actualETag.ToLowerInvariant());
                }
            }
            catch (WebException)
            {
                //TODO:
            }
            finally
            {
                if(req != null) req.Abort();
                if(res != null) res.Close();
            }
        }

        [Test]
        public void GetPreSignedUriDefaultExpireDatePositiveTest()
        {
            var uri = _ossClient.GeneratePresignedUri(_bucketName, _objectKey);
            try
            {
                var goResult = _ossClient.GetObject(uri);
                using (var stream = goResult.Content)
                {
                    var actualETag = FileUtils.ComputeContentMd5(stream);
                    Assert.AreEqual(_objectETag.ToLowerInvariant(), actualETag.ToLowerInvariant());
                }
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [Test]
        public void GetPreSignedUriDefaultNegativeTest()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            var now = DateTime.Now;
            //set expiration time to 5 seconds before
            var expireDate = now.AddSeconds(-5);
            var uri = _ossClient.GeneratePresignedUri(_bucketName, _objectKey, expireDate);
            try
            {
                req = WebRequest.Create(uri) as HttpWebRequest;
                req.Method = "GET";
                res = req.GetResponse() as HttpWebResponse;
                Assert.Fail("response fail for expired URI");
            }
            catch (WebException e)
            {
                Assert.IsTrue(e.Message.Contains("403"), 
                    string.Format("Unexpected exception: {0}", e.Message));
            }
            finally
            {
                if (req != null) req.Abort();
                if (res != null) res.Close();
            }
        }

        [Test]
        public void GetPreSignedUriWithContentTypeAndMd5PositiveTest()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            var now = DateTime.Now;
            //set expiration time to 5 seconds later
            var expireDate = now.AddSeconds(5);
            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, _objectKey);
            gpuRequest.Expiration = expireDate;
            gpuRequest.ContentType = "application/zip";
            gpuRequest.ContentMd5 = _objectETag;
            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            try
            {
                req = WebRequest.Create(uri) as HttpWebRequest;
                req.Method = "GET";
                req.ContentType = "application/zip";
                req.Headers.Add(HttpRequestHeader.ContentMd5, _objectETag);
                res = req.GetResponse() as HttpWebResponse;
                Assert.AreEqual(HttpStatusCode.OK, res.StatusCode, "response status code is not expected.");
                using (var stream = res.GetResponseStream())
                {
                    var actualETag = FileUtils.ComputeContentMd5(stream);
                    Assert.AreEqual(_objectETag.ToLowerInvariant(), actualETag.ToLowerInvariant());
                }
            }
            catch (WebException)
            {
                //TODO:
            }
            finally
            {
                if (req != null) req.Abort();
                if (res != null) res.Close();
            }
        }

        [Test]
        public void GetPreSignedUriWithContentTypeAndMd5NegativeTest()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            var now = DateTime.Now;
            //set expiration time to 5 seconds later
            var expireDate = now.AddSeconds(5);
            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, _objectKey);
            gpuRequest.Expiration = expireDate;
            //do not set content type
            //gpuRequest.ContentType = "application/zip";
            gpuRequest.ContentMd5 = _objectETag;
            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            try
            {
                req = WebRequest.Create(uri) as HttpWebRequest;
                req.Method = "GET";
                req.ContentType = "application/zip";
                req.Headers.Add(HttpRequestHeader.ContentMd5, _objectETag);
                res = req.GetResponse() as HttpWebResponse;
                Assert.Fail("response fail for expired URI");
            }
            catch (WebException e)
            {
                Assert.IsTrue(e.Message.Contains("403"),
                    string.Format("Unexpected exception: {0}", e.Message));
            }
            finally
            {
                if (req != null) req.Abort();
                if (res != null) res.Close();
            }
        }

        [Test]
        public void GetPreSignedUriWithUserMetaPositiveTest()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            var now = DateTime.Now;
            //set expiration time to 5 seconds later
            var expireDate = now.AddSeconds(5);
            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, _objectKey);
            gpuRequest.Expiration = expireDate;
            gpuRequest.UserMetadata.Add("name1", "vaue1");
            gpuRequest.UserMetadata.Add("name2", "vaue2");
            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            try
            {
                req = WebRequest.Create(uri) as HttpWebRequest;
                req.Method = "GET";
                req.Headers.Add("x-oss-meta-name1", "vaue1");
                req.Headers.Add("x-oss-meta-name2", "vaue2");
                res = req.GetResponse() as HttpWebResponse;
                Assert.AreEqual(HttpStatusCode.OK, res.StatusCode, "response status code is not expected.");
                using (var stream = res.GetResponseStream())
                {
                    var actualETag = FileUtils.ComputeContentMd5(stream);
                    Assert.AreEqual(_objectETag.ToLowerInvariant(), actualETag.ToLowerInvariant());
                }
            }
            catch (WebException)
            {
                //TODO:
            }
            finally
            {
                if (req != null) req.Abort();
                if (res != null) res.Close();
            }
        }

        [Test]
        public void GetPreSignedUriWithResponseHeaderPositiveTest()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            var now = DateTime.Now;
            //set expiration time to 5 seconds later
            var expireDate = now.AddSeconds(5);
            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, _objectKey)
            {
                Expiration = expireDate,
                ResponseHeaders =
                {
                    CacheControl = "No-cache",
                    ContentType = "application/zip",
                    ContentDisposition = "myDownload.zip"
                }
            };
            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            try
            {
                req = WebRequest.Create(uri) as HttpWebRequest;
                req.Method = "GET";
                res = req.GetResponse() as HttpWebResponse;
                Assert.AreEqual(HttpStatusCode.OK, res.StatusCode, "response status code is not expected.");
                using (var stream = res.GetResponseStream())
                {
                    var actualETag = FileUtils.ComputeContentMd5(stream);
                    Assert.AreEqual(_objectETag.ToLowerInvariant(), actualETag.ToLowerInvariant());
                }
            }
            catch (WebException)
            {
                //TODO:
            }
            finally
            {
                if (req != null) req.Abort();
                if (res != null) res.Close();
            }
        }

        [Test]
        public void GetPreSignedUriFullSettingsPositiveTest()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            var now = DateTime.Now;
            //set expiration time to 5 seconds later
            var expireDate = now.AddSeconds(5);
            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, _objectKey)
            {
                Expiration = expireDate,
                ContentType = "application/zip",
                ContentMd5 = _objectETag,
                ResponseHeaders =
                {
                    CacheControl = "No-cache",
                    ContentType = "application/zip",
                    ContentDisposition = "myDownload.zip"
                }
            };

            gpuRequest.AddUserMetadata("name1", "vaue1");
            gpuRequest.AddUserMetadata("name2", "vaue2");
            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            try
            {
                req = WebRequest.Create(uri) as HttpWebRequest;
                req.Method = "GET";

                req.ContentType = "application/zip";
                req.Headers.Add(HttpRequestHeader.ContentMd5, _objectETag);

                req.Headers.Add("x-oss-meta-name1", "vaue1");
                req.Headers.Add("x-oss-meta-name2", "vaue2");
                res = req.GetResponse() as HttpWebResponse;
                Assert.AreEqual(HttpStatusCode.OK, res.StatusCode, "response status code is not expected.");
                using (var stream = res.GetResponseStream())
                {
                    var actualETag = FileUtils.ComputeContentMd5(stream);
                    Assert.AreEqual(_objectETag.ToLowerInvariant(), actualETag.ToLowerInvariant());
                }
            }
            catch (WebException)
            {
                //TODO:
            }
            finally
            {
                if (req != null) req.Abort();
                if (res != null) res.Close();
            }
        }

        [Test]
        public void GetObjectWithPreSignedAndAclParameter()
        {
            var targetObject = OssTestUtils.GetObjectKey(_className);
            OssTestUtils.UploadObject(_ossClient, _bucketName, targetObject, Config.UploadTestFile);
            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, targetObject, SignHttpMethod.Get)
            {
                Expiration = DateTime.Now.AddHours(1),
            };
            gpuRequest.AddQueryParam("acl", "");
            var uri = _ossClient.GeneratePresignedUri(gpuRequest);
            try
            {
                var getObjectResult = _ossClient.GetObject(uri);
                Assert.AreEqual(getObjectResult.HttpStatusCode, HttpStatusCode.OK);

                var xml = new XmlSerializer(typeof(AccessControlPolicy));
                var model = xml.Deserialize(getObjectResult.Content) as AccessControlPolicy;
                var acl = new AccessControlList();
                foreach (var grant in model.Grants)
                {
                    if (grant == EnumUtils.GetStringValue(CannedAccessControlList.PublicRead))
                    {
                        acl.GrantPermission(GroupGrantee.AllUsers, Permission.Read);
                        acl.ACL = CannedAccessControlList.PublicRead;
                    }
                    else if (grant == EnumUtils.GetStringValue(CannedAccessControlList.PublicReadWrite))
                    {
                        acl.GrantPermission(GroupGrantee.AllUsers, Permission.FullControl);
                        acl.ACL = CannedAccessControlList.PublicReadWrite;
                    }
                    else if (grant == EnumUtils.GetStringValue(CannedAccessControlList.Private))
                    {
                        acl.ACL = CannedAccessControlList.Private;
                    }
                    else if (grant == EnumUtils.GetStringValue(CannedAccessControlList.Default))
                    {
                        acl.ACL = CannedAccessControlList.Default;
                    }
                }

                var acl1 = _ossClient.GetObjectAcl(_bucketName, targetObject);
                Assert.AreEqual(acl1.ACL, acl.ACL);
            }
            catch (OssException e)
            {
                Assert.Fail(e.Message);
            }
        }
        #endregion

        #region PUT methods
        [Test]
        public void PutPreSignedUriDefaultPositiveTest()
        {
            var testStr = FileUtils.GenerateOneKb();
            var bytes = Encoding.ASCII.GetBytes(testStr);
            //calculate the expected ETag
            var expectedETag = FileUtils.ComputeContentMd5(new MemoryStream(bytes));

            HttpWebRequest req = null;
            HttpWebResponse res = null;

            var now = DateTime.Now;
            //set expiration time to 5 seconds later
            var expireDate = now.AddSeconds(5);
            var targetObject = OssTestUtils.GetObjectKey(_className);
            var uri = _ossClient.GeneratePresignedUri(_bucketName, targetObject, expireDate, SignHttpMethod.Put);
            try
            {
                req = WebRequest.Create(uri) as HttpWebRequest;
                req.Method = "PUT";
                using (var stream = req.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                try
                {
                    res = req.GetResponse() as HttpWebResponse;
                    Assert.AreEqual(HttpStatusCode.OK, res.StatusCode, "response status code is not expected.");
                    //get the uploaded object ETag
                    var actualETag = _ossClient.GetObjectMetadata(_bucketName, targetObject).ETag;
                    Assert.AreEqual(expectedETag.ToLowerInvariant(), actualETag.ToLowerInvariant(),
                        "Uploaded object ETag value is not expected");
                }
                catch (WebException)
                {
                    //TODO:
                }
            }
            finally
            {
                if (req != null) req.Abort();
                if (res != null) res.Close();
            }
        }

        [Test]
        public void PutObjectWithPreSignedUriAndCrc()
        {
            var testStr = FileUtils.GenerateOneKb();
            var bytes = Encoding.ASCII.GetBytes(testStr);
         
            var now = DateTime.Now;
            //set expiration time to 5 seconds later
            var expireDate = now.AddSeconds(5);
            var targetObject = OssTestUtils.GetObjectKey(_className);
            var uri = _ossClient.GeneratePresignedUri(_bucketName, targetObject, expireDate, SignHttpMethod.Put);
            try
            {
                var putResult = _ossClient.PutObject(uri, new MemoryStream(bytes));
                Assert.AreEqual(putResult.HttpStatusCode, HttpStatusCode.OK);
            }
            catch (WebException ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [Test]
        public void PutObjectWithPreSignedUriWithoutCrc()
        {
            var testStr = FileUtils.GenerateOneKb();
            var bytes = Encoding.ASCII.GetBytes(testStr);

            var now = DateTime.Now;
            //set expiration time to 5 seconds later
            var expireDate = now.AddSeconds(5);
            var targetObject = OssTestUtils.GetObjectKey(_className);
            var uri = _ossClient.GeneratePresignedUri(_bucketName, targetObject, expireDate, SignHttpMethod.Put);
            try
            {
                Common.ClientConfiguration config = new Common.ClientConfiguration();
                config.EnableCrcCheck = false;
                IOss ossClient = OssClientFactory.CreateOssClient(config);
                var putResult = ossClient.PutObject(uri, new MemoryStream(bytes));
                Assert.AreEqual(putResult.HttpStatusCode, HttpStatusCode.OK);
            }
            catch (WebException ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [Test]
        public void PutPreSignedUriDefaultNegativeTest()
        {
            var testStr = FileUtils.GenerateOneKb();
            var bytes = Encoding.ASCII.GetBytes(testStr);

            HttpWebRequest req = null;
            HttpWebResponse res = null;

            var now = DateTime.Now;
            //set expiration time to 5 seconds before
            var expireDate = now.AddSeconds(-5);
            var targetObject = OssTestUtils.GetObjectKey(_className);
            var uri = _ossClient.GeneratePresignedUri(_bucketName, targetObject, expireDate, SignHttpMethod.Put);
            try
            {
                req = WebRequest.Create(uri) as HttpWebRequest;
                req.Method = "PUT";
                using (var stream = req.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                res = req.GetResponse() as HttpWebResponse;
                Assert.Fail("response fail for expired URI");
            }
            catch (WebException e)
            {
                Assert.IsTrue(e.Message.Contains("403"),
                    string.Format("Unexpected exception: {0}", e.Message));
            }
            finally
            {
                if (req != null) req.Abort();
                if (res != null) res.Close();
            }
        }
        
        [Test]
        public void PutObjectWithPreSignedUriWithParameter()
        {
            var targetObject = OssTestUtils.GetObjectKey(_className);
            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, targetObject, SignHttpMethod.Put)
            {
                ContentType = "text/rtf",
                Expiration = DateTime.Now.AddHours(1),
            };
            gpuRequest.UserMetadata.Add("Author", "oss");
            gpuRequest.UserMetadata.Add("Test", "test");
            gpuRequest.AddQueryParam("x-param-null", "");
            gpuRequest.AddQueryParam("x-param-space0", " ");
            gpuRequest.AddQueryParam("x-param-value", "value");
            gpuRequest.AddQueryParam("x-param-space1", " ");

            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            var metadata = new ObjectMetadata();
            metadata.ContentType = "text/rtf";
            metadata.UserMetadata.Add("Author", "oss");
            metadata.UserMetadata.Add("Test", "test");

            var putObjectResult = _ossClient.PutObject(uri, Config.UploadTestFile, metadata);
            Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.OK);
        }

        [Test]
        public void PutObjectWithPreSignedUriWithParameterNegativeTest()
        {
            var targetObject = OssTestUtils.GetObjectKey(_className);
            var gpuRequest = new GeneratePresignedUriRequest(_bucketName, targetObject, SignHttpMethod.Put)
            {
                ContentType = "text/rtf",
                Expiration = DateTime.Now.AddHours(1),
            };
            gpuRequest.UserMetadata.Add("Author", "oss");
            gpuRequest.UserMetadata.Add("Test", "test");
            gpuRequest.AddQueryParam("x-param-null", "");
            gpuRequest.AddQueryParam("x-param-space0", " ");
            gpuRequest.AddQueryParam("x-param-value", "value");
            gpuRequest.AddQueryParam("x-param-space1", " ");

            var uri = _ossClient.GeneratePresignedUri(gpuRequest);

            var metadata = new ObjectMetadata();
            metadata.ContentType = "text/rtf";
            metadata.UserMetadata.Add("Author", "oss");
            metadata.UserMetadata.Add("Test", "test1");
            try
            {
                var putObjectResult = _ossClient.PutObject(uri, Config.UploadTestFile, metadata);
                Assert.AreEqual(putObjectResult.HttpStatusCode, HttpStatusCode.Forbidden);
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.SignatureDoesNotMatch, e.ErrorCode);
            }
        }
        #endregion

        #region Misc Test
        [Test]
        public void GeneratePresignedUriWithExecptionTest()
        {
            try
            {
                var targetObject = OssTestUtils.GetObjectKey(_className);
                var gpuRequest = new GeneratePresignedUriRequest(_bucketName, targetObject, SignHttpMethod.Head)
                {
                    ContentType = "text/rtf",
                    Expiration = DateTime.Now.AddHours(1),
                };
                gpuRequest.UserMetadata.Add("Author", "oss");
                gpuRequest.UserMetadata.Add("Test", "test");
                gpuRequest.AddQueryParam("x-param-null", "");
                gpuRequest.AddQueryParam("x-param-space0", " ");
                gpuRequest.AddQueryParam("x-param-value", "value");
                gpuRequest.AddQueryParam("x-param-space1", " ");
                var uri = _ossClient.GeneratePresignedUri(gpuRequest);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void GeneratePresignedUriStsTokenTest()
        {
            var targetObject = OssTestUtils.GetObjectKey(_className);
            try
            {
                var client = new OssClient(Config.Endpoint, 
                    new Common.Authentication.DefaultCredentialsProvider(
                        new Common.Authentication.DefaultCredentials(Config.AccessKeyId, Config.AccessKeySecret, "test-token")));

                var gpuRequest = new GeneratePresignedUriRequest(_bucketName, targetObject, SignHttpMethod.Get)
                {
                    ContentType = "text/rtf",
                    Expiration = DateTime.Now.AddHours(1),
                };
                var uri = client.GeneratePresignedUri(gpuRequest);
                Assert.AreNotEqual(uri.ToString().IndexOf("security-token=test-token"), -1);
            }
            catch
            {
                Assert.IsTrue(false);
            }

            var request = new GeneratePresignedUriRequest(_bucketName, targetObject, SignHttpMethod.Get)
            {
                ContentType = "text/rtf",
                Expiration = DateTime.Now.AddHours(1),
            };
            request.BucketName = null;
            _ossClient.GeneratePresignedUri(request);

            request.BucketName = _bucketName;
            request.Key = targetObject;
            _ossClient.GeneratePresignedUri(request);
        }
        #endregion
    }
}
