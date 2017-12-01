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
        public void LifecycleBasicSettingTest()
        {
            LifecycleRule rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "test";
            rule.Status = RuleStatus.Enabled;
            rule.ExpirationTime = DateTime.UtcNow.Date.AddDays(365);
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
            rule.ExpirationTime = DateTime.UtcNow.Date.AddDays(365);
            rule.AbortMultipartUpload = new LifecycleRule.LifeCycleExpiration()
            {
                ExpirationTime = DateTime.UtcNow.Date.AddDays(200)
            };
            rule.Transition = new LifecycleRule.LifeCycleTransition()
            {
                ExpirationTime = DateTime.UtcNow.Date.AddDays(180),
                StorageClass = StorageClass.IA                                         
            };
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
            rule.Transition = new LifecycleRule.LifeCycleTransition()
            {
                Days = 250,
                StorageClass = StorageClass.Archive
            };
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
