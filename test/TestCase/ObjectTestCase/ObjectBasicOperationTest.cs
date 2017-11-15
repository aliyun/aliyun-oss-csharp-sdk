using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;
using System.Text;
using System.Net;
using System.Threading;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectBasicOperationTest
    {
        private static IOss _ossClient;
        private static IOss _ossClientMd5;
        private static string _className;
        private static string _bucketName;
        private static string _objectKey;
        private static string _objectETag;
        private static AutoResetEvent _event;

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            _ossClientMd5 = OssClientFactory.CreateOssClientEnableMD5(true);
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

            _event = new AutoResetEvent(false);
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        #region stream upload

        [Test]
        public void UploadObjectBasicSettingsTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                //upload the object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadTestFile, new ObjectMetadata());
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
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
        public void UploadObjectBasicSettingsAsyncTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                AutoResetEvent finished = new AutoResetEvent(false);
                //upload the object
                OssTestUtils.BeginUploadObject(_ossClient, _bucketName, key,
                                               Config.UploadTestFile,
                                               (ar) => { 
                                                OssTestUtils.EndUploadObject(_ossClient, ar); 
                                                finished.Set();
                                               },
                                               null);
                finished.WaitOne(1000 * 60); // wait for up to 1 min;
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
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
        public void UploadUnSeekableStreamTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(new byte[1024], 0, 1024);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);

                    // upload the nonseekable stream, with MD5 enabled
                    _ossClientMd5.PutObject(_bucketName, key, new NonSeekableStream(ms));
                    Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
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
        public void UploadStreamWithChunkedEncodingTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(new byte[1024], 0, 1024);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);

                    PutObjectRequest putRequest = new PutObjectRequest(_bucketName, key, ms, null, true);
                    // upload the nonseekable stream, with MD5 enabled
                    _ossClient.PutObject(putRequest);
                    Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                    OssObject obj = _ossClient.GetObject(_bucketName, key);
                    Assert.AreEqual(obj.ContentLength, (long)1024);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
        public void DownloadAndUploadStreamWithChunkedEncodingTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var obj = _ossClient.GetObject(_bucketName, _objectKey);
                Assert.IsNotNull(obj.Content);
                PutObjectRequest putRequest = new PutObjectRequest(_bucketName, key, obj.Content, null, true);
                _ossClient.PutObject(putRequest);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                OssObject obj2 = _ossClient.GetObject(_bucketName, key);
                Assert.AreEqual(obj.ContentLength, obj2.ContentLength);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Assert.Fail(e.ToString());
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
        public void UploadObjectNullMetadataTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                //upload the object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadTestFile, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
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
        public void UploadObjectDefaultMetadataTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                //upload the object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadTestFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
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
        public void UploadObjectWithMd5()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                string md5;
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    md5 = OssUtils.ComputeContentMd5(fs, fs.Length);
                }

                var meta = new ObjectMetadata() { ContentMd5 = md5 };

                //upload the object
                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
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
        public void UploadObjectFullSettingsTest()
        {
            //test folder structure
            var folder = OssTestUtils.GetObjectKey(_className);
            var key = folder + "/" + OssTestUtils.GetObjectKey(_className);

            //config metadata
            var metadata = new ObjectMetadata
            {
                CacheControl = "no-cache",
                ContentDisposition = "abc.zip",
                ContentEncoding = "gzip"
            };
            var eTag = FileUtils.ComputeContentMd5(Config.UploadTestFile);
            metadata.ETag = eTag;
            //enable server side encryption
            const string encryption = "AES256";
            metadata.ServerSideEncryption = encryption;
            //user metadata
            metadata.UserMetadata.Add("MyKey1", "MyValue1");
            metadata.UserMetadata.Add("MyKey2", "MyValue2");

            try
            {
                //upload object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadTestFile, metadata);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var uploadedObjectMetadata = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(eTag.ToLowerInvariant(), uploadedObjectMetadata.ETag.ToLowerInvariant());
                Assert.AreEqual(encryption, uploadedObjectMetadata.ServerSideEncryption);
                Assert.AreEqual(2, uploadedObjectMetadata.UserMetadata.Count);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        #endregion

        #region file upload

        [Test]
        public void UploadObjectSpecifyFilePathTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile, new ObjectMetadata());
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
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
        public void UploadObjectSpecifyFilePathDefaultMetadataTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
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
        public void ResumableUploadObjectTestWithObjectLessThanPartSize()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadTestFile);
                var fileSize = fileInfo.Length;

                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(), null, fileSize + 1);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
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
        public void ResumableUploadObjectTestWithObjectEqualPartSize()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadTestFile);
                var fileSize = fileInfo.Length;

                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(), null, fileSize);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
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
        public void ResumableUploadObjectTestWithObjectMoreThanPartSize()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadTestFile);
                var fileSize = fileInfo.Length;

                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(), null, fileSize - 1);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
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
        public void ResumableUploadObjectTestWithObjectPartSizeTooSmall()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadTestFile);
                var fileSize = fileInfo.Length;

                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(), Config.DownloadFolder, 1);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
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
        public void ResumableUploadObjectTestWithSmallObjectCheckContentType()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var newFileName = Path.GetDirectoryName(Config.UploadTestFile) + "/newfile.js";

            try
            {
                File.Copy(Config.UploadTestFile, newFileName);

                var result = _ossClient.ResumableUploadObject(_bucketName, key, newFileName, new ObjectMetadata(), null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);

                var objectMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/x-javascript", objectMeta.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
                File.Delete(newFileName);
            }
        }

        [Test]
        public void ResumableUploadObjectTestWithBigObjectCheckContentType()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var newFileName = Path.GetDirectoryName(Config.MultiUploadTestFile) + "/newfile.js";

            try
            {
                File.Copy(Config.MultiUploadTestFile, newFileName, true);
                var fileInfo = new FileInfo(newFileName);
                var fileSize = fileInfo.Length;

                var result = _ossClient.ResumableUploadObject(_bucketName, key, newFileName, new ObjectMetadata(), Config.DownloadFolder, fileSize / 3);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);

                var objectMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/x-javascript", objectMeta.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
                File.Delete(newFileName);
            }
        }

        #endregion

        #region invalid key
        [Test]
        public void InvalidObjectKeyTest()
        {
            foreach (var invalidKeyName in OssTestUtils.InvalidObjectKeyNamesList)
            {
                try
                {
                    //try to upload the object with invalid key name
                    OssTestUtils.UploadObject(_ossClient, _bucketName, invalidKeyName,
                        Config.UploadTestFile, new ObjectMetadata());
                    Assert.Fail("Upload should not pass for invalid object key name {0}", invalidKeyName);
                }
                catch (ArgumentException)
                {
                    Assert.IsTrue(true);
                }
            }
        }

        #endregion

        #region get object

        [Test]
        public void GetAndDeleteNonExistObjectTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            //non exist object
            Assert.IsFalse(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
            try
            {
                _ossClient.GetObject(_bucketName, key);
                Assert.Fail("Get non exist object should not pass");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.NoSuchKey, e.ErrorCode);
            }
            //according to API doc, delete non-exist object return 204
            _ossClient.DeleteObject(_bucketName, key);
        }

        [Test]
        public void GetObjectBasicTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            try
            {
                OssTestUtils.DownloadObject(_ossClient, _bucketName, _objectKey, targetFile);
                var expectedETag = _ossClient.GetObjectMetadata(_bucketName, _objectKey).ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            }
            finally
            {
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
        public void GetObjectUsingRangeTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            try
            {
                OssTestUtils.DownloadObjectUsingRange(_ossClient, _bucketName, _objectKey, targetFile);
                var expectedETag = _ossClient.GetObjectMetadata(_bucketName, _objectKey).ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            }
            finally
            {
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
        public void ListAllObjectsTest()
        {
            //test folder structure
            var folderName = OssTestUtils.GetObjectKey(_className);
            var key = folderName + "/" + OssTestUtils.GetObjectKey(_className);

            try
            {
                //upload the object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadTestFile, new ObjectMetadata());

                //list objects by specifying bucket name
                var allObjects = _ossClient.ListObjects(_bucketName);
                //default value is 100
                Assert.AreEqual(100, allObjects.MaxKeys);
                var allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                //there is already one sample object
                Assert.AreEqual(2, allObjectsSumm.Count);

                //list objects by specifying folder as prefix
                allObjects = _ossClient.ListObjects(_bucketName, folderName);
                Assert.AreEqual(folderName, allObjects.Prefix);
                allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                Assert.AreEqual(1, allObjectsSumm.Count);

                var loRequest = new ListObjectsRequest(_bucketName);
                loRequest.Prefix = folderName;
                loRequest.MaxKeys = 10;
                loRequest.Delimiter = "/";
                allObjects = _ossClient.ListObjects(loRequest);
                Assert.AreEqual(folderName, allObjects.Prefix);
                Assert.AreEqual("/", allObjects.Delimiter);
                //Assert.Fail("List objects using full conditions");
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void GetObjectMatchingETagPositiveTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.MatchingETagConstraints.Add(_objectETag);

            _ossClient.GetObject(coRequest);
        }

        [Test]
        public void GetObjectMatchingETagNegativeTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.MatchingETagConstraints.Add("Dummy");

            try
            {
                _ossClient.GetObject(coRequest);
                Assert.Fail("Get object should not pass with MatchingETag set to wrong value");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.PreconditionFailed, e.ErrorCode);
            }
        }

        [Test]
        public void GetObjectModifiedSincePositiveTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.ModifiedSinceConstraint = DateTime.UtcNow.AddDays(-1);

            _ossClient.GetObject(coRequest);
        }

        [Test]
        public void GetObjectModifiedSinceNegativeTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey)
            {
                ModifiedSinceConstraint = DateTime.UtcNow.AddDays(1)
            };

            try
            {
                _ossClient.GetObject(coRequest);
                Assert.Fail("Get object should not pass with ModifiedSince set to wrong value");
            }
            //according to API, return 304
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.NotModified, e.ErrorCode);
            }
        }

        [Test]
        public void GetObjectNonMatchingETagPositiveTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.NonmatchingETagConstraints.Add("Dummy");

            _ossClient.GetObject(coRequest);
        }

        [Test]
        public void GetObjectNonMatchingETagNegativeTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.NonmatchingETagConstraints.Add(_objectETag);

            try
            {
                _ossClient.GetObject(coRequest);
                Assert.Fail("Get object should not pass with NonMatchingETag set to wrong value");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.NotModified, e.ErrorCode);
            }
        }

        [Test]
        public void GetObjectUnmodifiedSincePositiveTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.UnmodifiedSinceConstraint = DateTime.UtcNow.AddDays(1);

            _ossClient.GetObject(coRequest);
        }

        [Test]
        public void GetObjectUnmodifiedSinceNegativeTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.UnmodifiedSinceConstraint = DateTime.UtcNow.AddDays(-1);

            try
            {
                _ossClient.GetObject(coRequest);
                Assert.Fail("Get object should not pass with UnmodifiedSince set to wrong value");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.PreconditionFailed, e.ErrorCode);
            }
        }

        #endregion

        #region delete object

        [Test]
        public void DeleteMultiObjectsVerboseTest()
        {
            var count = new Random().Next(2, 20);
            LogUtility.LogMessage("Will create {0} objects for multi delete this time", count);
            var objectkeys = CreateMultiObjects(count);
            LogUtility.LogMessage("{0} objects have been created", count);

            var doRequest = new DeleteObjectsRequest(_bucketName, objectkeys, false);
            var doResponse = _ossClient.DeleteObjects(doRequest);
            //verbose mode would return all object keys
            Assert.AreEqual(count, doResponse.Keys.Length);
        }

        [Test]
        public void DeleteMultiObjectsQuietTest()
        {
            var count = new Random().Next(2, 20);
            LogUtility.LogMessage("Will create {0} objects for multi delete this time", count);
            var objectkeys = CreateMultiObjects(count);
            LogUtility.LogMessage("{0} objects have been created", count);

            var doRequest = new DeleteObjectsRequest(_bucketName, objectkeys);
            var doResponse = _ossClient.DeleteObjects(doRequest);
            //quiet mode won't return object keys
            Assert.IsNull(doResponse.Keys);
        }

        [Test]
        public void UploadObjectWithNoSurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className) + "";
            try
            {
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    _ossClient.PutObject(_bucketName, key, fs, null);
                }

                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/octet-stream", result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        #endregion

        #region content-type

        [Test]
        public void UploadObjectWithGenerateContentTypeByKeySurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className) + ".js";
            try
            {
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    _ossClient.PutObject(_bucketName, key, fs, null);
                }

                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/x-javascript", result.Metadata.ContentType);
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
        public void UploadObjectWithKeySurfixAndMetadata()
        {
            var key = OssTestUtils.GetObjectKey(_className) + ".js";
            try
            {
                var metadata = new ObjectMetadata();
                metadata.ContentType = "application/vnd.android.package-archive";

                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    _ossClient.PutObject(_bucketName, key, fs, metadata);
                }

                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/vnd.android.package-archive", result.Metadata.ContentType);
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
        public void UploadObjectWithKeySurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className) + ".js";
            var newFileName = Path.GetDirectoryName(Config.UploadTestFile) + "/newfile";
            try
            {
                File.Copy(Config.UploadTestFile, newFileName, true);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key, newFileName, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/octet-stream", result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                    File.Delete(newFileName);
                }
            }
        }

        [Test]
        public void UploadObjectWithFileSurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var newFileName = Path.GetDirectoryName(Config.UploadTestFile) + "/newfile.js";

            try
            {
                File.Copy(Config.UploadTestFile, newFileName, true);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key, newFileName, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/x-javascript", result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                    File.Delete(newFileName);
                }
            }
        }

        [Test]
        public void UploadObjectWithFileSurfix2()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var newFileName = Path.GetDirectoryName(Config.UploadTestFile) + "/newfile.m4u";

            try
            {
                File.Copy(Config.UploadTestFile, newFileName, true);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key, newFileName, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("video/vnd.mpegurl", result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                    File.Delete(newFileName);
                }
            }
        }

        [Test]
        public void UploadObjectWithFileSurfixAndFileSurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className) + ".potx";
            var newFileName = Path.GetDirectoryName(Config.UploadTestFile) + "/newFile.docx";

            try
            {
                File.Copy(Config.UploadTestFile, newFileName, true);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key, newFileName, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                                result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                    File.Delete(newFileName);
                }
            }
        }

        #endregion

        #region does object exist

        [Test]
        public void DoesObjectExistWithBucketNotExist()
        {
            try
            {
                const string bucketName = "not-exist-bucket";
                const string key = "one";
                try
                {
                    _ossClient.DeleteBucket(bucketName);
                }
                catch (Exception)
                {
                    //nothing
                }

                bool isExist = _ossClient.DoesObjectExist(bucketName, key);
                Assert.False(isExist);
            }
            catch (Exception e)
            {
                Assert.True(false, e.Message);
            }
        }

        [Test]
        public void DoesObjectExistWithBucketExistAndObjectNotExist()
        {
            try
            {
                string bucketName = _bucketName;
                const string key = "one";
                try
                {
                    _ossClient.DeleteObject(bucketName, key);
                }
                catch (Exception)
                {
                    //nothing
                }

                bool isExist = _ossClient.DoesObjectExist(bucketName, key);
                Assert.False(isExist);
            }
            catch (Exception e)
            {
                Assert.True(false, e.Message);
            }
        }

        [Test]
        public void DoesObjectExistWithObjecttExist()
        {
            string bucketName = _bucketName;
            const string key = "one";

            try
            {
                try
                {
                    _ossClient.PutObject(bucketName, key, Config.UploadTestFile);
                }
                catch (Exception)
                {
                    //nothing
                }

                bool isExist = _ossClient.DoesObjectExist(bucketName, key);
                Assert.True(isExist);
            }
            catch (Exception e)
            {
                Assert.True(false, e.Message);
            }
            finally
            {
                _ossClient.DeleteObject(bucketName, key);
            }
        }

        #endregion

        #region modify object meta

        /// <summary>
        /// Modify the value in metadata from A to B
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithA2B()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf"
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);

                meta.ContentType = "application/mac-binhex40";
                _ossClient.ModifyObjectMeta(_bucketName, key, meta);

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, newMeta.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// Modify the metadata value from A to A
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithA2A()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf"
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);

                _ossClient.ModifyObjectMeta(_bucketName, key, meta);

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, newMeta.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// Adds a new metadata key C and remove one old key B.
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithAB2C()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf",
                    CacheControl = "public"
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);
                Assert.AreEqual(meta.CacheControl, oldMeta.CacheControl);

                _ossClient.ModifyObjectMeta(_bucketName, key, new ObjectMetadata() { ContentType = "application/vnd.wap.wmlc" });

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/vnd.wap.wmlc", newMeta.ContentType);
                Assert.AreEqual(null, newMeta.CacheControl);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// Updates the metadata value of A to C and adds a new metadata key B
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithA2CB()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf",
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);
                Assert.AreEqual(meta.CacheControl, oldMeta.CacheControl);

                var toModifyMeta = new ObjectMetadata()
                {
                    ContentType = "application/vnd.wap.wmlc",
                    CacheControl = "max-stale"
                };
                _ossClient.ModifyObjectMeta(_bucketName, key, toModifyMeta);

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/vnd.wap.wmlc", newMeta.ContentType);
                Assert.AreEqual("max-stale", newMeta.CacheControl);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// Update the metadata key which has more than max size key length.
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithKeySizeTooLong()
        {
            var key = OssTestUtils.GetObjectKey(_className) + OssTestUtils.GetRandomString(512);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf",
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);
                Assert.AreEqual(meta.CacheControl, oldMeta.CacheControl);

                var toModifyMeta = new ObjectMetadata()
                {
                    ContentType = "application/vnd.wap.wmlc",
                    CacheControl = "max-stale"
                };
                _ossClient.ModifyObjectMeta(_bucketName, key, toModifyMeta, 100 * 1024);

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/vnd.wap.wmlc", newMeta.ContentType);
                Assert.AreEqual("max-stale", newMeta.CacheControl);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// Clear the metadata
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithToEmpty() 
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf",
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);
                Assert.AreEqual(meta.CacheControl, oldMeta.CacheControl);

                _ossClient.ModifyObjectMeta(_bucketName, key, new ObjectMetadata());

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/octet-stream", newMeta.ContentType);
                Assert.AreEqual(null, newMeta.CacheControl);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// Sets a new metadata, use default content-type.
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithAddMeta() 
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var newFileName = Path.GetDirectoryName(Config.UploadTestFile) + "/newfile";

            try
            {
                File.Copy(Config.UploadTestFile, newFileName, true);

                _ossClient.PutObject(_bucketName, key, newFileName, new ObjectMetadata());
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/octet-stream", oldMeta.ContentType);
                Assert.AreEqual(null, oldMeta.CacheControl);

                _ossClient.ModifyObjectMeta(_bucketName, key, new ObjectMetadata() { CacheControl = "public" });

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/octet-stream", newMeta.ContentType);
                Assert.AreEqual("public", newMeta.CacheControl);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
                File.Delete(newFileName);
            }
        }

        /// <summary>
        /// Validates if allowing empty value as the metadata value.
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithSetEmptyValue()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf",
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);
                Assert.AreEqual(meta.CacheControl, oldMeta.CacheControl);

                var newMeta = new ObjectMetadata()
                {
                    CacheControl = "",
                    ContentEncoding = null,
                    ContentType = "application/octet-stream",
                    ExpirationTime = oldMeta.ExpirationTime
                };

                _ossClient.ModifyObjectMeta(_bucketName, key, newMeta);

                var newMeta2 = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/octet-stream", newMeta2.ContentType);
                Assert.AreEqual(null, newMeta2.CacheControl);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        #endregion

        #region append object

        [Test]
        public void AppendObject()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                long position = 0;
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        Content = fs,
                        Position = position
                    };

                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                    position = result.NextAppendPosition;
                }

                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        Content = fs,
                        Position = position
                    };

                    var result = _ossClient.AppendObject(request);
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
        public void AppendObjectWithHeader()
        {
            const string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                long position = 0;
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position
                    };
                    request.ObjectMetadata.ContentType = contentType;

                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                    position = result.NextAppendPosition;
                }

                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position
                    };

                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength * 2, result.NextAppendPosition);
                    Assert.IsTrue(result.HashCrc64Ecma != 0);
                }

                var meta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(contentType, meta.ContentType);
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
        public void AppendObjectWithAppendPositionMoreThanCurrentPosition()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = 100
                    };

                    _ossClient.AppendObject(request);
                    Assert.IsFalse(true);
                }
            }
            catch (OssException ex)
            {
                Assert.AreEqual("PositionNotEqualToLength", ex.ErrorCode);
            }
            catch (WebException ex)
            {
                HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                if (errorResponse.StatusCode != HttpStatusCode.Conflict)
                {
                    Assert.IsTrue(false);
                }
            }
            finally
            {
                System.Threading.Thread.Sleep(5000);
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void AppendObjectWithAppendPositionLessThanCurrentPosition()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = 0
                    };

                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);

                    request.Position = result.NextAppendPosition - 1;
                    _ossClient.AppendObject(request);

                    Assert.IsFalse(true);
                }
            }
            catch (OssException ex)
            {
                Assert.AreEqual("PositionNotEqualToLength", ex.ErrorCode);
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
        public void AppendObjectWithHasNormalObjectExist()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile);

                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = 0
                    };

                    _ossClient.AppendObject(request);
                    Assert.IsFalse(true);
                }
            }
            catch (OssException ex)
            {
                Assert.AreEqual("ObjectNotAppendable", ex.ErrorCode);
            }
            catch (WebException ex)
            {
                HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                if (errorResponse.StatusCode != HttpStatusCode.Conflict)
                {
                    Assert.IsTrue(false);
                }
            }
            finally
            {
                System.Threading.Thread.Sleep(5 * 1000);
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void AppendObjectWithAppendEmptyObject()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                long position = 0;
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position
                    };

                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                    position = result.NextAppendPosition;
                }

                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                     var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position
                    };
                    request.Content = new MemoryStream(Encoding.UTF8.GetBytes(""));
                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                }
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
        public void AsyncAppendObject()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = 0
                    };

                    _ossClient.BeginAppendObject(request, AppendObjectCallback, new string('a', 5));
                    _event.WaitOne();
                }
            }
            finally
            {
                System.Threading.Thread.Sleep(5 * 1000);
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadObjectWithHasAppendObjectExist()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = 0
                    };

                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                }

                _ossClient.PutObject(_bucketName, key, Config.UploadTestFile);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        #endregion

        #region resumable put object

        [Test]
        public void ResumableUploadObjectTest() 
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null,
                                                           Config.DownloadFolder);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
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
        public void ResumableUploadObjectCheckpointTest()
        {
            var key = "test/短板.mp4";
            // this test case requires to run under admin 
            try
            {
                // checkpoint is null
                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);

                // checkpoint is xx/
                result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(),
                                                        Config.DownloadFolder + Path.DirectorySeparatorChar);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);

                // checkpoint is empty
                result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null, "");
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);

                // checkpoint is current directory
                result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null, ".");
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);

                // checkpoint is previous directory
                result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null, "..");
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        public class FakeClient : OssClient
        {
            public int beginFailedIndex = 0;
            public int endFailedIndex = 0;
            public int currentIndex = 0;

            public FakeClient(string endpoint, string accessKeyId, string accessKeySecret)
            : base(endpoint, accessKeyId, accessKeySecret) { }

            /// <inheritdoc/>        
            override protected void ThrowIfNullRequest<TRequestType>(TRequestType request)
            {
                if (currentIndex >= beginFailedIndex && currentIndex <= endFailedIndex)
                {
                    currentIndex++;
                    throw new ArgumentNullException("uploadPartRequest");
                }

                currentIndex++;
                base.ThrowIfNullRequest(request);
            }
        };

        [Test]
        public void ResumableUploadObjectWithRetry() 
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var client = new FakeClient(Config.Endpoint, Config.AccessKeyId, Config.AccessKeySecret);
                client.beginFailedIndex = 2;
                client.endFailedIndex = 3;

                var result = client.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null,
                                                        Config.DownloadFolder);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
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
        public void ResumableUploadObjectWithFailedTimeMoreThanRetryTime() 
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var client = new FakeClient(Config.Endpoint, Config.AccessKeyId, Config.AccessKeySecret);
                client.beginFailedIndex = 2;
                client.endFailedIndex = 100;
                client.currentIndex = 0;

                client.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null,
                                                        Config.DownloadFolder);
                Assert.IsFalse(true);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
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
        public void ResumableUploadObjectFirstFailedAndSecondSucceeded() 
        {
            var key = OssTestUtils.GetObjectKey(_className);

            var client = new FakeClient(Config.Endpoint, Config.AccessKeyId, Config.AccessKeySecret);
            try
            {
                client.beginFailedIndex = 2;
                client.endFailedIndex = 100;

                client.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null,
                                                       Config.DownloadFolder);
                Assert.IsTrue(false);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            try
            {
                client.beginFailedIndex = 0;
                client.endFailedIndex = 0;
                client.currentIndex = 1;

                var result = client.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null,
                                                       Config.DownloadFolder);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        #endregion

        #region private
        private static List<string> CreateMultiObjects(int objectsCount)
        {
            var sampleObjects = new List<string>();
            for (var i = 0; i < objectsCount; i++)
            {
                var objectKey = OssTestUtils.GetObjectKey(_className);
                OssTestUtils.UploadObject(_ossClient, _bucketName, objectKey,
                    Config.UploadTestFile, new ObjectMetadata());
                sampleObjects.Add(objectKey);
                System.Threading.Thread.Sleep(100);
            }
            return sampleObjects;
        }

        private static void AppendObjectCallback(IAsyncResult ar)
        {
            try
            {
                _ossClient.EndAppendObject(ar);
            }
            catch (Exception e)
            {
                Assert.IsFalse(true, e.ToString());
            }
            finally
            {
                _event.Set();
            }
        }
        #endregion
    };
}

