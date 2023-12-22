
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Aliyun.OSS.Test.TestClass.OtherTestClass
{
    [TestFixture]
    public class SignerTest
    {

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
        }

        [Test]
        public void TestV1Sign()
        {
            var credentials = new DefaultCredentials("ak", "sk", "");
            var bucketName = "examplebucket";
            var keyName = "nelson";
            
            // Only Header
            var request = new ServiceRequest();
            request.Method = HttpMethod.Put;

            //DateTime.
            var time = DateUtils.ParseRfc822Date("Wed, 28 Dec 2022 10:27:41 GMT");
            request.Headers.Add("Date", DateUtils.FormatRfc822Date(time));
            request.Headers.Add("Content-MD5", "eB5eJF1ptWaXm4bijSPyxw==");
            request.Headers.Add("Content-Type", "text/html");
            request.Headers.Add("x-oss-meta-author", "alice");
            request.Headers.Add("x-oss-meta-magic", "abracadabra");
            request.Headers.Add("x-oss-date", "Wed, 28 Dec 2022 10:27:41 GMT");

            var singer = OssRequestSigner.Create(SignatureVersion.V1);
            singer.Bucket = bucketName;
            singer.Key = keyName;

            singer.Sign(request, credentials);

            var authPat = "OSS ak:kSHKmLxlyEAKtZPkJhG9bZb5k7M=";

            Assert.AreEqual(authPat, request.Headers["Authorization"]);

            // With Signed Parameter
            request = new ServiceRequest();
            request.Method = HttpMethod.Put;

            //DateTime.
            time = DateUtils.ParseRfc822Date("Wed, 28 Dec 2022 10:27:41 GMT");
            request.Headers.Add("Date", DateUtils.FormatRfc822Date(time));
            request.Headers.Add("Content-MD5", "eB5eJF1ptWaXm4bijSPyxw==");
            request.Headers.Add("Content-Type", "text/html");
            request.Headers.Add("x-oss-meta-author", "alice");
            request.Headers.Add("x-oss-meta-magic", "abracadabra");
            request.Headers.Add("x-oss-date", "Wed, 28 Dec 2022 10:27:41 GMT");

            request.Parameters.Add("acl", "");

            singer = OssRequestSigner.Create(SignatureVersion.V1);
            singer.Bucket = bucketName;
            singer.Key = keyName;

            singer.Sign(request, credentials);

            authPat = "OSS ak:/afkugFbmWDQ967j1vr6zygBLQk=";

            Assert.AreEqual(authPat, request.Headers["Authorization"]);

            // With signed & non-signed Parameter & non-signed headers
            request = new ServiceRequest();
            request.Method = HttpMethod.Put;

            //DateTime.
            time = DateUtils.ParseRfc822Date("Wed, 28 Dec 2022 10:27:41 GMT");
            request.Headers.Add("date", DateUtils.FormatRfc822Date(time));
            request.Headers.Add("content-md5", "eB5eJF1ptWaXm4bijSPyxw==");
            request.Headers.Add("content-type", "text/html");
            request.Headers.Add("x-oss-meta-author", "alice");
            request.Headers.Add("x-Oss-meta-magic", "abracadabra");
            request.Headers.Add("X-oss-date", "Wed, 28 Dec 2022 10:27:41 GMT");

            request.Headers.Add("User-Agent", "test");

            request.Parameters.Add("acl", "");
            request.Parameters.Add("non-resousce", "123");

            singer = OssRequestSigner.Create(SignatureVersion.V1);
            singer.Bucket = bucketName;
            singer.Key = keyName;

            singer.Sign(request, credentials);

            authPat = "OSS ak:/afkugFbmWDQ967j1vr6zygBLQk=";

            Assert.AreEqual(authPat, request.Headers["Authorization"]);
        }

        [Test]
        public void TestV1PreSign()
        {
            var credentials = new DefaultCredentials("ak", "sk", "");
            var bucketName = "bucket";
            var keyName = "key";

            var request = new ServiceRequest();
            request.Method = HttpMethod.Get;

            request.Parameters.Add("versionId", "versionId");

            //DateTime.
            var expiration = DateUtils.ParseRfc822Date("Sun, 12 Nov 2023 16:43:40 GMT");

            var singer = OssRequestSigner.Create(SignatureVersion.V1);
            singer.Bucket = bucketName;
            singer.Key = keyName;

            var signingContext = new SigningContext
            {
                Credentials = credentials,
                Expiration = expiration,
            };

            singer.PreSign(request, signingContext);

            Assert.AreEqual("1699807420", request.Parameters["Expires"]);
            Assert.AreEqual("ak", request.Parameters["OSSAccessKeyId"]);
            Assert.AreEqual("dcLTea+Yh9ApirQ8o8dOPqtvJXQ=", request.Parameters["Signature"]);
            Assert.AreEqual("versionId", request.Parameters["versionId"]);
        }

        [Test]
        public void TestV1PreSignToken()
        {
            var credentials = new DefaultCredentials("ak", "sk", "token");
            var bucketName = "bucket";
            var keyName = "key+123";

            var request = new ServiceRequest();
            request.Method = HttpMethod.Get;

            request.Parameters.Add("versionId", "versionId");

            //DateTime.
            var expiration = DateUtils.ParseRfc822Date("Sun, 12 Nov 2023 16:56:44 GMT");

            var singer = OssRequestSigner.Create(SignatureVersion.V1);
            singer.Bucket = bucketName;
            singer.Key = keyName;

            var signingContext = new SigningContext
            {
                Credentials = credentials,
                Expiration = expiration,
            };

            singer.PreSign(request, signingContext);

            Assert.AreEqual("1699808204", request.Parameters["Expires"]);
            Assert.AreEqual("ak", request.Parameters["OSSAccessKeyId"]);
            Assert.AreEqual("jzKYRrM5y6Br0dRFPaTGOsbrDhY=", request.Parameters["Signature"]);
            Assert.AreEqual("versionId", request.Parameters["versionId"]);
            Assert.AreEqual("token", request.Parameters["security-token"]);
        }

        [Test]
        public void TestV4Sign()
        {
            var credentials = new DefaultCredentials("ak", "sk", "");
            var bucketName = "bucket";
            var keyName = "1234+-/123/1.txt";
            var request = new ServiceRequest();
            request.Method = HttpMethod.Put;

            //DateTime.
            var time = DateTimeOffset.FromUnixTimeSeconds(1702743657).DateTime.ToLocalTime();
            request.Headers.Add("Date", DateUtils.FormatRfc822Date(time));

            request.Headers.Add("x-oss-head1", "value");
            request.Headers.Add("abc", "value");
            request.Headers.Add("ZAbc", "value");
            request.Headers.Add("XYZ", "value");
            request.Headers.Add("content-type", "text/plain");
            request.Headers.Add("x-oss-content-sha256", "UNSIGNED-PAYLOAD");

            request.Parameters.Add("param1", "value1");
            request.Parameters.Add("+param1", "value3");
            request.Parameters.Add("|param1", "value4");
            request.Parameters.Add("+param2", "");
            request.Parameters.Add("|param2", "");
            request.Parameters.Add("param2", "");

            var singer = OssRequestSigner.Create(SignatureVersion.V4);
            singer.Bucket = bucketName;
            singer.Key = keyName;
            singer.Region = "cn-hangzhou";
            singer.Product = "oss";

            singer.Sign(request, credentials);

            var authPat = "OSS4-HMAC-SHA256 Credential=ak/20231216/cn-hangzhou/oss/aliyun_v4_request,Signature=e21d18daa82167720f9b1047ae7e7f1ce7cb77a31e8203a7d5f4624fa0284afe";

            Assert.AreEqual(authPat, request.Headers["Authorization"]);
        }

        [Test]
        public void TestV4SignWithToken()
        {
            var credentials = new DefaultCredentials("ak", "sk", "token");
            var bucketName = "bucket";
            var keyName = "1234+-/123/1.txt";
            var request = new ServiceRequest();
            request.Method = HttpMethod.Put;

            //DateTime.
            var time = DateTimeOffset.FromUnixTimeSeconds(1702784856).DateTime.ToLocalTime();
            request.Headers.Add("Date", DateUtils.FormatRfc822Date(time));

            request.Headers.Add("x-oss-head1", "value");
            request.Headers.Add("abc", "value");
            request.Headers.Add("ZAbc", "value");
            request.Headers.Add("XYZ", "value");
            request.Headers.Add("content-type", "text/plain");
            request.Headers.Add("x-oss-content-sha256", "UNSIGNED-PAYLOAD");

            request.Parameters.Add("param1", "value1");
            request.Parameters.Add("+param1", "value3");
            request.Parameters.Add("|param1", "value4");
            request.Parameters.Add("+param2", "");
            request.Parameters.Add("|param2", "");
            request.Parameters.Add("param2", "");

            var singer = OssRequestSigner.Create(SignatureVersion.V4);
            singer.Bucket = bucketName;
            singer.Key = keyName;
            singer.Region = "cn-hangzhou";
            singer.Product = "oss";

            singer.Sign(request, credentials);

            var authPat = "OSS4-HMAC-SHA256 Credential=ak/20231217/cn-hangzhou/oss/aliyun_v4_request,Signature=b94a3f999cf85bcdc00d332fbd3734ba03e48382c36fa4d5af5df817395bd9ea";

            Assert.AreEqual(authPat, request.Headers["Authorization"]);
        }


        [Test]
        public void TestV4SignWithAdditionalHeaders()
        {
            var credentials = new DefaultCredentials("ak", "sk", "");
            var bucketName = "bucket";
            var keyName = "1234+-/123/1.txt";
            var request = new ServiceRequest();
            request.Method = HttpMethod.Put;

            //DateTime.
            var time = DateTimeOffset.FromUnixTimeSeconds(1702747512).DateTime.ToLocalTime();
            request.Headers.Add("Date", DateUtils.FormatRfc822Date(time));

            request.Headers.Add("x-oss-head1", "value");
            request.Headers.Add("abc", "value");
            request.Headers.Add("ZAbc", "value");
            request.Headers.Add("XYZ", "value");
            request.Headers.Add("content-type", "text/plain");
            request.Headers.Add("x-oss-content-sha256", "UNSIGNED-PAYLOAD");

            request.Parameters.Add("param1", "value1");
            request.Parameters.Add("+param1", "value3");
            request.Parameters.Add("|param1", "value4");
            request.Parameters.Add("+param2", "");
            request.Parameters.Add("|param2", "");
            request.Parameters.Add("param2", "");

            var singer = OssRequestSigner.Create(SignatureVersion.V4);
            singer.Bucket = bucketName;
            singer.Key = keyName;
            singer.Region = "cn-hangzhou";
            singer.Product = "oss";

            singer.AdditionalHeaders = new List<string>() { "ZAbc", "abc" };

            singer.Sign(request, credentials);

            var authPat = "OSS4-HMAC-SHA256 Credential=ak/20231216/cn-hangzhou/oss/aliyun_v4_request,AdditionalHeaders=abc;zabc,Signature=4a4183c187c07c8947db7620deb0a6b38d9fbdd34187b6dbaccb316fa251212f";

            Assert.AreEqual(authPat, request.Headers["Authorization"]);

            //  with default signed header
            var request1 = new ServiceRequest();
            request1.Method = HttpMethod.Put;

            //DateTime.
            request1.Headers.Add("Date", DateUtils.FormatRfc822Date(time));

            request1.Headers.Add("x-oss-head1", "value");
            request1.Headers.Add("abc", "value");
            request1.Headers.Add("ZAbc", "value");
            request1.Headers.Add("XYZ", "value");
            request1.Headers.Add("content-type", "text/plain");
            request1.Headers.Add("x-oss-content-sha256", "UNSIGNED-PAYLOAD");

            request1.Parameters.Add("param1", "value1");
            request1.Parameters.Add("+param1", "value3");
            request1.Parameters.Add("|param1", "value4");
            request1.Parameters.Add("+param2", "");
            request1.Parameters.Add("|param2", "");
            request1.Parameters.Add("param2", "");

            singer = OssRequestSigner.Create(SignatureVersion.V4);
            singer.Bucket = bucketName;
            singer.Key = keyName;
            singer.Region = "cn-hangzhou";
            singer.Product = "oss";

            singer.AdditionalHeaders = new List<string>() { "x-oss-no-exist", "ZAbc", "x-oss-head1", "abc" };

            singer.Sign(request1, credentials);

            Assert.AreEqual(authPat, request1.Headers["Authorization"]);
        }


        [Test]
        public void TestV4PreSign()
        {
            var credentials = new DefaultCredentials("ak", "sk", "");
            var bucketName = "bucket";
            var keyName = "1234+-/123/1.txt";
            var request = new ServiceRequest();
            request.Method = HttpMethod.Put;

            //DateTime.
            var signTime = DateTimeOffset.FromUnixTimeSeconds(1702781677).DateTime.ToLocalTime();
            var expiration = DateTimeOffset.FromUnixTimeSeconds(1702782276).DateTime.ToLocalTime();

            request.Headers.Add("x-oss-head1", "value");
            request.Headers.Add("abc", "value");
            request.Headers.Add("ZAbc", "value");
            request.Headers.Add("XYZ", "value");
            request.Headers.Add("content-type", "application/octet-stream");

            request.Parameters.Add("param1", "value1");
            request.Parameters.Add("+param1", "value3");
            request.Parameters.Add("|param1", "value4");
            request.Parameters.Add("+param2", "");
            request.Parameters.Add("|param2", "");
            request.Parameters.Add("param2", "");

            var singer = OssRequestSigner.Create(SignatureVersion.V4);
            singer.Bucket = bucketName;
            singer.Key = keyName;
            singer.Region = "cn-hangzhou";
            singer.Product = "oss";

            var signingContext = new SigningContext
            {
                Credentials = credentials,
                Expiration = expiration,
                SignTime = signTime,
            };

            singer.PreSign(request, signingContext);

            Assert.AreEqual("OSS4-HMAC-SHA256", request.Parameters["x-oss-signature-version"]);
            Assert.AreEqual("20231217T025437Z", request.Parameters["x-oss-date"]);
            Assert.AreEqual("599", request.Parameters["x-oss-expires"]);
            Assert.AreEqual("ak/20231217/cn-hangzhou/oss/aliyun_v4_request", request.Parameters["x-oss-credential"]);
            Assert.AreEqual("a39966c61718be0d5b14e668088b3fa07601033f6518ac7b523100014269c0fe", request.Parameters["x-oss-signature"]);
            Assert.AreEqual(false, request.Parameters.ContainsKey("x-oss-additional-headers"));
        }

        [Test]
        public void TestV4PreSignWithToken()
        {
            var credentials = new DefaultCredentials("ak", "sk", "token");
            var bucketName = "bucket";
            var keyName = "1234+-/123/1.txt";
            var request = new ServiceRequest();
            request.Method = HttpMethod.Put;

            //DateTime.
            var signTime = DateTimeOffset.FromUnixTimeSeconds(1702785388).DateTime.ToLocalTime();
            var expiration = DateTimeOffset.FromUnixTimeSeconds(1702785987).DateTime.ToLocalTime();

            request.Headers.Add("x-oss-head1", "value");
            request.Headers.Add("abc", "value");
            request.Headers.Add("ZAbc", "value");
            request.Headers.Add("XYZ", "value");
            request.Headers.Add("content-type", "application/octet-stream");

            request.Parameters.Add("param1", "value1");
            request.Parameters.Add("+param1", "value3");
            request.Parameters.Add("|param1", "value4");
            request.Parameters.Add("+param2", "");
            request.Parameters.Add("|param2", "");
            request.Parameters.Add("param2", "");

            var singer = OssRequestSigner.Create(SignatureVersion.V4);
            singer.Bucket = bucketName;
            singer.Key = keyName;
            singer.Region = "cn-hangzhou";
            singer.Product = "oss";

            var signingContext = new SigningContext
            {
                Credentials = credentials,
                Expiration = expiration,
                SignTime = signTime,
            };

            singer.PreSign(request, signingContext);

            Assert.AreEqual("OSS4-HMAC-SHA256", request.Parameters["x-oss-signature-version"]);
            Assert.AreEqual("20231217T035628Z", request.Parameters["x-oss-date"]);
            Assert.AreEqual("599", request.Parameters["x-oss-expires"]);
            Assert.AreEqual("ak/20231217/cn-hangzhou/oss/aliyun_v4_request", request.Parameters["x-oss-credential"]);
            Assert.AreEqual("3817ac9d206cd6dfc90f1c09c00be45005602e55898f26f5ddb06d7892e1f8b5", request.Parameters["x-oss-signature"]);
            Assert.AreEqual(false, request.Parameters.ContainsKey("x-oss-additional-headers"));
        }

        [Test]
        public void TestV4PreSignWithAdditionalHeaders()
        {
            var credentials = new DefaultCredentials("ak", "sk", "");
            var bucketName = "bucket";
            var keyName = "1234+-/123/1.txt";
            var request = new ServiceRequest();
            request.Method = HttpMethod.Put;

            //DateTime.
            var signTime = DateTimeOffset.FromUnixTimeSeconds(1702783809).DateTime.ToLocalTime();
            var expiration = DateTimeOffset.FromUnixTimeSeconds(1702784408).DateTime.ToLocalTime();

            request.Headers.Add("x-oss-head1", "value");
            request.Headers.Add("abc", "value");
            request.Headers.Add("ZAbc", "value");
            request.Headers.Add("XYZ", "value");
            request.Headers.Add("content-type", "application/octet-stream");

            request.Parameters.Add("param1", "value1");
            request.Parameters.Add("+param1", "value3");
            request.Parameters.Add("|param1", "value4");
            request.Parameters.Add("+param2", "");
            request.Parameters.Add("|param2", "");
            request.Parameters.Add("param2", "");

            var singer = OssRequestSigner.Create(SignatureVersion.V4);
            singer.Bucket = bucketName;
            singer.Key = keyName;
            singer.Region = "cn-hangzhou";
            singer.Product = "oss";
            singer.AdditionalHeaders = new List<string>() { "ZAbc", "abc" };

            var signingContext = new SigningContext
            {
                Credentials = credentials,
                Expiration = expiration,
                SignTime = signTime,
            };

            singer.PreSign(request, signingContext);

            Assert.AreEqual("OSS4-HMAC-SHA256", request.Parameters["x-oss-signature-version"]);
            Assert.AreEqual("20231217T033009Z", request.Parameters["x-oss-date"]);
            Assert.AreEqual("599", request.Parameters["x-oss-expires"]);
            Assert.AreEqual("ak/20231217/cn-hangzhou/oss/aliyun_v4_request", request.Parameters["x-oss-credential"]);
            Assert.AreEqual("6bd984bfe531afb6db1f7550983a741b103a8c58e5e14f83ea474c2322dfa2b7", request.Parameters["x-oss-signature"]);
            Assert.AreEqual("abc;zabc", request.Parameters["x-oss-additional-headers"]);


            // with default signed header
            var request1 = new ServiceRequest();
            request1.Method = HttpMethod.Put;

            request1.Headers.Add("x-oss-head1", "value");
            request1.Headers.Add("abc", "value");
            request1.Headers.Add("ZAbc", "value");
            request1.Headers.Add("XYZ", "value");
            request1.Headers.Add("content-type", "application/octet-stream");

            request1.Parameters.Add("param1", "value1");
            request1.Parameters.Add("+param1", "value3");
            request1.Parameters.Add("|param1", "value4");
            request1.Parameters.Add("+param2", "");
            request1.Parameters.Add("|param2", "");
            request1.Parameters.Add("param2", "");

            var singer1 = OssRequestSigner.Create(SignatureVersion.V4);
            singer1.Bucket = bucketName;
            singer1.Key = keyName;
            singer1.Region = "cn-hangzhou";
            singer1.Product = "oss";
            singer1.AdditionalHeaders = new List<string>() { "x-oss-no-exist", "abc", "x-oss-head1", "ZAbc" };

            var signingContext1 = new SigningContext
            {
                Credentials = credentials,
                Expiration = expiration,
                SignTime = signTime,
            };

            singer1.PreSign(request1, signingContext1);

            Assert.AreEqual("OSS4-HMAC-SHA256", request1.Parameters["x-oss-signature-version"]);
            Assert.AreEqual("20231217T033009Z", request1.Parameters["x-oss-date"]);
            Assert.AreEqual("599", request1.Parameters["x-oss-expires"]);
            Assert.AreEqual("ak/20231217/cn-hangzhou/oss/aliyun_v4_request", request1.Parameters["x-oss-credential"]);
            Assert.AreEqual("6bd984bfe531afb6db1f7550983a741b103a8c58e5e14f83ea474c2322dfa2b7", request1.Parameters["x-oss-signature"]);
            Assert.AreEqual("abc;zabc", request1.Parameters["x-oss-additional-headers"]);
        }

    }
}
