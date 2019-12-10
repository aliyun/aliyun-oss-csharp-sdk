using System;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public partial class BucketSettingsTest
    {
        private const string LogPrefix = "LoggingTest";

        [Test]
        public void GetBucketNotSetLoggingTest()
        {
            _ossClient.DeleteBucketLogging(_bucketName);
            //get should not throw exception
             _ossClient.GetBucketLogging(_bucketName);
        }

        [Test]
        public void EnableLoggingTest()
        {
            var sblRequest = new SetBucketLoggingRequest(_bucketName, _bucketName, LogPrefix);
            try
            {
                _ossClient.SetBucketLogging(sblRequest);
                OssTestUtils.WaitForCacheExpire();
                var blResponse = _ossClient.GetBucketLogging(_bucketName);
                Assert.AreEqual(_bucketName, blResponse.TargetBucket);
                Assert.AreEqual(LogPrefix, blResponse.TargetPrefix);
            }
            catch (OssException e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                _ossClient.DeleteBucketLogging(_bucketName);
                _ossClient.GetBucketLogging(_bucketName);
            }
        }

        [Test]
        public void EnableLoggingInvalidTargetBucketNameTest()
        {
            foreach (var invalidBucketName in OssTestUtils.InvalidBucketNamesList)
            {
                try
                {
                    var sblRequest = new SetBucketLoggingRequest(_bucketName, invalidBucketName, LogPrefix);
                    _ossClient.SetBucketLogging(sblRequest);
                    Assert.Fail("Set Bucket logging should not pass with invalid target bucket name");
                }
                catch (ArgumentException)
                {
                    Assert.IsTrue(true);
                }
                finally
                {
                    _ossClient.DeleteBucketLogging(_bucketName);
                }
            }
        }

        [Test]
        public void EnableLoggingNonExistTargetBucketNameTest()
        {
            //generate the target bucket name
            var targetBucketName = OssTestUtils.GetBucketName(_className);
            //target bucket should not exist
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, targetBucketName));

            var sblRequest = new SetBucketLoggingRequest(_bucketName, targetBucketName, LogPrefix);
            try
            {
                _ossClient.SetBucketLogging(sblRequest);
                Assert.Fail("Set Bucket logging should not pass with non exist target bucket name");

            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.InvalidTargetBucketForLogging, e.ErrorCode);
            }
            finally
            {
                _ossClient.DeleteBucketLogging(_bucketName);
            }
        }

        [Test]
        public void EnableLoggingInvalidPrefixNameTest()
        {
            foreach (var invalidPrefix in OssTestUtils.InvalidLoggingPrefixNamesList)
            {
                try
                {
                    var sblRequest = new SetBucketLoggingRequest(_bucketName, _bucketName, invalidPrefix);
                    _ossClient.SetBucketLogging(sblRequest);
                    Assert.Fail("Set Bucket logging should not pass with invalid logging prefix {0}", invalidPrefix);
                }
                catch (ArgumentException)
                {
                    Assert.IsTrue(true);
                }
                finally
                {
                    _ossClient.DeleteBucketLogging(_bucketName);
                }
            }
        }

        [Test]
        public void EnableLoggingWithEmtpyPrefixNameTest()
        {
            var sblRequest = new SetBucketLoggingRequest(_bucketName, _bucketName, "");
            try
            {
                _ossClient.SetBucketLogging(sblRequest);
                OssTestUtils.WaitForCacheExpire();
                var blResponse = _ossClient.GetBucketLogging(_bucketName);
                Assert.AreEqual(_bucketName, blResponse.TargetBucket);
                Assert.AreEqual("", blResponse.TargetPrefix);
            }
            catch (OssException e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                _ossClient.DeleteBucketLogging(_bucketName);
                _ossClient.GetBucketLogging(_bucketName);
            }
        }
    }
}
