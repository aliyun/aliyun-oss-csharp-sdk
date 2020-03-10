using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
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
        private static string _archiveBucketName;
        private static AutoResetEvent _event;

        [OneTimeSetUp]
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
            _archiveBucketName = _bucketName + "archive";
            _ossClient.CreateBucket(_bucketName);
            _ossClient.CreateBucket(_archiveBucketName, StorageClass.Archive);
            //create sample object
            _objectKey = OssTestUtils.GetObjectKey(_className);
            try
            {
                var poResult = OssTestUtils.UploadObject(_ossClient, _bucketName, _objectKey,
                    Config.UploadTestFile, new ObjectMetadata());

                _objectETag = poResult.ETag;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
            _event = new AutoResetEvent(false);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
            OssTestUtils.CleanBucket(_ossClient, _archiveBucketName);
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
        public void UploadObjectBasicSettingsAsyncStandardUsageTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                //upload the object
                var ar = OssTestUtils.BeginUploadObject(_ossClient, _bucketName, key,
                                               Config.UploadTestFile,
                                               null,
                                               null);
                var result = OssTestUtils.EndUploadObject(_ossClient, ar);
                Assert.AreEqual(result.HttpStatusCode, HttpStatusCode.OK);
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
        public void UploadObjectBasicSettingsAsyncExceptionTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            Exception e = new Exception("inject exception");
            try
            {
                //upload the object
                AutoResetEvent done = new AutoResetEvent(false);
                var result = OssTestUtils.BeginUploadObject(_ossClient, _bucketName, key,
                                               Config.UploadTestFile,
                                               (ar) => {
                                                   done.Set();
                                                   throw e;
                                               },
                                               null);
                done.WaitOne(1000 * 100); // in real world, this is not needed. This is just to make sure the injected exception gets caught.
                OssTestUtils.EndUploadObject(_ossClient, result);
            }
            catch(Exception ex)
            {
                Assert.AreSame(ex, e);
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
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void DownloadAndUploadStreamWithImplicitChunkedEncodingTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var obj = _ossClient.GetObject(_bucketName, _objectKey);
                Assert.IsNotNull(obj.Content);
                PutObjectRequest putRequest = new PutObjectRequest(_bucketName, key, obj.Content, null); // user do not need to explictly use chunked enconding
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

            var saveAs = "abc测试123.zip";
            var contentDisposition = string.Format("attachment;filename*=utf-8''{0}", HttpUtils.EncodeUri(saveAs, "utf-8"));

            //config metadata
            var metadata = new ObjectMetadata
            {
                CacheControl = "no-cache",
                ContentDisposition = contentDisposition,//"abc.zip",
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
                Assert.IsTrue(uploadedObjectMetadata.ContentDisposition.Contains("abc"));
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
        public void UploadObjectWithInvalidArgument()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                Stream stream = null;
                var meta = new ObjectMetadata() { ContentMd5 = "test" };

                //upload the object
                _ossClient.PutObject(_bucketName, key, stream, meta);
                Assert.Fail("the argument is null, should not be here");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("content"));
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
        public void AsyncUploadObjectSpecifyFilePathDefaultMetadataTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var asyncResult = _ossClient.BeginPutObject(_bucketName, key, fs, null, null);
                    var waitHandle = asyncResult.AsyncWaitHandle;
                    waitHandle = asyncResult.AsyncWaitHandle;
                    waitHandle.WaitOne();
                    Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                }
            }
            catch
            {
                Assert.IsTrue(false);
            }


            key = OssTestUtils.GetObjectKey(_className);

            try
            {
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var asyncResult = _ossClient.BeginPutObject(_bucketName, key, fs, new ObjectMetadata(), null, null);
                    var waitHandle = asyncResult.AsyncWaitHandle;
                    waitHandle = asyncResult.AsyncWaitHandle;
                    waitHandle.WaitOne();
                    Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                }
            }
            catch
            {
                Assert.IsTrue(false);
            }


            key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var asyncResult = _ossClient.BeginPutObject(_bucketName, key, Config.UploadTestFile, new ObjectMetadata(), null, null);
                var waitHandle = asyncResult.AsyncWaitHandle;
                waitHandle = asyncResult.AsyncWaitHandle;
                waitHandle.WaitOne();
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
            }
            catch
            {
                Assert.IsTrue(false);
            }

            key = OssTestUtils.GetObjectKey(_className);

            try
            {
                ObjectMetadata meta = null;
                var asyncResult = _ossClient.BeginPutObject(_bucketName, key, Config.UploadTestFile, meta, null, null);
                var waitHandle = asyncResult.AsyncWaitHandle;
                waitHandle = asyncResult.AsyncWaitHandle;
                waitHandle.WaitOne();
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [Test]
        public void UploadObjectWithExceptionTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            //file path invalid
            try
            {
                var result = _ossClient.PutObject(_bucketName, key, "invalid-file-Path", null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //file path is folder
            try
            {
                var result = _ossClient.PutObject(_bucketName, key, Config.DownloadFolder, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //file path invalid
            try
            {
                var result = _ossClient.BeginPutObject(_bucketName, key, "invalid-file-Path", null, null, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //file path is folder
            try
            {
                var result = _ossClient.BeginPutObject(_bucketName, key, Config.DownloadFolder, null, null, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //file path invalid
            try
            {
                var result = _ossClient.PutObject(new Uri("http://endpoint/bucket/key"), "invalid-file-Path", null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //file path is folder
            try
            {
                var result = _ossClient.PutObject(new Uri("http://endpoint/bucket/key"), Config.DownloadFolder, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
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
        public void AsyncGetObjectBasicTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            try
            {
                var asyncResult = _ossClient.BeginGetObject(_bucketName, _objectKey, null, null);
                Thread.Sleep(1 * 1000);
                asyncResult.AsyncWaitHandle.WaitOne();
                var ossObject = _ossClient.EndGetObject(asyncResult);
                using (var stream = ossObject.Content)
                {
                    var downloadedFileETag = FileUtils.ComputeContentMd5(stream);
                    var expectedETag = _ossClient.GetObjectMetadata(_bucketName, _objectKey).ETag;
                    Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
                }
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [Test]
        public void AsyncGetObjectWithExceptionTest()
        {
            var key = _objectKey + "-not-exist";
            try
            {
                var asyncResult = _ossClient.BeginGetObject(_bucketName, key, null, null);
                Thread.Sleep(1 * 1000);
                asyncResult.AsyncWaitHandle.WaitOne();
                var ossObject = _ossClient.EndGetObject(asyncResult);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void GetObjectToStreamTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    var request = new GetObjectRequest(_bucketName, _objectKey);
                    var result = _ossClient.GetObject(request, stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    var downloadedFileETag = FileUtils.ComputeContentMd5(stream);
                    var expectedETag = result.ETag;
                    Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
                }
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
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
                //list objects by specifying bucket name
                var allObjects = _ossClient.ListObjects(_bucketName);
                var allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);

                //default value is 100
                Assert.AreEqual(100, allObjects.MaxKeys);

                //upload the object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadTestFile, new ObjectMetadata());

                allObjects = _ossClient.ListObjects(_bucketName);
                var allObjectsSumm2 = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                //there is already one sample object
                Assert.AreEqual(allObjectsSumm2.Count, allObjectsSumm.Count + 1);
                foreach(var objectSummary in allObjectsSumm2)
                {
                    Assert.IsTrue(objectSummary.ToString().Contains(_bucketName));
                }

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

                //upload the object 
                var key1 = folderName + "/" + OssTestUtils.GetObjectKey(_className);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key1,
                   Config.UploadTestFile, new ObjectMetadata());

                //list objects with marker
                loRequest.Prefix = folderName;
                loRequest.MaxKeys = 1;
                loRequest.Delimiter = "";
                allObjects = _ossClient.ListObjects(loRequest);
                Assert.AreEqual(folderName, allObjects.Prefix);
                Assert.AreEqual(true, allObjects.IsTruncated);
                loRequest.Marker = allObjects.NextMarker;
                allObjects = _ossClient.ListObjects(loRequest);
                Assert.AreEqual(folderName, allObjects.Prefix);
                Assert.AreEqual(false, allObjects.IsTruncated);

                //MaxKeys & EncodingType null 
                loRequest.Prefix = folderName;
                loRequest.MaxKeys = null;
                loRequest.Delimiter = "/";
                loRequest.EncodingType = null;
                allObjects = _ossClient.ListObjects(loRequest);
                Assert.AreEqual(100, allObjects.MaxKeys);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void AsyncListAllObjectsTest()
        {
            //test folder structure
            var folderName = OssTestUtils.GetObjectKey(_className);
            var key = folderName + "/" + OssTestUtils.GetObjectKey(_className);

            try
            {
                //list objects by specifying bucket name
                var asyncResult = _ossClient.BeginListObjects(_bucketName, null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                var allObjects = _ossClient.EndListObjects(asyncResult);
                var allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);

                //default value is 100
                Assert.AreEqual(100, allObjects.MaxKeys);

                //upload the object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadTestFile, new ObjectMetadata());

                asyncResult = _ossClient.BeginListObjects(_bucketName, null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                allObjects = _ossClient.EndListObjects(asyncResult);
                var allObjectsSumm2 = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);

                //there is already one sample object
                Assert.AreEqual(allObjectsSumm2.Count, allObjectsSumm.Count + 1);

                //list objects by specifying folder as prefix
                asyncResult = _ossClient.BeginListObjects(_bucketName, folderName, null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                allObjects = _ossClient.EndListObjects(asyncResult);
                Assert.AreEqual(folderName, allObjects.Prefix);
                allObjectsSumm = OssTestUtils.ToArray<OssObjectSummary>(allObjects.ObjectSummaries);
                Assert.AreEqual(1, allObjectsSumm.Count);

                var loRequest = new ListObjectsRequest(_bucketName);
                loRequest.Prefix = folderName;
                loRequest.MaxKeys = 10;
                loRequest.Delimiter = "/";
                asyncResult = _ossClient.BeginListObjects(loRequest, null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                allObjects = _ossClient.EndListObjects(asyncResult);
                Assert.AreEqual(folderName, allObjects.Prefix);
                Assert.AreEqual("/", allObjects.Delimiter);
                //Assert.Fail("List objects using full conditions");

                //upload the object 
                var key1 = folderName + "/" + OssTestUtils.GetObjectKey(_className);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key1,
                   Config.UploadTestFile, new ObjectMetadata());

                //list objects with marker
                loRequest.Prefix = folderName;
                loRequest.MaxKeys = 1;
                loRequest.Delimiter = "";
                asyncResult = _ossClient.BeginListObjects(loRequest, null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                allObjects = _ossClient.EndListObjects(asyncResult);
                Assert.AreEqual(folderName, allObjects.Prefix);
                Assert.AreEqual(true, allObjects.IsTruncated);
                loRequest.Marker = allObjects.NextMarker;
                allObjects = _ossClient.ListObjects(loRequest);
                Assert.AreEqual(folderName, allObjects.Prefix);
                Assert.AreEqual(false, allObjects.IsTruncated);

                //MaxKeys & EncodingType null 
                loRequest.Prefix = folderName;
                loRequest.MaxKeys = null;
                loRequest.Delimiter = "/";
                loRequest.EncodingType = null;
                asyncResult = _ossClient.BeginListObjects(loRequest, null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                allObjects = _ossClient.EndListObjects(asyncResult);
                Assert.AreEqual(100, allObjects.MaxKeys);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void AsyncListAllObjectsWithExceptionTest()
        {
            //test folder structure
            var folderName = OssTestUtils.GetObjectKey(_className);
            var key = folderName + "/" + OssTestUtils.GetObjectKey(_className);

            try
            {
                ListObjectsRequest listRequest = null;
                //list objects by specifying bucket name
                var asyncResult = _ossClient.BeginListObjects(listRequest, null, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
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

        [Test]
        public void DeleteObjectsRequestTest()
        {
            try
            {
                var request = new DeleteObjectsRequest("bucket", null, true);
                Assert.IsTrue(false, "should not here");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var list = new List<string>();
                var request = new DeleteObjectsRequest("bucket", list, true);
                Assert.IsTrue(false, "should not here");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var list = new List<string>();
                for (int i = 0; i < 1001; i++)
                {
                    list.Add("key" + i);
                }
                var request = new DeleteObjectsRequest("bucket", list, true);
                Assert.IsTrue(false, "should not here");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            var list1 = new List<string>();
            list1.Add("key");
            var request1 = new DeleteObjectsRequest("bucket", list1, true);
            request1.EncodingType = null;
            Assert.AreEqual(request1.EncodingType, HttpUtils.UrlEncodingType);
            request1.EncodingType = "type";
            Assert.AreEqual(request1.EncodingType, "type");
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

        [Test]
        public void UploadObjectCaseFileSurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className) + ".potx";
            var newFileName = Path.GetDirectoryName(Config.UploadTestFile) + "/newFile.Docx";

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

        [Test]
        public void UploadObjectFileSurfixGif()
        {
            var key = OssTestUtils.GetObjectKey(_className) + ".gif";
            var newFileName = Path.GetDirectoryName(Config.UploadTestFile) + "/newFile.Gif";

            try
            {
                File.Copy(Config.UploadTestFile, newFileName, true);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key, newFileName, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("image/gif",
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
            string bucketName = _bucketName;
            const string key = "one";
            try
            {
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

            //throw execption
            try
            {
                var client = new OssClient(Config.Endpoint, Config.AccessKeyId, Config.AccessKeySecret, "", new ClientConfiguration());
                bool isExist = _ossClient.DoesObjectExist(bucketName, key);
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(true, e.Message);
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
        public void AppendObjectWithCrc()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                long position = 0;
                ulong initCrc = 0;
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        Content = fs,
                        Position = position,
                        InitCrc = initCrc
                    };

                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                    position = result.NextAppendPosition;
                    initCrc = result.HashCrc64Ecma;
                }

                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        Content = fs,
                        Position = position,
                        InitCrc = initCrc
                    };

                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength * 2, result.NextAppendPosition);
                    Assert.IsTrue(result.HashCrc64Ecma != 0);
                    initCrc = result.HashCrc64Ecma;
                }

                var meta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("Appendable", meta.ObjectType);
                Assert.AreEqual(initCrc.ToString(), meta.Crc64);
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
        public void AppendObjectWithWrongCrc()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                long position = 0;
                ulong initCrc = 123; // wrong init CRC
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        Content = fs,
                        Position = position,
                        InitCrc = initCrc
                    };

                    _ossClient.AppendObject(request);
                    Assert.Fail();
                }
            }
            catch(ClientException e)
            {
                Assert.IsTrue(e.Message.Contains("Crc64"));
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
#if NETCOREAPP2_0
            catch (System.Net.Http.HttpRequestException ex2)
            {
                Assert.IsTrue(ex2.Message.Contains(HttpStatusCode.Conflict.ToString()));
            }
#endif
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
#if TEST_DOTNETCORE
            catch (System.Net.Http.HttpRequestException ex2)
            {
                Assert.IsTrue(ex2.Message.Contains(HttpStatusCode.Conflict.ToString()));
            }
#endif
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
        public void AppendObjectWithMd5()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                long position = 0;
                string eTag = string.Empty;
                using (var fs = File.Open(Config.UploadTestFile, FileMode.Open))
                {
                    var fileLength = fs.Length;
                    var request = new AppendObjectRequest(_bucketName, key)
                    {
                        Content = fs,
                        Position = position,
                        InitCrc = 0
                    };

                    var client = OssClientFactory.CreateOssClientEnableMD5(true);
                    var result = client.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                    position = result.NextAppendPosition;
                    eTag = result.ETag;
                }

                var meta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("Appendable", meta.ObjectType);
                Assert.AreEqual(eTag, meta.ETag);
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

        [Test]
        public void AppendObjectWithNullParamter()
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
                        Content = null,
                        Position = position
                    };

                    var result = _ossClient.AppendObject(request);
                    Assert.AreEqual(fileLength, result.NextAppendPosition);
                    position = result.NextAppendPosition;
                    Assert.IsFalse(true);
                }
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("request.Content"));
            }
        }

        #endregion

        #region Restore object tests
        [Test]
        public void RestoreObjectBasicTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                //upload the object
                OssTestUtils.UploadObject(_ossClient, _archiveBucketName, key,
                    Config.UploadTestFile, new ObjectMetadata());
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _archiveBucketName, key));

                RestoreObjectResult result = _ossClient.RestoreObject(_archiveBucketName, key);
                Assert.IsTrue(result.HttpStatusCode == HttpStatusCode.Accepted);

                try
                {
                    result = _ossClient.RestoreObject(_archiveBucketName, key);
                }
                catch(OssException e)
                {
                    Assert.AreEqual(e.ErrorCode, "RestoreAlreadyInProgress");
                }

                int wait = 60;
                while (wait > 0)
                {
                    var meta = _ossClient.GetObjectMetadata(_archiveBucketName, key);
                    string restoreStatus = meta.HttpMetadata["x-oss-restore"] as string;
                    if (restoreStatus!= null && restoreStatus.StartsWith("ongoing-request=\"false\"", StringComparison.InvariantCultureIgnoreCase))
                    {
                        break;
                    }

                    Thread.Sleep(1000 * 30);
                    wait--;
                }

                result = _ossClient.RestoreObject(_archiveBucketName, key);
                Assert.IsTrue(result.HttpStatusCode == HttpStatusCode.OK);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _archiveBucketName, key))
                {
                    _ossClient.DeleteObject(_archiveBucketName, key);
                }
            }
        }
        #endregion

        #region Symlink tests
        [Test]
        public void CreateAndGetSymlinkTest()
        {
            string symlink = _objectKey + "_link";
            _ossClient.CreateSymlink(_bucketName, symlink, _objectKey);
            OssObject obj = _ossClient.GetObject(_bucketName, symlink);
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.ContentLength > 0);
            Assert.AreEqual(obj.HttpStatusCode, HttpStatusCode.OK);
            obj.Dispose();

            OssSymlink ossSymlink = _ossClient.GetSymlink(_bucketName, symlink);
            Assert.AreEqual(ossSymlink.Target, _objectKey);
        }

        [Test]
        public void CreateSymlinkInvalidParameters()
        {
            try
            {
                _ossClient.CreateSymlink(null, _objectKey + "_link", _objectKey);
                Assert.Fail();
            }
            catch(ArgumentException)
            {}

            try
            {
                _ossClient.CreateSymlink(_bucketName, null, _objectKey);
                Assert.Fail();
            }
            catch (ArgumentException)
            { }

            try
            {
                _ossClient.CreateSymlink(_bucketName, _objectKey + "_link", null);
                Assert.Fail();
            }
            catch (ArgumentException)
            { }

            try
            {
                _ossClient.CreateSymlink(_bucketName, _objectKey, _objectKey);
                Assert.Fail();
            }
            catch (ArgumentException)
            { }
        }

        [Test]
        public void GetSymlinkInvalidParameters()
        {
            try
            {
                _ossClient.GetSymlink(null, _objectKey + "_link");
                Assert.Fail();
            }
            catch (ArgumentException)
            { }

            try
            {
                _ossClient.GetSymlink(_bucketName, null);
                Assert.Fail();
            }
            catch (ArgumentException)
            { }

            try
            {
                _ossClient.GetSymlink(_bucketName, _objectKey);
                Assert.Fail();
            }
            catch(OssException ex)
            {
                Assert.AreEqual(ex.ErrorCode, "NotSymlink");
            }
        }

        [Test]
        public void CreateAndGetSymlinkWithSpecialNameTest()
        {
            string symlink = _objectKey + " +\t#_link";
            _ossClient.CreateSymlink(_bucketName, symlink, _objectKey);
            OssObject obj = _ossClient.GetObject(_bucketName, symlink);
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.ContentLength > 0);
            Assert.AreEqual(obj.HttpStatusCode, HttpStatusCode.OK);

            OssSymlink ossSymlink = _ossClient.GetSymlink(_bucketName, symlink);
            Assert.AreEqual(ossSymlink.Target, _objectKey);
            obj.Dispose();
        }

        [Test]
        public void CreateAndGetSymlinkWithUserMetadataTest()
        {
            string symlink = _objectKey + " +\t#_link";
            ObjectMetadata metadata = new ObjectMetadata();
            metadata.HttpMetadata.Add("x-oss-header1", "value1");
            metadata.UserMetadata.Add("meta1", "value1");
            metadata.UserMetadata.Add("meta2", "value2");
            CreateSymlinkRequest req = new CreateSymlinkRequest(_bucketName, symlink, _objectKey, metadata);
            _ossClient.CreateSymlink(req);
            OssObject obj = _ossClient.GetObject(_bucketName, symlink);
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.ContentLength > 0);
            Assert.AreEqual(obj.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(obj.Metadata.UserMetadata.Count, metadata.UserMetadata.Count);
            Assert.AreEqual(obj.Metadata.UserMetadata["meta1"], metadata.UserMetadata["meta1"]);
            Assert.AreEqual(obj.Metadata.UserMetadata["meta2"], metadata.UserMetadata["meta2"]);
            Assert.AreEqual(obj.Metadata.ObjectType, "Symlink");
            obj.Dispose();

            OssSymlink ossSymlink = _ossClient.GetSymlink(_bucketName, symlink);
            Assert.AreEqual(ossSymlink.Target, _objectKey);
            Assert.AreEqual(ossSymlink.ObjectMetadata.UserMetadata["meta1"], metadata.UserMetadata["meta1"]);
            Assert.AreEqual(ossSymlink.ObjectMetadata.UserMetadata["meta2"], metadata.UserMetadata["meta2"]);
        }

        [Test]
        public void CreateSymlinkWithNullUserMetadataTest()
        {
            string symlink = _objectKey + " +\t#_link";
            CreateSymlinkRequest req = new CreateSymlinkRequest(_bucketName, symlink, _objectKey);
            _ossClient.CreateSymlink(req);
            OssObject obj = _ossClient.GetObject(_bucketName, symlink);
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.ContentLength > 0);
            Assert.AreEqual(obj.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(obj.Metadata.UserMetadata.Count, 0);
            obj.Dispose();
        }

        #endregion

        #region Post Object
        [Test]
        public void GeneratePostPolicyTest()
        {
            try
            {
                _ossClient.GeneratePostPolicy(DateTime.UtcNow, null);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);

            }

            var conds = new PolicyConditions();
            var policy = _ossClient.GeneratePostPolicy(DateTime.UtcNow, conds);
            Assert.AreNotEqual(policy.IndexOf("expiration"), -1);
        }


        #endregion

        #region object meta test
        [Test]
        public void ObjectMetadataTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            ObjectMetadata metadata = new ObjectMetadata();
            metadata.UserMetadata.Add("meta1", "value1");
            metadata.UserMetadata.Add("meta2", "value2");
            _ossClient.PutObject(_bucketName, key, new MemoryStream(Encoding.ASCII.GetBytes("hello")), metadata);

            var result = _ossClient.GetObjectMetadata(new GetObjectMetadataRequest(_bucketName, key));
            Assert.AreEqual(result.UserMetadata.ContainsKey("meta1"), true);
            Assert.AreEqual(result.UserMetadata.ContainsKey("meta2"), true);
            Assert.AreEqual(result.ContentLength, 5);


            result = _ossClient.GetSimplifiedObjectMetadata(new GetObjectMetadataRequest(_bucketName, key));
            Assert.AreEqual(result.UserMetadata.ContainsKey("meta1"), false);
            Assert.AreEqual(result.UserMetadata.ContainsKey("meta2"), false);
            Assert.AreEqual(result.ContentLength, 5);
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

