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
        public void GetBucketNotSetWebsiteTest()
        {
            _ossClient.DeleteBucketWebsite(_bucketName);
            try
            {
                _ossClient.GetBucketWebsite(_bucketName);
                Assert.Fail("Get bucket website should not pass when it was not set or deleted");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.NoSuchWebsiteConfiguration, e.ErrorCode);
            }
        }

        [Test]
        public void SetBucketWebsiteTest()
        {
            try
            {
                const string indexPage = "index.html";
                const string errorPage = "NotFound.html";
                var sbwRequest = new SetBucketWebsiteRequest(_bucketName, indexPage, errorPage);
                _ossClient.SetBucketWebsite(sbwRequest);
                OssTestUtils.WaitForCacheExpire();
                var swResult = _ossClient.GetBucketWebsite(_bucketName);
                Assert.AreEqual(swResult.IndexDocument, indexPage);
                Assert.AreEqual(swResult.ErrorDocument, errorPage);
            }
            finally
            {
                _ossClient.DeleteBucketWebsite(_bucketName);
                try
                {
                    _ossClient.GetBucketWebsite(_bucketName);
                    Assert.Fail("Get bucket website should not pass when it was not set or deleted");
                }
                catch (OssException e)
                {
                    Assert.AreEqual(OssErrorCode.NoSuchWebsiteConfiguration, e.ErrorCode);
                }
            }
        }

        [Test]
        public void SetBucketWebsiteInvalidInputTest()
        {
            foreach (var invalidPageName in OssTestUtils.InvalidPageNamesList)
            {
                try
                {
                    var sbwRequest = new SetBucketWebsiteRequest(_bucketName, invalidPageName, invalidPageName);
                    _ossClient.SetBucketWebsite(sbwRequest);
                    Assert.Fail("Set Bucket website should fail with invalid page names");
                }
                catch (ArgumentException)
                {
                    Assert.IsTrue(true);
                }
            }
        }
    }
}
