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
        public void GetBucketNotSetCorsTest()
        {
            //ensure no Cors is set
            _ossClient.DeleteBucketCors(_bucketName);
            try
            {
                _ossClient.GetBucketCors(_bucketName);
                Assert.Fail("Get bucket cors should not pass when it was not set or deleted");
            }
            catch(OssException e)
            {
                Assert.AreEqual(OssErrorCode.NoSuchCORSConfiguration, e.ErrorCode);
            }
        }

        [Test]
        public void EnableBucketCorsEmptyTest()
        {
            var sbcRequest = new SetBucketCorsRequest(_bucketName);
            var newRole = new CORSRule();
            try
            {
                sbcRequest.AddCORSRule(newRole);
                Assert.Fail("Add corsrule without any settings should not pass");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void EnableBucketCorsAddAndDeleteSingleRuleTest()
        {
            try
            {
                var sbcRequest = new SetBucketCorsRequest(_bucketName);
                sbcRequest.AddCORSRule(ConstructDummyCorsRule());
                _ossClient.SetBucketCors(sbcRequest);
                OssTestUtils.WaitForCacheExpire();
                var rules = OssTestUtils.ToArray<CORSRule>(_ossClient.GetBucketCors(_bucketName));
                Assert.AreEqual(1, rules.Count);
            }
            finally
            {
                _ossClient.DeleteBucketCors(_bucketName);
                OssTestUtils.WaitForCacheExpire();
                try
                {
                    _ossClient.GetBucketCors(_bucketName);
                    Assert.Fail("Get bucket cors should not pass when it was not set or deleted");
                }
                catch (OssException e)
                {
                    Assert.AreEqual(OssErrorCode.NoSuchCORSConfiguration, e.ErrorCode);
                }
            }
        }

        [Test]
        public void EnableBucketCorsAddAndDeleteMultipleRulesTest()
        {
            try
            {
                var sbcRequest = new SetBucketCorsRequest(_bucketName);
                sbcRequest.AddCORSRule(ConstructDummyCorsRule());
                sbcRequest.AddCORSRule(ConstructDummyCorsRule());
                _ossClient.SetBucketCors(sbcRequest);
                OssTestUtils.WaitForCacheExpire();
                var rules = OssTestUtils.ToArray <CORSRule>(_ossClient.GetBucketCors(_bucketName));
                Assert.AreEqual(2, rules.Count);
            }
            finally
            {
                _ossClient.DeleteBucketCors(_bucketName);
            }
        }

        [Test]
        public void EnableBucketCorsAddInvalidSingleRuleTest()
        {
            try
            {
                var sbcRequest = new SetBucketCorsRequest(_bucketName);
                var rule = new CORSRule();
                rule.AddAllowedOrigin("Original " + Guid.NewGuid());
                rule.AddAllowedMethod("GETGET");
                sbcRequest.AddCORSRule(rule);
                _ossClient.SetBucketCors(sbcRequest);
                Assert.Fail("Invalid Cors Rule should not be created successfully");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
            finally
            {
                _ossClient.DeleteBucketCors(_bucketName);
            }
        }

        private static CORSRule ConstructDummyCorsRule()
        {
            var rule = new CORSRule();
            rule.AddAllowedOrigin("Original " + Guid.NewGuid());
            rule.AddAllowedMethod("GET");
            rule.AddAllowedHeader("HTTP");
            rule.AddExposeHeader("HTTP");
            return rule;
        }
    }
}
