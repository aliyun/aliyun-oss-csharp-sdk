using System;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectCopyTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _sourceObjectKey;
        private static string _sourceObjectETag;
        private static string _sourceBigObjectKey;
        private static string _sourceBigObjectETag;

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
            _sourceObjectETag = poResult.ETag;

            //upload multipart sample object as source object
            _sourceBigObjectKey = _sourceObjectKey + ".js";
            metadata = new ObjectMetadata();
            poResult = OssTestUtils.UploadObject(_ossClient, _bucketName, _sourceBigObjectKey,
                Config.MultiUploadTestFile, metadata);
            _sourceBigObjectETag = poResult.ETag;
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void CopyObjectBasicTest()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            //construct metadata
            var metadata = new ObjectMetadata();
            const string userMetaKey = "myKey";
            const string userMetaValue = "myValue";
            metadata.UserMetadata.Add(userMetaKey, userMetaValue);
            metadata.CacheControl = "No-Cache";

            var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, targetObjectKey)
            {
                NewObjectMetadata = metadata
            };

            //copy object
            _ossClient.CopyObject(coRequest);
            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));
            var resMetadata = _ossClient.GetObjectMetadata(_bucketName, targetObjectKey);
            Assert.AreEqual(userMetaValue, resMetadata.UserMetadata[userMetaKey]);

            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }

        [Test]
        public void CopyObjectBasicTestWithInvalidBucket()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            //construct metadata
            var metadata = new ObjectMetadata();
            const string userMetaKey = "myKey";
            const string userMetaValue = "myValue";
            metadata.UserMetadata.Add(userMetaKey, userMetaValue);
            metadata.CacheControl = "No-Cache";

            try
            {
                var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, "/invalid_bucket", targetObjectKey)
                {
                    NewObjectMetadata = metadata
                };

                _ossClient.CopyObject(coRequest);

                Assert.Fail("Copy object should not pass with invalid bucket name");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            } 
        }

        [Test]
        public void CopyObjectBasicTestWithInvalidObject()
        {
            //construct metadata
            var metadata = new ObjectMetadata();
            const string userMetaKey = "myKey";
            const string userMetaValue = "myValue";
            metadata.UserMetadata.Add(userMetaKey, userMetaValue);
            metadata.CacheControl = "No-Cache";

            try
            {
                var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, "/__%")
                {
                    NewObjectMetadata = metadata
                };

                _ossClient.CopyObject(coRequest);

                Assert.Fail("Copy object should not pass with invalid object name");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            } 
        }

        [Test]
        public void CopyObjectMatchingETagPositiveTest()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, targetObjectKey);
            coRequest.MatchingETagConstraints.Add(_sourceObjectETag);

            _ossClient.CopyObject(coRequest);
            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }

        [Test]
        public void CopyObjectMatchingETagNegativeTest()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, targetObjectKey);
            coRequest.MatchingETagConstraints.Add("Dummy");

            try
            {
                _ossClient.CopyObject(coRequest);
                Assert.Fail("Copy object should not pass with MatchingETag set to wrong value");
            }
            catch(OssException e)
            {
                Assert.AreEqual(OssErrorCode.PreconditionFailed, e.ErrorCode);
            }
            Assert.IsFalse(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));
        }

        [Test]
        public void CopyObjectNonMatchingETagPositiveTest()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, targetObjectKey);
            coRequest.NonmatchingETagConstraints.Add("Dummy");

            _ossClient.CopyObject(coRequest);
            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }

        [Test]
        public void CopyObjectNonMatchingETagNegativeTest()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, targetObjectKey);
            coRequest.NonmatchingETagConstraints.Add(_sourceObjectETag);

            try
            {
                _ossClient.CopyObject(coRequest);
                Assert.Fail("Copy object should not pass with NonMatchingETag set to correct value");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.NotModified, e.ErrorCode);
            }
            Assert.IsFalse(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));
        }

        [Test]
        public void CopyObjectModifiedSincePositiveTest()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, targetObjectKey);
            coRequest.ModifiedSinceConstraint = DateTime.Now.AddDays(-1);

            _ossClient.CopyObject(coRequest);
            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }

        [Test]
        public void CopyObjectModifiedSinceNegativeTest()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, targetObjectKey)
            {
                ModifiedSinceConstraint = DateTime.Now.AddDays(1)
            };

            try
            {
                _ossClient.CopyObject(coRequest);
                Assert.Fail("Copy object should not pass with NonMatchingETag set to correct value");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.NotModified, e.ErrorCode);
            }
            Assert.IsFalse(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));
        }

        [Test]
        public void CopyObjectUnmodifiedSincePositiveTest()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, targetObjectKey)
            {
                UnmodifiedSinceConstraint = DateTime.Now.AddDays(1)
            };

            _ossClient.CopyObject(coRequest);
            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }

        [Test]
        public void CopyObjectUnmodifiedSinceNegativeTest()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, targetObjectKey)
            {
                UnmodifiedSinceConstraint = DateTime.Now.AddDays(-1)
            };

            try
            {
                _ossClient.CopyObject(coRequest);
                Assert.Fail("Copy object should not pass with NonMatchingETag set to correct value");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.PreconditionFailed, e.ErrorCode);
            }
            Assert.IsFalse(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));
        }

        [Test]
        public void ResumableCopyObjectTestWithFileLengthLessThanPartSize() 
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);
            try
            {
                var copyRequest = new CopyObjectRequest(_bucketName, _sourceBigObjectKey, _bucketName, targetObjectKey);
                copyRequest.MatchingETagConstraints.Add(_sourceBigObjectETag);

                var sourceObjectMeta = _ossClient.GetObjectMetadata(_bucketName, _sourceBigObjectKey);

                _ossClient.ResumableCopyObject(copyRequest, null, sourceObjectMeta.ContentLength + 1);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

                var targetObjectMeta = _ossClient.GetObjectMetadata(_bucketName, targetObjectKey);
                Assert.AreEqual(sourceObjectMeta.ContentLength, targetObjectMeta.ContentLength);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, targetObjectKey);
            } 
        }

        [Test]
        public void ResumableCopyObjectTestWithFileLengthEqualPartSize() 
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);
            try
            {
                var copyRequest = new CopyObjectRequest(_bucketName, _sourceBigObjectKey, _bucketName, targetObjectKey);
                copyRequest.MatchingETagConstraints.Add(_sourceBigObjectETag);

                var sourceObjectMeta = _ossClient.GetObjectMetadata(_bucketName, _sourceBigObjectKey);

                _ossClient.ResumableCopyObject(copyRequest, null, sourceObjectMeta.ContentLength);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

                var targetObjectMeta = _ossClient.GetObjectMetadata(_bucketName, targetObjectKey);
                Assert.AreEqual(sourceObjectMeta.ContentLength, targetObjectMeta.ContentLength);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, targetObjectKey);
            }
        }

        [Test]
        public void ResumableCopyObjectTestWithFileLengthMoreThanPartSize()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);
            try
            {
                var copyRequest = new CopyObjectRequest(_bucketName, _sourceBigObjectKey, _bucketName, targetObjectKey);
                copyRequest.MatchingETagConstraints.Add(_sourceBigObjectETag);

                var sourceObjectMeta = _ossClient.GetObjectMetadata(_bucketName, _sourceBigObjectKey);

                _ossClient.ResumableCopyObject(copyRequest, null, sourceObjectMeta.ContentLength - 10);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

                var targetObjectMeta = _ossClient.GetObjectMetadata(_bucketName, targetObjectKey);
                Assert.AreEqual(sourceObjectMeta.ContentLength, targetObjectMeta.ContentLength);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, targetObjectKey);
            }
        }

        [Test]
        public void ResumableCopyObjectTestWithDefaultPartSize()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);
            try
            {
                var copyRequest = new CopyObjectRequest(_bucketName, _sourceBigObjectKey, _bucketName, targetObjectKey);
                copyRequest.MatchingETagConstraints.Add(_sourceBigObjectETag);

                var sourceObjectMeta = _ossClient.GetObjectMetadata(_bucketName, _sourceBigObjectKey);

                _ossClient.ResumableCopyObject(copyRequest, Config.DownloadFolder);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

                var targetObjectMeta = _ossClient.GetObjectMetadata(_bucketName, targetObjectKey);
                Assert.AreEqual(sourceObjectMeta.ContentLength, targetObjectMeta.ContentLength);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, targetObjectKey);
            }
        }

        [Test]
        public void ResumableCopyObjectTest()
        { 
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);
            try
            {
                var copyRequest = new CopyObjectRequest(_bucketName, _sourceBigObjectKey, _bucketName, targetObjectKey);

                var sourceObjectMeta = _ossClient.GetObjectMetadata(_bucketName, _sourceBigObjectKey);

                _ossClient.ResumableCopyObject(copyRequest, Config.DownloadFolder);

                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));

                var targetObjectMeta = _ossClient.GetObjectMetadata(_bucketName, targetObjectKey);
                Assert.AreEqual(sourceObjectMeta.ContentLength, targetObjectMeta.ContentLength);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, targetObjectKey);
            }
        }

        [Test]
        public void AsyncCopyObjectBasicTest()
        {
            var targetObjectKey = OssTestUtils.GetObjectKey(_className);

            //construct metadata
            var metadata = new ObjectMetadata();
            const string userMetaKey = "myKey";
            const string userMetaValue = "myValue";
            metadata.UserMetadata.Add(userMetaKey, userMetaValue);
            metadata.CacheControl = "No-Cache";

            var coRequest = new CopyObjectRequest(_bucketName, _sourceObjectKey, _bucketName, targetObjectKey)
            {
                NewObjectMetadata = metadata
            };

            //copy object
            _ossClient.CopyObject(coRequest);
            var asyncResult = _ossClient.BeginCopyObject(coRequest, null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            var coResult = _ossClient.EndCopyResult(asyncResult);

            Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, targetObjectKey));
            var resMetadata = _ossClient.GetObjectMetadata(_bucketName, targetObjectKey);
            Assert.AreEqual(userMetaValue, resMetadata.UserMetadata[userMetaKey]);

            _ossClient.DeleteObject(_bucketName, targetObjectKey);
        }
    }
}
