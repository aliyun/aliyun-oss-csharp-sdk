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
            var InvalidPageNamesList = new List<string>
            {
            "a", ".html"
            };

            foreach (var invalidPageName in InvalidPageNamesList)
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

            InvalidPageNamesList = new List<string>
            {
                string.Empty, null
            };

            foreach (var invalidPageName in InvalidPageNamesList)
            {
                try
                {
                    var sbwRequest = new SetBucketWebsiteRequest(_bucketName, invalidPageName, invalidPageName);
                    _ossClient.SetBucketWebsite(sbwRequest);
                    Assert.Fail("Set Bucket website should fail with invalid page names");
                }
                catch (OssException)
                {
                    Assert.IsTrue(true);
                }
            }

            try
            {
                var sbwRequest = new SetBucketWebsiteRequest(_bucketName, "index.html", ".html");
                _ossClient.SetBucketWebsite(sbwRequest);
                Assert.Fail("Set Bucket website should fail with invalid page names");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void SetBucketWebsiteWithXmlDataTest()
        {
            try
            {
                string config =
                    @" 
                    <WebsiteConfiguration>
                        <IndexDocument>
                            <Suffix>index1.html</Suffix>
                            <SupportSubDir>true</SupportSubDir>
                            <Type>0</Type>
                        </IndexDocument>
                        <ErrorDocument>
                            <Key>NotFound.html</Key>
                        </ErrorDocument>
                    </WebsiteConfiguration>
                    ";

                const string indexPage = "index1.html";
                const string errorPage = "NotFound.html";
                var sbwRequest = new SetBucketWebsiteRequest(_bucketName, config);
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
    }
}
