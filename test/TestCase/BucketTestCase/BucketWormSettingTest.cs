using System;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public partial class BucketSettingsTest
    {
        [Test]
        public void BucketWormSettingTest()
        {
            //InitiateBucketWorm
            var initrequest = new InitiateBucketWormRequest(_bucketName, 365);
            var initResult = _ossClient.InitiateBucketWorm(initrequest);

            //GetBucketWorm
            var getresult = _ossClient.GetBucketWorm(_bucketName);
            Assert.AreEqual(getresult.State, BucketWormState.InProgress);
            Assert.AreEqual(getresult.RetentionPeriodInDays, 365);
            Assert.AreEqual(getresult.WormId, initResult.WormId);

            //Delete Bucketworm
            _ossClient.AbortBucketWorm(_bucketName);

            _ossClient.InitiateBucketWorm(initrequest);

            var getresult2 = _ossClient.GetBucketWorm(_bucketName);

            //Lock BucketWorm
            var comrequest = new CompleteBucketWormRequest(_bucketName, getresult2.WormId);
            _ossClient.CompleteBucketWorm(comrequest);

            var getresult3 = _ossClient.GetBucketWorm(_bucketName);
            Assert.AreEqual(getresult3.State, BucketWormState.Locked);

            //Extend BucketWorm
            var extendrequest = new ExtendBucketWormRequest(_bucketName, 366, getresult3.WormId);
            _ossClient.ExtendBucketWorm(extendrequest);
        }

    }
}

