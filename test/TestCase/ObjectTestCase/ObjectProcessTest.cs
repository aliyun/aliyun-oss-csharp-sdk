using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectProcessTest 
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _keyName;
        private static string _localImageFile;
        private static string _processedKey;
        private static string _imageInfo;
        private static string _process;

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
            _keyName += ".jpg";

            _process = "image/resize,m_fixed,w_100,h_100";
            _localImageFile = Config.ImageTestFile;
            _processedKey = "process/image" + _keyName;
            _imageInfo = "{\n    \"FileSize\": {\"value\": \"3267\"},\n    \"Format\": {\"value\": \"jpg\"},\n    \"ImageHeight\": {\"value\": \"100\"},\n    \"ImageWidth\": {\"value\": \"100\"}}";
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void ImageProcessTest() 
        {
            try
            {
                // put example image
                _ossClient.PutObject(_bucketName, _keyName, _localImageFile);

                // get processed image
                GetObjectRequest request = new GetObjectRequest(_bucketName, _keyName, _process);
                OssObject ossObject = _ossClient.GetObject(request);

                // put processed image
                Stream seekableStream = ConvertStreamToSeekable(ossObject.Content);
                _ossClient.PutObject(_bucketName, _processedKey, seekableStream);

                // get info of processed image
                var imgInfo = GetOssImageObjectInfo(_bucketName, _processedKey);

                // check processed result
                Assert.AreEqual(imgInfo, _imageInfo);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, _keyName);
                _ossClient.DeleteObject(_bucketName, _processedKey);
            }
        }

        [Test]
        public void GenerateUriWithProcessTest()
        {
            try
            {
                // put example image
                _ossClient.PutObject(_bucketName, _keyName, _localImageFile);

                // generate uri
                var req = new GeneratePresignedUriRequest(_bucketName, _keyName, SignHttpMethod.Get)
                {
                    Expiration = DateTime.Now.AddHours(1),
                    Process = _process
                };
                var uri = _ossClient.GeneratePresignedUri(req);

                // get processed image
                OssObject ossObject = _ossClient.GetObject(uri);

                // put processed image
                Stream seekableStream = ConvertStreamToSeekable(ossObject.Content);
                _ossClient.PutObject(_bucketName, _processedKey, seekableStream);

                // get info of processed image
                var imgInfo = GetOssImageObjectInfo(_bucketName, _processedKey);

                // check processed result
                Assert.AreEqual(imgInfo, _imageInfo);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, _keyName);
                _ossClient.DeleteObject(_bucketName, _processedKey);
            }
        }

        #region private
        private static Stream ConvertStreamToSeekable(Stream stream)
        {
            var memStream = new MemoryStream();
            IoUtils.WriteTo(stream, memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }

        private static string GetOssImageObjectInfo(string bucket, string key)
        {
            GetObjectRequest request = new GetObjectRequest(_bucketName, _processedKey, "image/info");
            OssObject ossObject = _ossClient.GetObject(request);

            StringBuilder builder = new StringBuilder();
            using (var requestStream = ossObject.Content)
            {
                byte[] buf = new byte[1024];
                var len = 0;
                while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                {
                    byte[] subBuf = new byte[len];
                    Buffer.BlockCopy(buf, 0, subBuf, 0, len);
                    builder.Append(Encoding.Default.GetString(subBuf));
                }
            }

            return builder.ToString();
        }
        #endregion
    }
}
