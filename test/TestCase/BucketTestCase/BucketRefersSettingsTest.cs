using System.Collections.Generic;

using Aliyun.OSS;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public partial class BucketSettingsTest
    {
        [Test]
        public void GetBucketDefaultRefersTest()
        {
            //create a new bucket
            var bucketName = OssTestUtils.GetBucketName(_className);
            try
            {
                _ossClient.CreateBucket(bucketName);
                //check the default value
                var referConfig = _ossClient.GetBucketReferer(bucketName);
                Assert.IsNull(referConfig.RefererList.Referers);
                Assert.IsTrue(referConfig.AllowEmptyReferer);
            }
            finally
            {
                if (OssTestUtils.BucketExists(_ossClient, bucketName))
                {
                    OssTestUtils.CleanBucket(_ossClient, bucketName);
                }
            }
        }

        [Test]
        public void SetBucketRefersPositiveTest()
        {
            //initialize refer list
            var referList = new List<string>
            {"http://*.aliyun.com", "http://wwww.alibaba.com"};
            //use construct which pass in 3 parameters
            var sbrRequest = new SetBucketRefererRequest(_bucketName, referList, false);
            _ossClient.SetBucketReferer(sbrRequest);
            OssTestUtils.WaitForCacheExpire();
            var referConfig = _ossClient.GetBucketReferer(_bucketName);
            Assert.AreEqual(2, referConfig.RefererList.Referers.Length);
            Assert.IsFalse(referConfig.AllowEmptyReferer);


            referList.Add("http://www.taobao?.com");
            //use construct which pass in 2 parameters, and allowEmptyRefer set to true
            sbrRequest = new SetBucketRefererRequest(_bucketName, referList);
            _ossClient.SetBucketReferer(sbrRequest);
            OssTestUtils.WaitForCacheExpire();
            referConfig = _ossClient.GetBucketReferer(_bucketName);
            Assert.AreEqual(3, referConfig.RefererList.Referers.Length);
            //it is true this time
            Assert.IsTrue(referConfig.AllowEmptyReferer);

            //use construct which pass in 1 parameter, which means set it back to init status
            sbrRequest = new SetBucketRefererRequest(_bucketName);
            _ossClient.SetBucketReferer(sbrRequest);
            OssTestUtils.WaitForCacheExpire();
            referConfig = _ossClient.GetBucketReferer(_bucketName);
            Assert.IsNull(referConfig.RefererList.Referers);
            Assert.IsTrue(referConfig.AllowEmptyReferer);
        }

        [Test]
        public void SetBucketRefersNullListTest()
        {
            var sbrRequest = new SetBucketRefererRequest(_bucketName, null);
            _ossClient.SetBucketReferer(sbrRequest);
            OssTestUtils.WaitForCacheExpire();
            var referConfig = _ossClient.GetBucketReferer(_bucketName);
            Assert.IsNull(referConfig.RefererList.Referers);
        }

        [Test]
        public void SetBucketRefersEmptyListTest()
        {
            var sbrRequest = new SetBucketRefererRequest(_bucketName, new List<string>(), false);
            _ossClient.SetBucketReferer(sbrRequest);
            OssTestUtils.WaitForCacheExpire();
            var referConfig = _ossClient.GetBucketReferer(_bucketName);
            Assert.IsNull(referConfig.RefererList.Referers);
            Assert.IsFalse(referConfig.AllowEmptyReferer);
        }

        [Test]
        public void SetBucketRefersNullElementTest()
        {
            //initialize refer list
            var referList = new List<string> { null };
            var sbrRequest = new SetBucketRefererRequest(_bucketName, referList);
            _ossClient.SetBucketReferer(sbrRequest);
            OssTestUtils.WaitForCacheExpire();
            var referConfig = _ossClient.GetBucketReferer(_bucketName);
            Assert.IsNull(referConfig.RefererList.Referers);
        }

        [Test]
        public void SetBucketRefersEmptyElementTest()
        {
            //initialize refer list
            var referList = new List<string> { string.Empty };
            var sbrRequest = new SetBucketRefererRequest(_bucketName, referList);
            _ossClient.SetBucketReferer(sbrRequest);
            OssTestUtils.WaitForCacheExpire();
            var referConfig = _ossClient.GetBucketReferer(_bucketName);
            Assert.IsNull(referConfig.RefererList.Referers);
        }
    }
}
