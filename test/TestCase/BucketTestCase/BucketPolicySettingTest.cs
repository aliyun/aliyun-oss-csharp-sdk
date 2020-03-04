using System;
using Aliyun.OSS.Common;
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
            var request = new SetBucketPolicyRequest(_bucketName, policy);

            Assert.AreEqual(policy, request.Policy);
            Assert.AreEqual(_bucketName, request.BucketName);

            _ossClient.SetBucketPolicy(request);

            OssTestUtils.WaitForCacheExpire();

            var result = _ossClient.GetBucketPolicy(_bucketName);
            Assert.AreEqual(result.Policy, policy);

            _ossClient.DeleteBucketPolicy(_bucketName);
        }

        [Test]
        public void SetBucketPolicyWithInvalidPolicyTexTest()
        {
            try
            {
                var request = new SetBucketPolicyRequest(_bucketName, "invalidpolicy");
                _ossClient.SetBucketPolicy(request);
                Assert.Fail("Set bucket policy should fail with invalid policy");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "InvalidPolicyDocument");
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void SetBucketPolicyWithInvalidArgumentTest()
        {
            try
            {
                _ossClient.SetBucketPolicy(null);
                Assert.Fail("Set bucket policy should fail.");
            }
            catch (ArgumentNullException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }
    }
}
