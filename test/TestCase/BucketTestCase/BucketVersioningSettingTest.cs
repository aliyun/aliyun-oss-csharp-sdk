using System;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public partial class BucketSettingsTest
    {
        [Test]
        public void BucketVersioningSettingTest()
        {
            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);

            //create a new bucket
            _ossClient.CreateBucket(bucketName);

            //default
            var result = _ossClient.GetBucketVersioning(bucketName);
            Assert.AreEqual(result.Status, VersioningStatus.Off);

            var info = _ossClient.GetBucketInfo(bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Off);

            //set to Enabled
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(bucketName, VersioningStatus.Enabled));
            result = _ossClient.GetBucketVersioning(bucketName);

            Assert.AreEqual(result.Status, VersioningStatus.Enabled);

            info = _ossClient.GetBucketInfo(bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            //set to Suspended
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(bucketName, VersioningStatus.Suspended));
            result = _ossClient.GetBucketVersioning(bucketName);

            Assert.AreEqual(result.Status, VersioningStatus.Suspended);

            info = _ossClient.GetBucketInfo(bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Suspended);

            _ossClient.DeleteBucket(bucketName);

        }

        [Test]
        public void SetBucketVersioningWithExceptionTest()
        {
            try
            {
                _ossClient.SetBucketVersioning(null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }
    }
}

