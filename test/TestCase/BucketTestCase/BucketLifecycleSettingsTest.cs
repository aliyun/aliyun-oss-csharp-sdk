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
            catch (ArgumentException)
            { }

            try
            {
                new SetBucketLifecycleRequest(null);
                Assert.Fail();
            }
            catch (ArgumentException) { }

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

            rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "object";
            rule.Status = RuleStatus.Enabled;
            rule.CreatedBeforeDate = DateTime.UtcNow.Date.AddDays(200);
            Test(rule);

            rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "object";
            rule.Status = RuleStatus.Disabled;
            rule.CreatedBeforeDate = DateTime.UtcNow.Date.AddDays(365);
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

            rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "object";
            rule.Status = RuleStatus.Disabled;
            rule.CreatedBeforeDate = DateTime.UtcNow.Date.AddDays(365);

            rule.AbortMultipartUpload = new LifecycleRule.LifeCycleExpiration()
            {
                CreatedBeforeDate = DateTime.UtcNow.Date.AddDays(200)
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


        [Test]
        public void LifecycleWithTagsSettingTest()
        {
            LifecycleRule rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "test";
            rule.Status = RuleStatus.Enabled;
            rule.ExpriationDays = 200;
            rule.Tags = new Tag[2]
            {
                new Tag(){
                    Key = "key1",
                    Value = "value1"
                },
                new Tag(){
                    Key = "key2",
                    Value = "value2"
                }
            };
            Test(rule);

            rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "object";
            rule.Status = RuleStatus.Disabled;
            rule.ExpriationDays = 365;
            Test(rule);

            rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "object";
            rule.Status = RuleStatus.Enabled;
            rule.CreatedBeforeDate = DateTime.UtcNow.Date.AddDays(200);
            rule.Tags = new Tag[1]
            {
                new Tag(){
                    Key = "key1",
                    Value = "value1"
                }
            };

            Test(rule);

            rule = new LifecycleRule();
            rule.ID = "StandardExpireRule" + Guid.NewGuid();
            rule.Prefix = "object";
            rule.Status = RuleStatus.Disabled;
            rule.CreatedBeforeDate = DateTime.UtcNow.Date.AddDays(365);
            rule.Tags = new Tag[3]
            {
                new Tag(){
                    Key = "key1",
                    Value = "value1"
                },
                new Tag(){
                    Key = "key2",
                    Value = "value1"
                },
                new Tag(){
                    Key = "key3",
                    Value = "value1"
                }
            };
            Test(rule);
        }

        [Test]
        public void LifecycleWithTagsAdvancedSettingTest()
        {
            LifecycleRule rule1 = new LifecycleRule();
            rule1.ID = "StandardExpireRule" + Guid.NewGuid();
            rule1.Prefix = "test";
            rule1.Status = RuleStatus.Enabled;
            rule1.ExpriationDays = 200;
            rule1.AbortMultipartUpload = new LifecycleRule.LifeCycleExpiration()
            {
                CreatedBeforeDate = DateTime.UtcNow.Date.AddDays(400)
            };

            LifecycleRule rule2 = new LifecycleRule();
            rule2.ID = "StandardExpireRule" + Guid.NewGuid();
            rule2.Prefix = "test2";
            rule2.Status = RuleStatus.Enabled;
            rule2.ExpriationDays = 400;
            rule2.Transitions = new LifecycleRule.LifeCycleTransition[2]
            {
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.IA
                },
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.Archive
                }
            };
            rule2.Transitions[0].LifeCycleExpiration.Days = 180;
            rule2.Transitions[1].LifeCycleExpiration.Days = 365;
            rule2.Tags = new Tag[2]
            {
                new Tag(){
                    Key = "key1",
                    Value = "value1"
                },
                new Tag(){
                    Key = "key2",
                    Value = "value2"
                }
            };

            LifecycleRule rule3 = new LifecycleRule();
            rule3.ID = "StandardExpireRule" + Guid.NewGuid();
            rule3.Prefix = "object";
            rule3.Status = RuleStatus.Disabled;
            rule3.CreatedBeforeDate = DateTime.UtcNow.Date.AddDays(365);
            rule3.Tags = new Tag[3]
            {
                new Tag(){
                    Key = "key3-1",
                    Value = "value3-1"
                },
                new Tag(){
                    Key = "key3-2",
                    Value = "value3-2"
                },
                new Tag(){
                    Key = "key3-3",
                    Value = "value3-3"
                }
            };

            SetBucketLifecycleRequest req = new SetBucketLifecycleRequest(_bucketName);
            req.AddLifecycleRule(rule1);
            req.AddLifecycleRule(rule2);
            req.AddLifecycleRule(rule3);
            _ossClient.SetBucketLifecycle(req);
            OssTestUtils.WaitForCacheExpire();
            var rules = _ossClient.GetBucketLifecycle(_bucketName);
            Assert.IsTrue(rules.Count == 3);
            Assert.AreEqual(rules[0], rule1);
            Assert.AreEqual(rules[1], rule2);
            Assert.AreEqual(rules[2], rule3);

            Assert.AreEqual(rules[0].Tags, null);
            Assert.AreEqual(rules[1].Tags[0].Key, "key1");
            Assert.AreEqual(rules[1].Tags[0].Value, "value1");
            Assert.AreEqual(rules[2].Tags[1].Key, "key3-2");
            Assert.AreEqual(rules[2].Tags[1].Value, "value3-2");

            _ossClient.DeleteBucketLifecycle(_bucketName);
        }
    }
}
