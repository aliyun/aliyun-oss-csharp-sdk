using System;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public partial class BucketSettingsTest
    {
        [Test]
        public void SetBucketPolicyTest()
        {
            string policy = "{\"Version\":\"1\",\"Statement\":[{\"Action\":[\"oss:PutObject\",\"oss:GetObject\"],\"Resource\": \"acs:oss:*:*:*\",\"Effect\": \"Deny\"}]}\n";
            var createBucketRequest = new SetBucketPolicyRequest(_bucketName, policy);

            _ossClient.SetBucketPolicy(createBucketRequest);

            OssTestUtils.WaitForCacheExpire();

            var result = _ossClient.GetBucketPolicy(_bucketName);
            Assert.AreEqual(result.Policy, policy);

            _ossClient.DeleteBucketPolicy(_bucketName);
        }

        [Test]
        public void SetBucketPolicyInvalidInputTest()
        {
            try
            {
                var request = new SetBucketPolicyRequest(_bucketName, "invalidpolicy");
                _ossClient.SetBucketPolicy(request);
                Assert.Fail("Set bucket policy should fail with invalid policy");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }
    }
}
