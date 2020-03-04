using Aliyun.OSS.Test.Util;
using Aliyun.OSS.Domain;
using NUnit.Framework;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectTaggingTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _keyName;

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
            _keyName = OssTestUtils.GetObjectKey(_className);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void SetObjectTaggingTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            _ossClient.PutObject(_bucketName, key, new MemoryStream(Encoding.ASCII.GetBytes("hello world")));

            var setRequest = new SetObjectTaggingRequest(_bucketName, key);

            var tag1 = new Tag
            {
                Key = "project",
                Value = "projectone"
            };

            var tag2 = new Tag
            {
                Key = "user",
                Value = "jsmith"
            };

            setRequest.AddTag(tag1);
            setRequest.AddTag(tag2);

            _ossClient.SetObjectTagging(setRequest);

            var result = _ossClient.GetObjectTagging(_bucketName, key);
            Assert.AreEqual(result.Tags.Count, setRequest.Tags.Count);
            Assert.AreEqual(result.Tags[0].Key, setRequest.Tags[0].Key);
            Assert.AreEqual(result.Tags[0].Value, setRequest.Tags[0].Value);
            Assert.AreEqual(result.Tags[1].Key, setRequest.Tags[1].Key);
            Assert.AreEqual(result.Tags[1].Value, setRequest.Tags[1].Value);


            var tags = new List<Tag>();
            tags.Add(tag2);
            setRequest.Tags = tags;
            _ossClient.SetObjectTagging(setRequest);

            result = _ossClient.GetObjectTagging(_bucketName, key);
            Assert.AreEqual(result.Tags.Count, 1);
            Assert.AreEqual(result.Tags[0].Key, tag2.Key);
            Assert.AreEqual(result.Tags[0].Value, tag2.Value);


            tags = new List<Tag>();
            tags.Add(tag1);
            setRequest = new SetObjectTaggingRequest(_bucketName, key, tags);
            _ossClient.SetObjectTagging(setRequest);

            result = _ossClient.GetObjectTagging(_bucketName, key);
            Assert.AreEqual(result.Tags.Count, 1);
            Assert.AreEqual(result.Tags[0].Key, tag1.Key);
            Assert.AreEqual(result.Tags[0].Value, tag1.Value);


            _ossClient.DeleteObjectTagging(_bucketName, key);

            result = _ossClient.GetObjectTagging(_bucketName, key);
            Assert.AreEqual(result.Tags.Count, 0);
        }

        [Test]
        public void SetObjectTaggingWithExceptionTest()
        {
            try
            {
                var request = new SetObjectTaggingRequest(_bucketName, _keyName);
                request.AddTag(null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var request = new SetObjectTaggingRequest(_bucketName, _keyName, null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var request = new SetObjectTaggingRequest(_bucketName, _keyName);
                request.Tags = null;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void SetObjectTaggingFromHeaderTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            var tag1 = new Tag
            {
                Key = "key1",
                Value = "value1"
            };

            var tag2 = new Tag
            {
                Key = "key2",
                Value = "value2"
            };

            var meta = new ObjectMetadata();
            meta.AddHeader("x-oss-tagging", "key1=value1&key2=value2");

            var putRequest = new PutObjectRequest(_bucketName, key, new MemoryStream(Encoding.ASCII.GetBytes("hello world")));
            putRequest.Metadata = meta;
            _ossClient.PutObject(putRequest);

            var result = _ossClient.GetObjectTagging(_bucketName, key);
            Assert.AreEqual(result.Tags.Count, 2);
            Assert.AreEqual(result.Tags[0].Key, tag1.Key);
            Assert.AreEqual(result.Tags[0].Value, tag1.Value);
            Assert.AreEqual(result.Tags[1].Key, tag2.Key);
            Assert.AreEqual(result.Tags[1].Value, tag2.Value);
        }
    }
}

