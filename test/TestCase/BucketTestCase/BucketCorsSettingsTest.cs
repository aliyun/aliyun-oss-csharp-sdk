using System;
using System.Collections.Generic;
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
                sbcRequest.AddCORSRule(ConstructDummyCorsRuleWithMultiAllowedMethod());
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

        [Test]
        public void SetBucketCorsRequestInvalidArgumentTest()
        {
            try
            {
                var sbcRequest = new SetBucketCorsRequest(_bucketName);
                sbcRequest.AddCORSRule(null);
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("corsRule should not be null or empty"));
            }

            try
            {
                var sbcRequest = new SetBucketCorsRequest(_bucketName);
                var rule = new CORSRule();
                rule.AddAllowedOrigin("Original " + Guid.NewGuid());
                rule.AddAllowedMethod("GET");
                for (var i = 0; i < 12; i++)
                {
                    sbcRequest.AddCORSRule(rule);
                }
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("One bucket not allow exceed ten item of CORSRules"));
            }

            try
            {
                var sbcRequest = new SetBucketCorsRequest(_bucketName);
                var rule = new CORSRule();
                sbcRequest.AddCORSRule(rule);
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("corsRule.AllowedOrigins should not be empty"));
            }

            try
            {
                var sbcRequest = new SetBucketCorsRequest(_bucketName);
                var rule = new CORSRule();
                rule.AddAllowedOrigin("Original " + Guid.NewGuid());
                sbcRequest.AddCORSRule(rule);
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("corsRule.AllowedMethods should not be empty"));
            }

            try
            {
                var sbcRequest = new SetBucketCorsRequest(_bucketName);
                sbcRequest.CORSRules = null;
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("CORSRule list should not be null"));
            }

            try
            {
                var corsRules = new List<CORSRule>();
                var rule = new CORSRule();
                rule.AddAllowedOrigin("Original " + Guid.NewGuid());
                rule.AddAllowedMethod("GET");
                for (var i = 0; i < 9; i++)
                {
                    corsRules.Add(rule);
                }
                var sbcRequest = new SetBucketCorsRequest(_bucketName);
                sbcRequest.CORSRules = corsRules;
                Assert.IsTrue(true);

                corsRules.Add(rule);
                corsRules.Add(rule);
                sbcRequest.CORSRules = corsRules;
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains(" bucket not allow exceed ten item of CORSRules"));
            }
        }

        [Test]
        public void SetBucketCORSRuleInvalidArgumentTest()
        {
            try
            {
                var allowedOrigins = new List<String>();
                allowedOrigins.Add("*");
                allowedOrigins.Add("*");
                var rule = new CORSRule();
                rule.AllowedOrigins = allowedOrigins;
                Assert.Fail("Invalid argument should not be successful");
            }
            catch(Exception e)
            {
                Assert.IsTrue(e.Message.Contains("At most one asterisk wildcard allowed"));
            }

            try
            {
                var allowedHeaders = new List<String>();
                allowedHeaders.Add("*");
                allowedHeaders.Add("*");
                var rule = new CORSRule();
                rule.AllowedHeaders = allowedHeaders;
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("At most one asterisk wildcard allowed"));
            }

            try
            {
                var exposeHeaders = new List<String>();
                exposeHeaders.Add("*");
                var rule = new CORSRule();
                rule.ExposeHeaders = exposeHeaders;
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Asterisk wildcard not allowed"));
            }

            try
            {
                var rule = new CORSRule();
                rule.MaxAgeSeconds = -1;
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("MaxAge should not less than 0 or greater than 999999999"));
            }

            try
            {
                var rule = new CORSRule();
                rule.MaxAgeSeconds = 999999999 + 1;
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("MaxAge should not less than 0 or greater than 999999999"));
            }

            try
            {
                var rule = new CORSRule();
                rule.AddAllowedHeader(null);
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("allowedHeader"));
            }

            try
            {
                var rule = new CORSRule();
                rule.AddAllowedOrigin("*");
                rule.AddAllowedOrigin("*");
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("At most one asterisk wildcard allowed"));
            }

            try
            {
                var rule = new CORSRule();
                rule.AddAllowedOrigin(null);
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("allowedOrigin"));
            }

            try
            {
                var rule = new CORSRule();
                rule.AddAllowedHeader("*");
                rule.AddAllowedHeader("*");
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("At most one asterisk wildcard allowed"));
            }

            try
            {
                var rule = new CORSRule();
                rule.AddExposeHeader(null);
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("exposedHeader"));
            }

            try
            {
                var rule = new CORSRule();
                rule.AddExposeHeader("*");
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Asterisk wildcard not allowed"));
            }

            try
            {
                var rule = new CORSRule();
                rule.AddAllowedMethod(null);
                Assert.Fail("Invalid argument should not be successful");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("allowedMethod should not be null or empty"));
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

        private static CORSRule ConstructDummyCorsRuleWithMultiAllowedMethod()
        {
            var rule = new CORSRule();
            rule.AddAllowedOrigin("Original " + Guid.NewGuid());
            rule.AddAllowedMethod("GET");
            rule.AddAllowedMethod("PUT");
            rule.AddAllowedMethod("DELETE");
            rule.AddAllowedMethod("POST");
            rule.AddAllowedMethod("HEAD");
            rule.MaxAgeSeconds = 120;
            return rule;
        }
    }
}
