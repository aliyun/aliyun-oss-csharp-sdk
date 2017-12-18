using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Common;
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
    public class ObjectResumableOperationTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _objectKey;     
        private static ClientConfiguration _config;

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            _config = new ClientConfiguration();

            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient(_config);
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
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        #region resumable upload object

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
                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null, Config.DownloadFolder);
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

            int failedCount = 0;
            try
            {
                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, null,
                                                              Config.DownloadFolder, 1024 * 200, 
                                                              (sender, e) =>
                                                            {
                                                                if (failedCount < 2) {
                                                                    failedCount++;
                                                                    throw new Exception("injection failure"); 
                                                                }
                                                            });
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
        public void ResumableUploadObjectWithRetryUsingSingleReadThread()
        {
            bool restoreVal = _config.UseSingleThreadReadInResumableUpload;
            _config.UseSingleThreadReadInResumableUpload = true;

            try
            {
                ResumableUploadObjectWithRetry();
            }
            finally
            {
                _config.UseSingleThreadReadInResumableUpload = restoreVal;
            }
        }

        [Test]
        public void ResumableUploadObjectWithRetryUsingSingleThread()
        {
            int restoreVal = _config.MaxResumableUploadThreads;
            _config.MaxResumableUploadThreads = 1;

            try
            {
                ResumableUploadObjectWithRetry();
            }
            finally
            {
                _config.MaxResumableUploadThreads = restoreVal;
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

                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(), Config.DownloadFolder, fileSize - 1);
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
        public void ResumableUploadObjectTestWithObjectMoreThanPartSizeUsingSingleReadThread()
        {
            bool restoreVal = _config.UseSingleThreadReadInResumableUpload;
            _config.UseSingleThreadReadInResumableUpload = true;
            try
            {
                ResumableUploadObjectTestWithObjectMoreThanPartSize();
            }
            finally
            {
                _config.UseSingleThreadReadInResumableUpload = restoreVal;
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

        [Test]
        public void ResumableUploadObjectTestWithBigObject()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadTestFile);
                var fileSize = fileInfo.Length;
                _config.MaxPartCachingSize = 1024 * 20; // set the max part caching size to 20k so that the ResumableUploadObject will use single thread.
                _config.UseSingleThreadReadInResumableUpload = true;
                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(), Config.DownloadFolder, fileSize / 3);
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
        public void ResumableUploadObjectTestWithManyParts()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadTestFile);
                var fileSize = fileInfo.Length;
                var result = _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(), Config.DownloadFolder, fileSize / 64);
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
        public void ResumableUploadObjectTestWithManyPartsUsingSingleReadThread()
        {
            bool restoreVal = _config.UseSingleThreadReadInResumableUpload;
            _config.UseSingleThreadReadInResumableUpload = true;
            try
            {
                ResumableUploadObjectTestWithManyParts();
            }
            finally
            {
                _config.UseSingleThreadReadInResumableUpload = restoreVal;
            }
        }

        [Test]
        public void ResumableUploadObjectTestWithSingleThread()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadTestFile);
                var fileSize = fileInfo.Length;
                var config = new ClientConfiguration();
                config.MaxResumableUploadThreads = 1;
                var client = OssClientFactory.CreateOssClient(config);
                var result = client.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(), Config.DownloadFolder, fileSize / 3);
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

        #region Resumable Download objects;
        [Test]
        public void ResumableDownloadSmallFileBasicTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, _objectKey, targetFile);
                var metadata =_ossClient.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
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
        public void ResumableDownloadSmallFileProgressUpdateTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            bool progressUpdateCalled = false;
            int percentDone = 0;
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, _objectKey, targetFile);
                long totalBytesDownloaded = 0;
                request.StreamTransferProgress += (object sender, StreamTransferProgressArgs e) => {
                    totalBytesDownloaded += e.TransferredBytes;
                    Console.WriteLine("TotalBytes:" + e.TotalBytes);
                    Console.WriteLine("TransferredBytes:" + e.TransferredBytes);
                    Console.WriteLine("PercentageDone:" + e.PercentDone);
                    progressUpdateCalled = true;
                    percentDone = e.PercentDone;
                };

                var metadata = _ossClient.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
                Assert.IsTrue(progressUpdateCalled);
                Assert.IsTrue(percentDone == 100);
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
        public void ResumableDownloadSmallFileWithMd5CheckTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            var config = new ClientConfiguration();
            config.EnalbeMD5Check = true;
            var client = OssClientFactory.CreateOssClient(config);
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, _objectKey, targetFile);
                var metadata = client.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
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
        public void ResumableDownloadBigFileBasicTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, key, Config.MultiUploadTestFile);

            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, key, targetFile);
                var metadata = _ossClient.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
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
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void ResumableDownloadBigFileSingleThreadTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, key, Config.MultiUploadTestFile);
            var config = new ClientConfiguration();
            config.MaxResumableDownloadThreads = 1;
            var client = OssClientFactory.CreateOssClient(config);
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, key, targetFile);
                var metadata = client.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
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
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void ResumableDownloadBigFileSingleThreadWithMd5CheckTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, key, Config.MultiUploadTestFile);
            var config = new ClientConfiguration();
            config.MaxResumableDownloadThreads = 1;
            config.EnalbeMD5Check = true;
            var client = OssClientFactory.CreateOssClient(config);
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, key, targetFile);
                var metadata = client.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
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
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void ResumableDownloadBigFileSingleThreadWithProgressUpdateTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, key, Config.MultiUploadTestFile);
            bool progressUpdateCalled = false;
            int percentDone = 0;
            long totalBytesDownloaded = 0;
            var config = new ClientConfiguration();
            config.MaxResumableDownloadThreads = 1;
            config.EnalbeMD5Check = true;
            var client = OssClientFactory.CreateOssClient(config);
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, key, targetFile);
                request.StreamTransferProgress += (object sender, StreamTransferProgressArgs e) => {
                    totalBytesDownloaded += e.TransferredBytes;
                    Console.WriteLine("TotalBytes:" + e.TotalBytes);
                    Console.WriteLine("TransferredBytes:" + e.TransferredBytes);
                    Console.WriteLine("PercentageDone:" + e.PercentDone);
                    progressUpdateCalled = true;
                    percentDone = e.PercentDone;
                };

                var metadata = client.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
                Assert.AreEqual(progressUpdateCalled, true);
                Assert.AreEqual(percentDone, 100);
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
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void ResumableDownloadBigFileWithProgressUpdateTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, key, Config.MultiUploadTestFile);
            bool progressUpdateCalled = false;
            int percentDone = 0;
            long totalBytesDownloaded = 0;
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, key, targetFile);
                request.StreamTransferProgress += (object sender, StreamTransferProgressArgs e) => {
                    totalBytesDownloaded += e.TransferredBytes;
                    Console.WriteLine("TotalBytes:" + e.TotalBytes);
                    Console.WriteLine("TransferredBytes:" + e.TransferredBytes);
                    Console.WriteLine("PercentageDone:" + e.PercentDone);
                    progressUpdateCalled = true;
                    percentDone = e.PercentDone;
                };

                var metadata = _ossClient.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
                Assert.AreEqual(progressUpdateCalled, true);
                Assert.AreEqual(percentDone, 100);
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
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void ResumableDownloadBigFileWithMd5CheckTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, key, Config.MultiUploadTestFile);

            var config = new ClientConfiguration();
            config.EnalbeMD5Check = true;
            var client = OssClientFactory.CreateOssClient(config);
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, key, targetFile);
                var metadata = client.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
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
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void ResumableDownloadBigFileWithMultipartUploadTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(), Config.DownloadFolder, null);

            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, key, targetFile);
                var metadata = _ossClient.ResumableDownloadObject(request);

                Assert.AreEqual(metadata.ContentLength, new FileInfo(targetFile).Length);
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
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void ResumableDownloadBigFileWithMultipartUploadWithMd5CheckTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.ResumableUploadObject(_bucketName, key, Config.MultiUploadTestFile, new ObjectMetadata(), Config.DownloadFolder, null);

            try
            {
                var config = new ClientConfiguration();
                config.EnalbeMD5Check = true;
                var client = OssClientFactory.CreateOssClient(config);
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, key, targetFile);
                var metadata = client.ResumableDownloadObject(request);

                Assert.AreEqual(metadata.ContentLength, new FileInfo(targetFile).Length);
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
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void ResumableDownloadBigFileBreakAndResumeTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, key, Config.MultiUploadTestFile);
            bool progressUpdateCalled = false;
            bool faultInjected = false;
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, key, targetFile);
                request.CheckpointFile = Config.DownloadFolder;
                request.PartSize = 1024 * 1024;
                request.StreamTransferProgress += (object sender, StreamTransferProgressArgs e) => {
                    if (!progressUpdateCalled)
                    {
                        progressUpdateCalled = true;
                    }
                    else
                    {
                        if (!faultInjected)
                        {
                            faultInjected = true;
                            throw new TimeoutException("Inject failure");
                        }
                    }
                };

                var metadata = _ossClient.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
                Assert.AreEqual(progressUpdateCalled, true);
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
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void ResumableDownloadBigFileSingleThreadBreakAndResumeTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);

            var key = OssTestUtils.GetObjectKey(_className);
            _ossClient.PutObject(_bucketName, key, Config.MultiUploadTestFile);
            bool progressUpdateCalled = false;
            bool faultInjected = false;
            try
            {
                DownloadObjectRequest request = new DownloadObjectRequest(_bucketName, key, targetFile);
                request.PartSize = 100 * 1024;
                request.CheckpointFile = Config.DownloadFolder;
                request.StreamTransferProgress += (object sender, StreamTransferProgressArgs e) => {
                    if (!progressUpdateCalled)
                    {
                        progressUpdateCalled = true;
                    }
                    else
                    {
                        if (!faultInjected)
                        {
                            faultInjected = true;
                            throw new TimeoutException("Inject failure");
                        }
                    }
                };

                var config = new ClientConfiguration();
                config.MaxResumableDownloadThreads = 1;
                var client = OssClientFactory.CreateOssClient(config);

                var metadata = client.ResumableDownloadObject(request);
                var expectedETag = metadata.ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
                Assert.AreEqual(progressUpdateCalled, true);
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
                _ossClient.DeleteObject(_bucketName, key);
            }
        }
        #endregion
    }
}
