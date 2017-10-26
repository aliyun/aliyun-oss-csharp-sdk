using System;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public partial class BucketSettingsTest
    {
        [Test]
        public void SetBucketStorageCapacityTest()
        {
            var request = new SetBucketStorageCapacityRequest(_bucketName, 10240);
            _ossClient.SetBucketStorageCapacity(request);

            OssTestUtils.WaitForCacheExpire();

            var result = _ossClient.GetBucketStorageCapacity(_bucketName);
            Assert.AreEqual(result.StorageCapacity, 10240);
        }

        [Test]
        public void SetBucketStorageCapacityInvalidInputTest()
        {
            try
            {
                var request = new SetBucketStorageCapacityRequest(_bucketName, -2);
                _ossClient.SetBucketStorageCapacity(request);
                Assert.Fail("Set bucket storage capacity should fail with invalid storage capacity");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }
    }
}
