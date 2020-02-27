using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Test.Util;
using Aliyun.OSS.Util;
using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.OtherTestClass
{
    [TestFixture]
    public class UtilsTest
    {
        [OneTimeSetUp]
        public static void ClassInitialize()
        {
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
        }

        internal class ServiceRequestMock : ServiceRequest
        {
            public override IDictionary<String, String> Headers
            {
                get { return null; }
            }
        }

        [Test]
        public void SignUtilsTest()
        {
            var request = new ServiceRequest();
            var signString = SignUtils.BuildCanonicalString("PUT", "/bucket", request);
            Assert.AreEqual(signString, "PUT\n\n\n/bucket");

            request = new ServiceRequestMock();
            signString = SignUtils.BuildCanonicalString("PUT", "/bucket", request);
            Assert.AreEqual(signString, "PUT\n\n\n/bucket");
        }

        [Test]
        public void OssUtilsTest()
        {
            var conf = new ClientConfiguration();

            Assert.AreEqual(OssUtils.IsBucketNameValid(""), false);

            var str = OssUtils.MakeResourcePath(new Uri("http://192.168.1.1"), "bucket", "key/abc/");
            Assert.AreEqual(str, "bucket/key/abc/");

            //cname 
            conf.IsCname = true;
            var uri = OssUtils.MakeBucketEndpoint(new Uri("http://endpoint"), "bucket", conf);
            Assert.AreEqual(uri.ToString(), "http://endpoint/");

            //ip endpoint & endpoint port
            conf.IsCname = false;
            uri = OssUtils.MakeBucketEndpoint(new Uri("http://192.168.1.1"), "bucket", conf);
            Assert.AreEqual(uri.ToString(), "http://192.168.1.1/");

            uri = OssUtils.MakeBucketEndpoint(new Uri("http://192.168.1.1:3128"), "bucket", conf);
            Assert.AreEqual(uri.ToString(), "http://192.168.1.1:3128/");

            //bucket
            conf.IsCname = false;
            uri = OssUtils.MakeBucketEndpoint(new Uri("http://endpoint"), null, conf);
            Assert.AreEqual(uri.ToString(), "http://endpoint/");

            conf.IsCname = false;
            uri = OssUtils.MakeBucketEndpoint(new Uri("http://endpoint"), "bucket", conf);
            Assert.AreEqual(uri.ToString(), "http://bucket.endpoint/");

            //TrimQuotes
            Assert.AreEqual(OssUtils.TrimQuotes("\"test\""), "test");
            Assert.AreEqual(OssUtils.TrimQuotes("test\""), "test");
            Assert.AreEqual(OssUtils.TrimQuotes("\"test"), "test");
            Assert.AreEqual(OssUtils.TrimQuotes(null), null);

            //ComputeContentCrc64
            var content = new MemoryStream(Encoding.ASCII.GetBytes(""));
            str = OssUtils.ComputeContentCrc64(content, 1);
            Assert.AreEqual(str, string.Empty);

            //JoinETag
            var etagList = new List<string>();
            etagList.Add("etag1");
            etagList.Add("etag2");
            str = OssUtils.JoinETag(etagList);
            Assert.AreEqual(str, "etag1, etag2");

            //GetResourcePathFromSignedUrl
            str = OssUtils.GetResourcePathFromSignedUrl(new Uri("http://endpoint/key"));
            Assert.AreEqual(str, "key");

            str = OssUtils.GetResourcePathFromSignedUrl(new Uri("http://endpoint"));
            Assert.AreEqual(str, "");

            //GetParametersFromSignedUrl

        }

        [Test]
        public void OssRequestSignerTest()
        {
            var request = new ServiceRequest();
            var credentials = new Common.Authentication.DefaultCredentials("accessKeyId", " ", "securityToken");
            var signer = new OssRequestSigner("");
            signer.Sign(request, credentials);

            try
            {
                credentials = new Common.Authentication.DefaultCredentials("accessKeyId", "", "securityToken");
                signer = new OssRequestSigner("");
                signer.Sign(request, credentials);
                Assert.IsTrue(false);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void IoUtilsTest()
        {
            var src = new MemoryStream(Encoding.ASCII.GetBytes(""));
            var dst = new MemoryStream();
            var len = IoUtils.WriteTo(src, dst, 1);
            Assert.AreEqual(len, 0);

            src = new MemoryStream(Encoding.ASCII.GetBytes("1234567"));
            dst = new MemoryStream();
            len = IoUtils.WriteTo(src, dst, 2);
            Assert.AreEqual(len, 2);
        }

        [Test]
        public void HttpUtilsTest()
        {
            var str = HttpUtils.GetContentType(null, null);
            Assert.AreEqual(str, "application/octet-stream");
        }

        [Test]
        public void ExceptionFactoryTest()
        {
            var exception = ExceptionFactory.CreateException("code", "msg", "id", "host");
            var exception1 = ExceptionFactory.CreateException("code1", "msg1", "id1", "host1", exception);
            Assert.AreEqual(exception1.InnerException, exception);

            try
            {
                ExceptionFactory.CreateInvalidResponseException(exception);
                Assert.IsTrue(false);
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual(e.InnerException, exception);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void EnumUtilsTest()
        {
        }
    }
}
