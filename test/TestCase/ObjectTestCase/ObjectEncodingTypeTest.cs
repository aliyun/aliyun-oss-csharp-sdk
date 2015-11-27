using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectEncodingTypeTest 
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _keyName;   

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
            //create sample object
            _keyName = OssTestUtils.GetObjectKey(_className);
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        #region stream upload

        [Test]
        public void DeleteObjectWithHiddenCharacters() 
        {
            char[] buffer = new char[2];
            buffer[0] = Convert.ToChar(0x1c);
            buffer[1] = Convert.ToChar(0x1a);

            var newKey = _keyName + (new string(buffer)) + ".1.cd";

            try
            {
                _ossClient.PutObject(_bucketName, newKey, Config.UploadSampleFile);

                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, newKey));

                _ossClient.DeleteObject(_bucketName, newKey);

                Assert.IsFalse(OssTestUtils.ObjectExists(_ossClient, _bucketName, newKey));
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, newKey);
            }
        }

        [Test]
        public void DeleteObjectsWithHiddenCharacters() 
        {
            char[] buffer = new char[2];
            buffer[0] = Convert.ToChar(0x1c);
            buffer[1] = Convert.ToChar(0x1a);

            var newKey1 = _keyName + (new string(buffer)) + ".1.cd";
            var newKey2 = _keyName + (new string(buffer)) + ".2.cd";
            var newKey3 = _keyName + ".3.cd";

            try
            {
                _ossClient.PutObject(_bucketName, newKey1, Config.UploadSampleFile);
                _ossClient.PutObject(_bucketName, newKey2, Config.UploadSampleFile);
                _ossClient.PutObject(_bucketName, newKey3, Config.UploadSampleFile);

                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, newKey1));
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, newKey2));
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, newKey3));

                var keys = new List<string>();
                keys.Add(newKey1);
                keys.Add(newKey2);
                keys.Add(newKey3);

                var request = new DeleteObjectsRequest(_bucketName, keys);
                _ossClient.DeleteObjects(request);

                Assert.IsFalse(OssTestUtils.ObjectExists(_ossClient, _bucketName, newKey1));
                Assert.IsFalse(OssTestUtils.ObjectExists(_ossClient, _bucketName, newKey2));
                Assert.IsFalse(OssTestUtils.ObjectExists(_ossClient, _bucketName, newKey3));
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, newKey1);
                _ossClient.DeleteObject(_bucketName, newKey2);
                _ossClient.DeleteObject(_bucketName, newKey3);
            }
        }

        //[Test]
        public void ListMultipartUploadsWithHiddenCharacters() 
        {
            char[] buffer = new char[2];
            buffer[0] = Convert.ToChar(0x1c);
            buffer[1] = Convert.ToChar(0x1a);

            var newKey = _keyName + (new string(buffer)) + ".1.cd";

            try
            {
                var initRequest = new InitiateMultipartUploadRequest(_bucketName, newKey);
                var initResult = _ossClient.InitiateMultipartUpload(initRequest);
                Assert.AreEqual(newKey, initRequest.Key);

                // 设置每块为 5M
                const int partSize = 5 * 1024 * 1024;

                var fileInfo = new FileInfo(Config.MultiUploadSampleFile);

                // 计算分块数目
                var partCount = OssTestUtils.CalculatePartCount(fileInfo.Length, partSize);

                // 新建一个List保存每个分块上传后的ETag和PartNumber
                var partETags = new List<PartETag>();

                //upload the file
                using (var fs = new FileStream(fileInfo.FullName, FileMode.Open))
                {
                    for (var i = 0; i < partCount; i++)
                    {
                        // 跳到每个分块的开头
                        long skipBytes = partSize * i;
                        fs.Position = skipBytes;

                        // 计算每个分块的大小
                        long size = partSize < fileInfo.Length - skipBytes ? partSize : fileInfo.Length - skipBytes;

                        // 创建UploadPartRequest，上传分块
                        var uploadPartRequest = new UploadPartRequest(_bucketName, newKey, initResult.UploadId);
                        uploadPartRequest.InputStream = fs;
                        uploadPartRequest.PartSize = size;
                        uploadPartRequest.PartNumber = (i + 1);
                        var uploadPartResult = _ossClient.UploadPart(uploadPartRequest);

                        // 将返回的PartETag保存到List中。
                        partETags.Add(uploadPartResult.PartETag);

                        var multipartListing = _ossClient.ListMultipartUploads(new ListMultipartUploadsRequest(_bucketName));
                        foreach (var upload in multipartListing.MultipartUploads)
                        {
                            Console.WriteLine(upload.ToString());
                            Assert.AreEqual(newKey, upload.Key);
                        }
                    }
                }

                var completeRequest = new CompleteMultipartUploadRequest(_bucketName, newKey, initResult.UploadId);
                foreach (var partETag in partETags)
                {
                    completeRequest.PartETags.Add(partETag);
                }
                _ossClient.CompleteMultipartUpload(completeRequest);
            }
            finally
            {
                var multipartListing = _ossClient.ListMultipartUploads(new ListMultipartUploadsRequest(_bucketName));
                foreach (var upload in multipartListing.MultipartUploads)
                {
                    var abortRequest = new AbortMultipartUploadRequest(_bucketName, newKey, upload.UploadId);
                    _ossClient.AbortMultipartUpload(abortRequest);
                }
                _ossClient.DeleteObject(_bucketName, newKey);
            }
        }

        #endregion

        private static List<string> CreateMultiObjects(int objectsCount)
        {
            var sampleObjects = new List<string>();
            for (var i = 0; i < objectsCount; i++)
            {
                var objectKey = OssTestUtils.GetObjectKey(_className);
                OssTestUtils.UploadObject(_ossClient, _bucketName, objectKey,
                    Config.UploadSampleFile, new ObjectMetadata());
                sampleObjects.Add(objectKey);
                System.Threading.Thread.Sleep(100);
            }
            return sampleObjects;
        }

    };
}
