using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    [TestFixture]
    public partial class BucketSettingsTest
    {
        [Test]
        public void LifecycleEmptySettingTest()
        {
            SetBucketLifecycleRequest req = new SetBucketLifecycleRequest(_bucketName);
            try
            {
                _ossClient.SetBucketLifecycle(req);
                Assert.Fail();
            }
            catch(ArgumentException)
            {}

            try
            {
                new SetBucketLifecycleRequest(null);
                Assert.Fail();
            }
            catch(ArgumentException){}

            try
            {
                new SetBucketLifecycleRequest(string.Empty);
                Assert.Fail();
            }
            catch (ArgumentException) { }
        }

        [Test]
        public void LifecycleBasicSettingTest()
        {
            LifecycleRule rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "test";
            rule.Status = RuleStatus.Enabled;
            rule.ExpriationDays = 200;
            Test(rule);

            rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "object";
            rule.Status = RuleStatus.Disabled;
            rule.ExpriationDays = 365;
            Test(rule);
        }

        [Test]
        public void LifecycleAdvancedSettingTest()
        {
            LifecycleRule rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "test";
            rule.Status = RuleStatus.Enabled;
            rule.ExpriationDays = 400;

            rule.AbortMultipartUpload = new LifecycleRule.LifeCycleExpiration()
            {
                CreatedBeforeDate = DateTime.UtcNow.Date.AddDays(400)
            };
            rule.Transitions = new LifecycleRule.LifeCycleTransition[2]
            {
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.IA
                },
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.Archive
                }
            };
            rule.Transitions[0].LifeCycleExpiration.Days = 180;
            rule.Transitions[1].LifeCycleExpiration.Days = 365;
            Test(rule);

            rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "object";
            rule.Status = RuleStatus.Disabled;
            rule.ExpriationDays = 365;

            rule.AbortMultipartUpload = new LifecycleRule.LifeCycleExpiration()
            {
                Days = 200
            };
            rule.Transitions = new LifecycleRule.LifeCycleTransition[1]
            {
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.Archive
                }
            };

            rule.Transitions[0].LifeCycleExpiration.Days = 250;
            Test(rule);
        }

        private void Test(LifecycleRule rule)
        {
            string bucket = _bucketName;
            SetBucketLifecycleRequest req = new SetBucketLifecycleRequest(bucket);
            req.AddLifecycleRule(rule);
            _ossClient.SetBucketLifecycle(req);
            OssTestUtils.WaitForCacheExpire();
            var rules = _ossClient.GetBucketLifecycle(bucket);
            Assert.IsTrue(rules.Count == 1);
            Assert.AreEqual(rules[0], rule);
            _ossClient.DeleteBucketLifecycle(bucket);
        }
    }
}
