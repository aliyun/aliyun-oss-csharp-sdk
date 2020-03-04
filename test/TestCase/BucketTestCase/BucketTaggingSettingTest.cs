using System;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;
using Aliyun.OSS.Domain;
using Aliyun.OSS.Common;
using System.Collections.Generic;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public partial class BucketSettingsTest
    {
        [Test]
        public void BucketTaggingBasicTest()
        {
            var setRequest = new SetBucketTaggingRequest(_bucketName);

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

            _ossClient.SetBucketTagging(setRequest);

            var result = _ossClient.GetBucketTagging(_bucketName);

            Assert.AreEqual(result.Tags.Count, setRequest.Tags.Count);
            Assert.AreEqual(result.Tags[0].Key, setRequest.Tags[0].Key);
            Assert.AreEqual(result.Tags[0].Value, setRequest.Tags[0].Value);
            Assert.AreEqual(result.Tags[1].Key, setRequest.Tags[1].Key);
            Assert.AreEqual(result.Tags[1].Value, setRequest.Tags[1].Value);

            _ossClient.DeleteBucketTagging(_bucketName);
            result = _ossClient.GetBucketTagging(_bucketName);
            Assert.AreEqual(result.Tags.Count, 0);
        }

        [Test]
        public void ListBucketByTagTest()
        {
            var setRequest = new SetBucketTaggingRequest(_bucketName);

            var tag1 = new Tag
            {
                Key = "tag1",
                Value = "value1"
            };

            var tag2 = new Tag
            {
                Key = "tag2",
                Value = "value2"
            };

            var tags = new List<Tag>();
            tags.Add(tag1);
            tags.Add(tag2);

            setRequest.Tags = tags;

            _ossClient.SetBucketTagging(setRequest);

            var getResult = _ossClient.GetBucketTagging(_bucketName);

            Assert.AreEqual(getResult.Tags.Count, setRequest.Tags.Count);
            Assert.AreEqual(getResult.Tags[0].Key, setRequest.Tags[0].Key);
            Assert.AreEqual(getResult.Tags[0].Value, setRequest.Tags[0].Value);
            Assert.AreEqual(getResult.Tags[1].Key, setRequest.Tags[1].Key);
            Assert.AreEqual(getResult.Tags[1].Value, setRequest.Tags[1].Value);

            //match
            var listRequest = new ListBucketsRequest();
            listRequest.Tag = tag2;

            var result = _ossClient.ListBuckets(listRequest);

            var i = 0;
            foreach (var bucket in result.Buckets)
            {
                i++;
                Assert.AreEqual(_bucketName, bucket.Name);
            }
            Assert.AreEqual(i, 1);

            //match key not match value
            var tag3 = new Tag
            {
                Key = "tag3",
                Value = "jsmith"
            };
            listRequest = new ListBucketsRequest();
            listRequest.Tag = tag3;

            result = _ossClient.ListBuckets(listRequest);

            i = 0;
            foreach (var bucket in result.Buckets)
            {
                i++;
                Assert.AreEqual(_bucketName, bucket.Name);
            }
            Assert.AreEqual(i, 0);

            //only has key
            var tag4 = new Tag
            {
                Key = "tag2"
            };
            listRequest = new ListBucketsRequest();
            listRequest.Tag = tag4;

            result = _ossClient.ListBuckets(listRequest);

            i = 0;
            foreach (var bucket in result.Buckets)
            {
                i++;
                Assert.AreEqual(_bucketName, bucket.Name);
            }
            Assert.AreEqual(i, 1);
        }

        [Test]
        public void ListBucketByTagNGTest()
        {
            //only has value
            var tag = new Tag
            {
                Value = "tag3"
            };
            var listRequest = new ListBucketsRequest();
            listRequest.Tag = tag;

            try
            {
                var result = _ossClient.ListBuckets(listRequest);
                Assert.Fail("should not here");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "InvalidArgument");
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void SetBucketTaggingWithExceptionTest()
        {
            try
            {
                var request = new SetBucketTaggingRequest(_bucketName);
                request.AddTag(null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var request = new SetBucketTaggingRequest("");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var request = new SetBucketTaggingRequest(_bucketName, null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var request = new SetBucketTaggingRequest(null, null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var request = new SetBucketTaggingRequest(_bucketName);
                request.Tags = null;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }
    }
}
