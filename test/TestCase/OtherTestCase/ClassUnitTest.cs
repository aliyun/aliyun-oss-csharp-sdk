﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aliyun.OSS.Common;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Common.ThirdParty;
using Aliyun.OSS.Test.Util;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;
using NUnit.Framework;
using ExecutionContext = Aliyun.OSS.Common.Communication.ExecutionContext;

namespace Aliyun.OSS.Test.TestClass.OtherTestClass
{
    [TestFixture]
    public class ClassUnitTest
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
        public void ServiceSignatureTest()
        {
            try
            {
                var signatuer = ServiceSignature.Create();
                signatuer.ComputeSignature("", "data");
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

            try
            {
                var signatuer = ServiceSignature.Create();
                signatuer.ComputeSignature("key", "");
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
        public void MD5CoreTest()
        {
            try
            {
                MD5Core.GetHash(null, null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                MD5Core.GetHash("", null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                byte[] content = null;
                MD5Core.GetHash(content);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                byte[] content = null;
                MD5Core.GetHashString(content);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                MD5Core.GetHashString(null, null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                MD5Core.GetHashString("", null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            string str32 = "";
            string str80 = "";
            string str124 = "";

            for (int i = 0; i < 32; i++)
                str32 += "a";
            for (int i = 0; i < 80; i++)
                str80 += "a";
            for (int i = 0; i < 124; i++)
                str124 += "a";

            MD5Core.GetHash(str32);
            MD5Core.GetHash(str80);
            MD5Core.GetHash(str124);
            MD5Core.GetHashString(str32);
            MD5Core.GetHashString(str80);
            MD5Core.GetHashString(str124);
        }


        [Test]
        public void WrapperStreamTest()
        {
            try
            {
                var warp0 = new WrapperStream(null);
            }
            catch (ArgumentNullException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            var content = new MemoryStream(Encoding.ASCII.GetBytes("123"));
            var warp1 = new WrapperStream(content);
            var warp2 = new WrapperStream(warp1);
            Assert.AreEqual(warp1.GetNonWrapperBaseStream(), content);
            Assert.AreEqual(warp2.GetNonWrapperBaseStream(), content);

            var seekWarp2 = warp2.GetSeekableBaseStream();

            Assert.AreEqual(WrapperStream.GetNonWrapperBaseStream(warp1), content);
            Assert.AreEqual(WrapperStream.GetNonWrapperBaseStream(content), content);

            Assert.AreEqual(warp1.CanWrite, true);

            warp1.Position = 2;
            Assert.AreEqual(content.Position, 2);

            try
            {
                warp1.ReadTimeout = 3;
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var timeout = warp1.ReadTimeout;
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                warp1.WriteTimeout = 3;
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var timeout = warp1.WriteTimeout;
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }


            warp1.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(content.Position, 0);

            warp1.SetLength(3);
            Assert.AreEqual(content.Length, 3);

        }

        [Test]
        public void PartialWrapperStreamTest()
        {
            var content = new MemoryStream(Encoding.ASCII.GetBytes("123"));
            content.Seek(1, SeekOrigin.Begin);
            var warp = new PartialWrapperStream(content, 0);
            Assert.AreEqual(warp.Length, 2);

            var warp1 = new PartialWrapperStream(content, 10);
            Assert.AreEqual(warp1.Length, 2);

            warp1.Position = 3;
            var ret = warp1.Read(null, 0, 0);
            Assert.AreEqual(ret, 0);

            warp1.Seek(0, SeekOrigin.Begin);
            warp1.Seek(1, SeekOrigin.Begin);
            warp1.Seek(4, SeekOrigin.Begin);
            warp1.Seek(0, SeekOrigin.Current);
            warp1.Seek(0, SeekOrigin.End);

            content.Seek(0, SeekOrigin.Begin);
            var md5Stream = new MD5Stream(content, null, content.Length);
            try
            {
                var warp2 = new PartialWrapperStream(md5Stream, 0);
                Assert.IsTrue(false);
            }
            catch (InvalidOperationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void HashingWrapperCrc64Test()
        {
            var hash = new HashingWrapperCrc64();
        }

        [Test]
        public void ServiceRequestTest()
        {
            string str;
            var request = new ServiceRequest();

            //BuildRequestUri
            request.Endpoint = new Uri("http://endpoint");
            request.ResourcePath = null;
            request.ParametersInUri = false;
            str = request.BuildRequestUri();
            Assert.AreEqual(str, "http://endpoint/");

            //BuildRequestContent
            var parameters = new Dictionary<string, string>();
            request.ParametersInUri = false;
            request.Content = null;
            request.Method = HttpMethod.Post;

            var stream = request.BuildRequestContent();
            Assert.AreEqual(stream, null);

            request.Dispose();
            request.Dispose();
        }

        [Test]
        public void DownloadObjectRequestTest()
        {
            var request = new DownloadObjectRequest("bucket", "key", "donwloadFile", "checkpointDir");
            request.NonmatchingETagConstraints.Add("etag1");
            request.NonmatchingETagConstraints.Add("etag2");

            //ToGetObjectRequest
            var objRequest = request.ToGetObjectRequest();
            Assert.AreEqual(objRequest.NonmatchingETagConstraints.Count, 2);


            //Populate
            var headers = new Dictionary<string, string>();
            request = new DownloadObjectRequest("bucket", "key", "donwloadFile", "checkpointDir");
            request.Populate(headers);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfModifiedSince), false);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfUnmodifiedSince), false);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfMatch), false);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfNoneMatch), false);

            request.ModifiedSinceConstraint = DateTime.UtcNow.AddDays(-1);
            request.UnmodifiedSinceConstraint = DateTime.UtcNow.AddDays(-1);
            request.MatchingETagConstraints.Add("etag1");
            request.NonmatchingETagConstraints.Add("etag2");
            request.Populate(headers);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfModifiedSince), true);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfUnmodifiedSince), true);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfMatch), true);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfNoneMatch), true);

            headers = new Dictionary<string, string>();
            request.RequestPayer = RequestPayer.Requester;
            request.Populate(headers);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.OssRequestPayer), true);
        }

        [Test]
        public void GeneratePresignedUriRequestTest()
        {
            var request = new GeneratePresignedUriRequest("bucket", "key");

            //Method
            request.Method = SignHttpMethod.Get;
            Assert.AreEqual(request.Method, SignHttpMethod.Get);
            request.Method = SignHttpMethod.Put;
            Assert.AreEqual(request.Method, SignHttpMethod.Put);
            try
            {
                request.Method = SignHttpMethod.Post;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //ResponseHeaders
            var responseHeader = new ResponseHeaderOverrides();
            request.ResponseHeaders = responseHeader;
            Assert.AreEqual(request.ResponseHeaders, responseHeader);
            try
            {
                responseHeader = null;
                request.ResponseHeaders = responseHeader;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //QueryParams
            var queryParams = new Dictionary<string, string>();
            request.QueryParams = queryParams;
            Assert.AreEqual(request.QueryParams, queryParams);
            try
            {
                queryParams = null;
                request.QueryParams = queryParams;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void GetObjectRequestTest()
        {
            //Populate
            var headers = new Dictionary<string, string>();
            var request = new GetObjectRequest("bucket", "key");
            request.SetRange(-1, 0);
            request.Populate(headers);
            Assert.AreEqual(headers[HttpHeaders.Range], "bytes=-0");

            headers = new Dictionary<string, string>();
            request.SetRange(10, -1);
            request.Populate(headers);
            Assert.AreEqual(headers[HttpHeaders.Range], "bytes=10-");
        }

        [Test]
        public void SetBucketLifecycleRequestTest()
        {
            var request = new SetBucketLifecycleRequest("bucket");
            var rule = new LifecycleRule();
            rule.ID = "StandardExpireRule";
            rule.Prefix = "test";
            rule.Status = RuleStatus.Enabled;
            rule.ExpriationDays = 200;

            var rules = new List<LifecycleRule>();

            //LifecycleRules
            request.LifecycleRules = rules;
            try
            {
                request.LifecycleRules = null;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            try
            {
                for (int i = 0; i < 1002; i++)
                    rules.Add(rule);
                request.LifecycleRules = rules;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //AddLifecycleRule
            try
            {
                request.AddLifecycleRule(null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            try
            {
                request = new SetBucketLifecycleRequest("bucket");
                var rule1 = new LifecycleRule();
                request.AddLifecycleRule(rule1);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }


            try
            {
                request = new SetBucketLifecycleRequest("bucket");
                for (int i = 0; i < 10002; i++)
                {
                    request.AddLifecycleRule(rule);
                }
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void ObjectMetadataTest()
        {
            var meta = new ObjectMetadata();
            Assert.AreEqual(meta.LastModified, DateTime.MinValue);
            Assert.AreEqual(meta.ExpirationTime, DateTime.MinValue);
            meta.ExpirationTime = DateTime.UtcNow;
            Assert.AreNotEqual(meta.ExpirationTime, DateTime.MinValue);
            Assert.AreEqual(meta.ContentLength, -1L);
            meta.HttpMetadata.Remove(HttpHeaders.ContentLength);
            Assert.AreEqual(meta.ContentLength, 0);

            meta.ContentType = "";
            Assert.AreEqual(meta.ContentType, null);

            Assert.AreEqual(meta.ContentEncoding, null);
            meta.ContentEncoding = "gzip";
            Assert.AreEqual(meta.ContentEncoding, "gzip");

            meta.CacheControl = null;
            Assert.AreEqual(meta.CacheControl, null);
            meta.CacheControl = "data";
            Assert.AreEqual(meta.CacheControl, "data");

            meta.ContentDisposition = null;
            Assert.AreEqual(meta.ContentDisposition, null);
            meta.ContentDisposition = "data";
            Assert.AreEqual(meta.ContentDisposition, "data");

            meta.ETag = null;
            Assert.AreEqual(meta.ETag, null);
            meta.ETag = "data";
            Assert.AreEqual(meta.ETag, "data");

            meta.ContentMd5 = null;
            Assert.AreEqual(meta.ContentMd5, null);
            meta.ContentMd5 = "data";
            Assert.AreEqual(meta.ContentMd5, "data");

            meta.Crc64 = null;
            Assert.AreEqual(meta.Crc64, null);
            meta.Crc64 = "data";
            Assert.AreEqual(meta.Crc64, "data");

            //meta.ServerSideEncryption = null;
            Assert.AreEqual(meta.ServerSideEncryption, null);
            meta.ServerSideEncryption = "AES256";
            Assert.AreEqual(meta.ServerSideEncryption, "AES256");
            try
            {
                meta.ServerSideEncryption = "Unknwon";
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            Assert.AreEqual(meta.ObjectType, null);
            meta.HttpMetadata[HttpHeaders.ObjectType] = "Normal";
            Assert.AreEqual(meta.ObjectType, "Normal");

            //no httpmeta & usermeta
            meta = new ObjectMetadata();
            meta.HttpMetadata.Clear();
            HttpWebRequest req = WebRequest.Create("http://www.endpoint/") as HttpWebRequest;
            meta.Populate(req);

            //content-length ContentType null 
            meta = new ObjectMetadata();
            meta.HttpMetadata.Remove(HttpHeaders.ContentLength);
            meta.ContentMd5 = "data";
            meta.UserMetadata["user"] = "user";
            req = WebRequest.Create("http://www.endpoint/") as HttpWebRequest;
            meta.Populate(req);

            //ContentLength  = 0 ContentType null 
            meta = new ObjectMetadata();
            meta.ContentLength = 0;
            meta.ContentMd5 = "data";
            meta.UserMetadata["user"] = "user";
            req = WebRequest.Create("http://www.endpoint/") as HttpWebRequest;
            meta.Populate(req);

            //ContentLength  > 0 ContentType not null 
            meta = new ObjectMetadata();
            meta.ContentLength = 10;
            meta.ContentType = "text/xml";
            meta.ContentMd5 = "data";
            meta.UserMetadata["user"] = "user";
            req = WebRequest.Create("http://www.endpoint/") as HttpWebRequest;
            meta.Populate(req);
        }

        [Test]
        public void OssObjectTest()
        {
            var obj = new OssObject();
            obj.Key = "key";

            obj.BucketName = null;
            var str = obj.ToString();
            Assert.AreNotEqual(str.IndexOf("targetBucket="), -1);

            obj.BucketName = "bucket";
            str = obj.ToString();
            Assert.AreNotEqual(str.IndexOf("targetBucket=bucket"), -1);
        }

        [Test]
        public void OwnerTest()
        {
            var obj = new Owner();

            var str = obj.ToString();
            Assert.AreNotEqual(str.IndexOf("Owner Id=,"), -1);
            Assert.AreNotEqual(str.IndexOf("DisplayName=]"), -1);

            obj.Id = "Id";
            obj.DisplayName = "DisplayName";
            str = obj.ToString();
            Assert.AreNotEqual(str.IndexOf("Owner Id=Id"), -1);
            Assert.AreNotEqual(str.IndexOf("DisplayName=DisplayName"), -1);
        }

        [Test]
        public void PolicyConditionsTest()
        {
            var policy = new PolicyConditions();

            var str = policy.Jsonize();
            Assert.AreNotEqual(str.IndexOf("conditions"), -1);

            //AddConditionItem
            policy.AddConditionItem(MatchMode.Exact, PolicyConditions.CondCacheControl, "data");
            policy.AddConditionItem(MatchMode.StartWith, PolicyConditions.CondContentEncoding, "data");

            policy.AddConditionItem("name", 10, 100);
            policy.AddConditionItem("eq-name", "value");
            Assert.IsTrue(true);

            str = policy.Jsonize();
            Assert.AreNotEqual(str.IndexOf("conditions"), -1);

            try
            {
                policy.AddConditionItem("name", 100, 10);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }


            try
            {
                policy.AddConditionItem(MatchMode.Range, PolicyConditions.CondContentLengthRange, "data");
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void ResponseHeaderOverridesTest()
        {
            var respons = new ResponseHeaderOverrides();
            var parameters = new Dictionary<string, string>();

            respons.Populate(parameters);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseCacheControl), false);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseContentDisposition), false);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseContentEncoding), false);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseHeaderContentLanguage), false);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseHeaderContentType), false);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseHeaderExpires), false);

            respons.CacheControl = "Control";
            respons.ContentDisposition = "Disposition";
            respons.ContentEncoding = "Encoding";
            respons.ContentLanguage = "Language";
            respons.ContentType = "Type";
            respons.Expires = "Expires";
            respons.Populate(parameters);
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseCacheControl], "Control");
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseContentDisposition], "Disposition");
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseContentEncoding], "Encoding");
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseHeaderContentLanguage], "Language");
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseHeaderContentType], "Type");
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseHeaderExpires], "Expires");
        }

        [Test]
        public void ResumableContextTest()
        {
            var bucketMd5 = FileUtils.ComputeContentMd5(new MemoryStream(Encoding.UTF8.GetBytes("bucket"))).ToUpperInvariant();
            var keyMd5 = FileUtils.ComputeContentMd5(new MemoryStream(Encoding.UTF8.GetBytes("key"))).ToUpperInvariant();
            var ckPath = "checkpointDir" + Path.DirectorySeparatorChar + bucketMd5 + "_" + keyMd5;

            var ctx = new ResumableContext("bucket", "key", "checkpointDir");
            Assert.AreEqual(ctx.CheckpointFile, ckPath);
            ctx.Clear();
            ctx.Load();
            ctx.Dump();

            ctx = new ResumableContext("bucket", "key", "");
            ctx.Clear();
            ctx.Load();
            ctx.Dump();

            Assert.AreEqual(ctx.FromString(""), false);
            Assert.AreEqual(ctx.FromString("id:md5:crc:"), false);

            Assert.AreEqual(ctx.ToString(), "");
            ctx.UploadId = "upload-id";
            ctx.PartContextList = new List<ResumablePartContext>();
            Assert.AreEqual(ctx.ToString(), "");

            ctx.PartContextList.Add(new ResumablePartContext());
            Assert.AreEqual(ctx.ToString(), "upload-id:md5:crc:0_0_0_False___0");


            try
            {
                string checkdir = "";
                for (int i = 0; i < 256; i++)
                    checkdir += "a";

                ctx = new ResumableContext("bucket", "key", checkdir);
                var path = ctx.CheckpointFile;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void ResumableDownloadContextTest()
        {
            var bucketMd5 = FileUtils.ComputeContentMd5(new MemoryStream(Encoding.UTF8.GetBytes("bucket"))).ToUpperInvariant();
            var keyMd5 = FileUtils.ComputeContentMd5(new MemoryStream(Encoding.UTF8.GetBytes("key"))).ToUpperInvariant();
            var key_versionMd5 = FileUtils.ComputeContentMd5(new MemoryStream(Encoding.UTF8.GetBytes("key?versionId=version"))).ToUpperInvariant();
            var ckPath = "checkpointDir" + Path.DirectorySeparatorChar + "Download_" + bucketMd5 + "_" + key_versionMd5;
            var ctx = new ResumableDownloadContext("bucket", "key", "version", "checkpointDir");
            Assert.AreEqual(ctx.BucketName, "bucket");
            Assert.AreEqual(ctx.Key, "key");
            Assert.AreEqual(ctx.VersionId, "version");
            Assert.AreEqual(ctx.CheckpointDir, "checkpointDir");
            Assert.AreEqual(ctx.CheckpointFile, ckPath);
            ctx.Clear();
            ctx.Load();

            ckPath = "checkpointDir" + Path.DirectorySeparatorChar + "Download_" + bucketMd5 + "_" + keyMd5;
            ctx = new ResumableDownloadContext("bucket", "key", null, "checkpointDir");
            Assert.AreEqual(ctx.BucketName, "bucket");
            Assert.AreEqual(ctx.Key, "key");
            Assert.AreEqual(ctx.VersionId, null);
            Assert.AreEqual(ctx.CheckpointDir, "checkpointDir");
            Assert.AreEqual(ctx.CheckpointFile, ckPath);
            ctx.Clear();
            ctx.Load();

            ctx = new ResumableDownloadContext("bucket", "key", null, "");
            Assert.AreEqual(ctx.BucketName, "bucket");
            Assert.AreEqual(ctx.Key, "key");
            Assert.AreEqual(ctx.VersionId, null);
            Assert.AreEqual(ctx.CheckpointDir, null);
            ctx.Clear();
            ctx.Load();

            Assert.AreEqual(ctx.FromString(""), false);
            Assert.AreEqual(ctx.FromString(":::"), false);
            Assert.AreEqual(ctx.FromString("etag:MD5:1234:1,2,3"), false);

            ctx.PartContextList = new List<ResumablePartContext>();
            Assert.AreEqual(ctx.FromString("etag:MD5:1234:1_2_3_True___0,2_2_3_True___0"), true);
        }

        [Test]
        public void ResumableCopyContextTest()
        {
            var srcMd5 = FileUtils.ComputeContentMd5(new MemoryStream(Encoding.UTF8.GetBytes("bucket-key"))).ToUpperInvariant();
            var src_versionMd5 = FileUtils.ComputeContentMd5(new MemoryStream(Encoding.UTF8.GetBytes("bucket-key?versionId=version"))).ToUpperInvariant();
            var dstMd5 = FileUtils.ComputeContentMd5(new MemoryStream(Encoding.UTF8.GetBytes("dst-bucket-dst-key"))).ToUpperInvariant();
            var ckPath = "checkpointDir" + Path.DirectorySeparatorChar + "Copy_" + src_versionMd5 + "_" + dstMd5;
            var ctx = new ResumableCopyContext("bucket", "key", "version", "dst-bucket", "dst-key", "checkpointDir");
            Assert.AreEqual(ctx.BucketName, "dst-bucket");
            Assert.AreEqual(ctx.Key, "dst-key");
            Assert.AreEqual(ctx.SourceBucketName, "bucket");
            Assert.AreEqual(ctx.SourceKey, "key");
            Assert.AreEqual(ctx.SourceVersionId, "version");
            Assert.AreEqual(ctx.CheckpointDir, "checkpointDir");
            Assert.AreEqual(ctx.CheckpointFile, ckPath);
            ctx.Clear();
            ctx.Load();

            ckPath = "checkpointDir" + Path.DirectorySeparatorChar + "Copy_" + srcMd5 + "_" + dstMd5;
            ctx = new ResumableCopyContext("bucket", "key", null, "dst-bucket", "dst-key", "checkpointDir");
            Assert.AreEqual(ctx.BucketName, "dst-bucket");
            Assert.AreEqual(ctx.Key, "dst-key");
            Assert.AreEqual(ctx.SourceBucketName, "bucket");
            Assert.AreEqual(ctx.SourceKey, "key");
            Assert.AreEqual(ctx.SourceVersionId, null);
            Assert.AreEqual(ctx.CheckpointDir, "checkpointDir");
            Assert.AreEqual(ctx.CheckpointFile, ckPath);
            ctx.Clear();
            ctx.Load();

            ctx = new ResumableCopyContext("bucket", "key", null, "dst-bucket", "dst-key", null);
            Assert.AreEqual(ctx.BucketName, "dst-bucket");
            Assert.AreEqual(ctx.Key, "dst-key");
            Assert.AreEqual(ctx.SourceBucketName, "bucket");
            Assert.AreEqual(ctx.SourceKey, "key");
            Assert.AreEqual(ctx.SourceVersionId, null);
            Assert.AreEqual(ctx.CheckpointDir, null);
        }

        [Test]
        public void ResumablePartContextTest()
        {
            var ctx = new ResumablePartContext();

            Assert.AreEqual(ctx.FromString(""), false);
            Assert.AreEqual(ctx.FromString("1_2"), false);
            Assert.AreEqual(ctx.FromString("-1_2_3_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("a_2_3_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_-2_3_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_a_3_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_2_-3_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_2_a_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_-4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_a_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_4___7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_4_-1__7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_4_a__7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_4_5__7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_4_5_7_a"), false);


            ctx = new ResumablePartContext();
            ctx.PartId = 1;
            ctx.Position = 2;
            ctx.Length = 3;
            ctx.IsCompleted = true;
            ctx.Crc64 = 0;
            Assert.AreEqual(ctx.ToString(), "1_2_3_True___0");
        }

        [Test]
        public void LifecycleRuleTest()
        {
            var rule1 = new LifecycleRule();
            var rule2 = new LifecycleRule();
            var rule1f = rule1;

            Assert.AreEqual(rule1.Equals(rule1f), true);
            Assert.AreEqual(rule1.Equals(null), false);

            rule1.ID = "RuleID";
            rule1.Prefix = "test";
            rule2.ID = "RuleID";
            rule2.Prefix = "test";
            Assert.AreEqual(rule1.Equals(rule2), true);

            rule1.ID = "RuleID";
            rule1.Prefix = "test";
            rule2.ID = "RuleID1";
            rule2.Prefix = "test";
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule1.ID = "RuleID";
            rule1.Prefix = "test";
            rule2.ID = "RuleID";
            rule2.Prefix = "test1";
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule1.ID = "RuleID";
            rule1.Prefix = "test";
            rule2.ID = "RuleID";
            rule2.Prefix = "test";
            rule1.ExpriationDays = 200;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.ExpriationDays = rule1.ExpriationDays;
            rule1.ExpirationTime = DateTime.UtcNow;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.ExpirationTime = rule1.ExpirationTime;
            rule1.CreatedBeforeDate = DateTime.UtcNow;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.CreatedBeforeDate = rule1.CreatedBeforeDate;
            rule1.Status = RuleStatus.Enabled;
            rule2.Status = RuleStatus.Disabled;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.Status = rule1.Status;
            rule1.AbortMultipartUpload = null;
            rule2.AbortMultipartUpload = new LifecycleRule.LifeCycleExpiration();
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.Status = rule1.Status;
            rule1.AbortMultipartUpload = new LifecycleRule.LifeCycleExpiration();
            rule1.AbortMultipartUpload.Days = 20;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.AbortMultipartUpload = rule1.AbortMultipartUpload;
            rule1.Transitions = null;
            rule2.Transitions = new LifecycleRule.LifeCycleTransition[2];
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule1.Transitions = new LifecycleRule.LifeCycleTransition[2];
            rule2.Transitions = null;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule1.Transitions = new LifecycleRule.LifeCycleTransition[2];
            rule2.Transitions = new LifecycleRule.LifeCycleTransition[1];
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule1.Transitions = new LifecycleRule.LifeCycleTransition[2]
            {
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.IA
                },
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.Archive
                }
            };
            rule2.Transitions = new LifecycleRule.LifeCycleTransition[2]
            {
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.Archive
                },
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.Archive
                }
            };
            Assert.AreEqual(rule1.Equals(rule2), false);


            //LifeCycleExpiration
            var expiration1 = new LifecycleRule.LifeCycleExpiration();
            var expiration2 = new LifecycleRule.LifeCycleExpiration();

            Assert.AreEqual(expiration1.Equals(expiration1), true);
            Assert.AreEqual(expiration1.Equals(expiration2), true);
            Assert.AreEqual(expiration1.Equals(null), false);

            expiration1.Days = 5;
            expiration2.Days = 10;
            Assert.AreEqual(expiration1.Equals(expiration2), false);

            expiration1.Days = 10;
            expiration2.Days = 10;
            expiration1.CreatedBeforeDate = DateTime.UtcNow;
            Assert.AreEqual(expiration1.Equals(expiration2), false);

            //LifeCycleTransition
            var transition1 = new LifecycleRule.LifeCycleTransition();
            var transition2 = new LifecycleRule.LifeCycleTransition();

            Assert.AreEqual(transition1.Equals(null), false);
            Assert.AreEqual(transition1.Equals(transition2), true);

            transition1.StorageClass = StorageClass.Archive;
            transition2.StorageClass = StorageClass.Standard;
            Assert.AreEqual(transition1.Equals(transition2), false);

            //Tags
            LifecycleRule rulet1 = new LifecycleRule();
            rulet1.ID = "StandardExpireRule";
            rulet1.Prefix = "test";
            rulet1.Status = RuleStatus.Enabled;
            rulet1.ExpriationDays = 200;

            LifecycleRule rulet2 = new LifecycleRule();
            rulet2.ID = "StandardExpireRule";
            rulet2.Prefix = "test";
            rulet2.Status = RuleStatus.Enabled;
            rulet2.ExpriationDays = 200;
            rulet2.Tags = new Tag[1]
            {
                new Tag(){
                    Key = "key1",
                    Value = "value1"
                }
            };
            Assert.AreNotEqual(rulet1, rulet2);
            Assert.AreNotEqual(rulet2, rulet1);

            rulet1.Tags = new Tag[1]
            {
                new Tag(){
                    Key = "key1",
                    Value = "value1"
                }
            };
            rulet2.Tags = new Tag[1]
            {
                new Tag(){
                    Key = "key2",
                    Value = "value2"
                }
            };
            Assert.AreNotEqual(rulet1, rulet2);

            rulet2.Tags = new Tag[2]
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
            Assert.AreNotEqual(rulet1, rulet2);

            rulet2.Tags = rulet1.Tags;
            Assert.AreEqual(rulet1, rulet2);

            //ExpiredObjectDeleteMarker
            LifecycleRule rulev1 = new LifecycleRule();
            rulev1.ID = "StandardExpireRule";
            rulev1.Prefix = "test";
            rulev1.Status = RuleStatus.Enabled;
            rulev1.ExpiredObjectDeleteMarker = false;

            LifecycleRule rulev2 = new LifecycleRule();
            rulev2.ID = "StandardExpireRule";
            rulev2.Prefix = "test";
            rulev2.Status = RuleStatus.Enabled;
            rulev2.ExpiredObjectDeleteMarker = false;
            Assert.AreEqual(rulev1, rulev2);

            rulev2.ExpiredObjectDeleteMarker = true;
            Assert.AreNotEqual(rulev1, rulev2);

            rulev2.ExpiredObjectDeleteMarker = null;
            Assert.AreNotEqual(rulev1, rulev2);

            //NoncurrentVersionExpiration
            LifecycleRule ruleve1 = new LifecycleRule();
            ruleve1.ID = "StandardExpireRule";
            ruleve1.Prefix = "test";
            ruleve1.Status = RuleStatus.Enabled;
            ruleve1.NoncurrentVersionExpiration = new LifecycleRule.LifeCycleNoncurrentVersionExpiration()
            {
                NoncurrentDays = 100
            };

            LifecycleRule ruleve2 = new LifecycleRule();
            ruleve2.ID = "StandardExpireRule";
            ruleve2.Prefix = "test";
            ruleve2.Status = RuleStatus.Enabled;
            ruleve2.NoncurrentVersionExpiration = new LifecycleRule.LifeCycleNoncurrentVersionExpiration()
            {
                NoncurrentDays = 100
            };
            Assert.AreEqual(ruleve1, ruleve2);

            ruleve2.NoncurrentVersionExpiration.NoncurrentDays = 200;
            Assert.AreNotEqual(ruleve1, ruleve2);

            ruleve2.NoncurrentVersionExpiration = null;
            Assert.AreNotEqual(ruleve1, ruleve2);

            ruleve2.NoncurrentVersionExpiration = ruleve1.NoncurrentVersionExpiration;
            Assert.AreEqual(ruleve1, ruleve2);

            ruleve1.NoncurrentVersionExpiration = null;
            Assert.AreNotEqual(ruleve1, ruleve2);

            //NoncurrentVersionTransitions
            LifecycleRule rulevt1 = new LifecycleRule();
            rulevt1.ID = "StandardExpireRule";
            rulevt1.Prefix = "test";
            rulevt1.Status = RuleStatus.Enabled;
            rulevt1.NoncurrentVersionTransitions = new LifecycleRule.LifeCycleNoncurrentVersionTransition[2]
            {
                new LifecycleRule.LifeCycleNoncurrentVersionTransition(){
                    StorageClass = StorageClass.IA,
                    NoncurrentDays = 90
                },
                new LifecycleRule.LifeCycleNoncurrentVersionTransition(){
                    StorageClass = StorageClass.Archive,
                    NoncurrentDays = 180
                }
            };

            LifecycleRule rulevt2 = new LifecycleRule();
            rulevt2.ID = "StandardExpireRule";
            rulevt2.Prefix = "test";
            rulevt2.Status = RuleStatus.Enabled;
            rulevt2.NoncurrentVersionTransitions = new LifecycleRule.LifeCycleNoncurrentVersionTransition[2]
            {
                new LifecycleRule.LifeCycleNoncurrentVersionTransition(){
                    StorageClass = StorageClass.IA,
                    NoncurrentDays = 90
                },
                new LifecycleRule.LifeCycleNoncurrentVersionTransition(){
                    StorageClass = StorageClass.Archive,
                    NoncurrentDays = 180
                }
            };
            Assert.AreEqual(rulevt1, rulevt2);

            rulevt2.NoncurrentVersionTransitions[1].NoncurrentDays = 190;
            Assert.AreNotEqual(ruleve1, rulevt2);

            ruleve2.NoncurrentVersionTransitions = new LifecycleRule.LifeCycleNoncurrentVersionTransition[1]
            {
                new LifecycleRule.LifeCycleNoncurrentVersionTransition(){
                    StorageClass = StorageClass.IA,
                    NoncurrentDays = 90
                }
            };
            Assert.AreNotEqual(rulevt1, rulevt2);

            rulevt2.NoncurrentVersionTransitions = null;
            Assert.AreNotEqual(rulevt1, rulevt2);

            rulevt2.NoncurrentVersionTransitions = rulevt1.NoncurrentVersionTransitions;
            Assert.AreEqual(rulevt1, rulevt2);

            rulevt1.NoncurrentVersionTransitions = null;
            Assert.AreNotEqual(rulevt1, rulevt2);
        }

        [Test]
        public void OssClientTest()
        {
            try
            {
                Uri uri = null;
                ICredentialsProvider credsProvider = null;
                var client = new OssClient(uri, credsProvider, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            try
            {
                ICredentialsProvider credsProvider = null;
                var client = new OssClient(new Uri("rtmp://endpoint"), credsProvider, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            try
            {
                ICredentialsProvider credsProvider = null;
                var client = new OssClient(new Uri("http://endpoint"), credsProvider, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            var client1 = new OssClient("endpoint", "ak", "sk");
            var credentials = new DefaultCredentials("ak", "sk", null);
            client1.SwitchCredentials(credentials);
            client1.SetEndpoint(new Uri("https://endpoint"));

            try
            {
                client1.SwitchCredentials(null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            Common.ClientConfiguration conf = null;
            client1 = new OssClient(new Uri("http://endpoint"), "ak", "sk", conf);
            client1.SwitchCredentials(credentials);
        }

        [Test]
        public void ListObjectsRequestTest()
        {
            var request = new ListObjectsRequest("bucket");

            //perfix 
            try
            {
                request.Prefix = null;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                string str = "a";
                for (int i = 0; i < 1024; i++)
                    str += "b";
                request.Prefix = str;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            //marker
            try
            {
                request.Marker = null;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                string str = "a";
                for (int i = 0; i < 1024; i++)
                    str += "b";
                request.Marker = str;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            //MaxKeys
            try
            {
                request.MaxKeys = 1024;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            //Delimiter
            try
            {
                request.Delimiter = null;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                string str = "a";
                for (int i = 0; i < 1024; i++)
                    str += "b";
                request.Delimiter = str;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            request.EncodingType = null;
            Assert.AreEqual(request.EncodingType, HttpUtils.UrlEncodingType);
            request.EncodingType = "type";
            Assert.AreEqual(request.EncodingType, "type");
        }

        [Test]
        public void AccessControlListTest()
        {
            var acl = new AccessControlList();

            try
            {
                acl.GrantPermission(null, Permission.FullControl);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                acl.RevokeAllPermissions(null);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            var grantee1 = GroupGrantee.AllUsers;
            var grantee2 = GroupGrantee.AllUsers;
            Assert.AreEqual(grantee1, grantee2);
            acl.GrantPermission(grantee1, Permission.FullControl);

            try
            {
                acl.RevokeAllPermissions(grantee2);
            }
            catch
            {
            }
        }

        [Test]
        public void DeserializerFactoryTest()
        {
            var factory = DeserializerFactory.GetFactory("text/json");
            Assert.AreEqual(factory, null);

            factory = DeserializerFactory.GetFactory("text/xml");
            Assert.AreNotEqual(factory.CreateUploadPartResultDeserializer(1), null);
        }

        internal class ResponseMock : ServiceResponse
        {
            public IDictionary<string, string> _headers;
            public HttpStatusCode _statusCode;
            public Stream _stream;

            public override HttpStatusCode StatusCode
            {
                get { return _statusCode; }
            }

            public override Exception Failure
            {
                get { return null; }
            }

            public override IDictionary<string, string> Headers
            {
                get
                {
                    return _headers;
                }
            }

            public override Stream Content
            {
                get
                {
                    return _stream;
                }
            }

            public ResponseMock()
            {
            }

            public ResponseMock(HttpStatusCode code, IDictionary<string, string> Headers, Stream stream)
            {
                _statusCode = code;
                _headers = Headers;
                _stream = stream;
            }

            private static IDictionary<string, string> GetResponseHeaders(HttpWebResponse response)
            {
                var headers = response.Headers;
                var result = new Dictionary<string, string>(headers.Count);

                for (var i = 0; i < headers.Count; i++)
                {
                    var key = headers.Keys[i];
                    var value = headers.Get(key);
                    result.Add(key, HttpUtils.Reencode(value, HttpUtils.Iso88591Charset, HttpUtils.Utf8Charset));
                }

                return result;
            }
        }

        internal class ServiceClientMock : ServiceClient
        {
            public ServiceClientMock(ClientConfiguration configuration)
            : base(configuration)
            {
            }

            protected override IAsyncResult BeginSendCore(ServiceRequest request, ExecutionContext context, AsyncCallback callback, object state)
            {
                throw new NotImplementedException();
            }

            protected override ServiceResponse SendCore(ServiceRequest request, ExecutionContext context)
            {
                throw new NotImplementedException();
            }

            protected override Task<ServiceResponse> SendCoreAsync(ServiceRequest request, Common.Communication.ExecutionContext context, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }


        [Test]
        public void AppendObjectResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateAppendObjectResultDeserializer();
            var headers = new Dictionary<string, string>();
            var content = new MemoryStream();
            //empty
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void GetAclResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetAclResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <AccessControlPolicy>
                    <Owner>
                        <ID>0022012****</ID>
                        <DisplayName>user_example</DisplayName>
                    </Owner>
                    <AccessControlList>
                        <Grant>default</Grant>
                    </AccessControlList>
                </AccessControlPolicy>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            //empty
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.ACL, CannedAccessControlList.Default);

            data =
                @" 
                <AccessControlPolicy>
                    <Owner>
                        <ID>0022012****</ID>
                        <DisplayName>user_example</DisplayName>
                    </Owner>
                    <AccessControlList>
                        <Grant>unknown</Grant>
                    </AccessControlList>
                </AccessControlPolicy>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.ACL, CannedAccessControlList.Private);
        }

        [Test]
        public void GetBucketLifecycleDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetBucketLifecycleDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <LifecycleConfiguration>
                  <Rule>
                    <ID>delete after one day</ID>
                    <Prefix>logs/</Prefix>
                    <Status>Enabled</Status>
                    <Expiration>
                      <Date>2017-01-01T00:00:00.000Z</Date>
                    </Expiration>
                  </Rule>
                </LifecycleConfiguration>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Count, 1);

            data =
                @" 
                <LifecycleConfiguration>
                  <Rule>
                    <ID>delete after one day</ID>
                    <Prefix>logs/</Prefix>
                    <Status>Enabled</Status>
                    <Expiration>
                    </Expiration>
                  </Rule>
                </LifecycleConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);

            data =
                @" 
                <LifecycleConfiguration>
                  <Rule>
                    <ID>delete after one day</ID>
                    <Prefix>logs/</Prefix>
                    <Status>Unknown</Status>
                    <Expiration>
                      <Days>1</Days>
                    </Expiration>
                  </Rule>
                </LifecycleConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (InvalidEnumArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void GetCorsResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetCorsResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <CORSConfiguration>
                    <CORSRule>
                      <AllowedOrigin></AllowedOrigin>
                      <AllowedMethod></AllowedMethod>
                      <AllowedHeader>*</AllowedHeader>
                      <ExposeHeader>x-oss-test</ExposeHeader>
                      <MaxAgeSeconds>100</MaxAgeSeconds>
                    </CORSRule>
                </CORSConfiguration>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);

            data =
                @" 
                <CORSConfiguration>
                    <CORSRule>
                      <AllowedHeader>*</AllowedHeader>
                      <ExposeHeader>x-oss-test</ExposeHeader>
                      <MaxAgeSeconds>100</MaxAgeSeconds>
                    </CORSRule>
                </CORSConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void GetObjectResponseDeserializerTest()
        {
            var clientService = new ServiceClientMock(new ClientConfiguration());
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new GetObjectRequest("bucket", "key");
            var deserializer = factory.CreateGetObjectResultDeserializer(request, new RetryableServiceClient(clientService));
            var headers = new Dictionary<string, string>();
            headers.Add(HttpHeaders.HashCrc64Ecma, "abcdef");
            string data = "";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void GetSymlinkResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = new GetSymlinkResultDeserializer();// factory.sy();
            var headers = new Dictionary<string, string>();
            string data = "";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                var result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (Common.OssException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void ListMultipartUploadsResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateListMultipartUploadsResultDeserializer();// factory.sy();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <ListMultipartUploadsResult>
                    <Bucket>oss-example</Bucket>
                    <KeyMarker></KeyMarker>
                    <UploadIdMarker></UploadIdMarker>
                    <NextKeyMarker>oss.avi</NextKeyMarker>
                    <NextUploadIdMarker>0004B99B8E707874FC2D692FA5D77D3F</NextUploadIdMarker>
                    <Delimiter></Delimiter>
                    <Prefix></Prefix>
                    <MaxUploads>1000</MaxUploads>
                    <IsTruncated>false</IsTruncated>
                    <CommonPrefixes>
                        <Prefix>multipart.data</Prefix>
                    </CommonPrefixes>
                </ListMultipartUploadsResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);

            data =
                @" 
                <ListMultipartUploadsResult>
                    <Bucket>oss-example</Bucket>
                    <KeyMarker></KeyMarker>
                    <UploadIdMarker></UploadIdMarker>
                    <NextKeyMarker>oss.avi</NextKeyMarker>
                    <NextUploadIdMarker>0004B99B8E707874FC2D692FA5D77D3F</NextUploadIdMarker>
                    <Delimiter></Delimiter>
                    <Prefix></Prefix>
                    <MaxUploads>1000</MaxUploads>
                    <IsTruncated>false</IsTruncated>
                    <CommonPrefixes>
                    </CommonPrefixes>
                </ListMultipartUploadsResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void ListObjectsResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateListObjectsResultDeserializer(); ;
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <ListBucketResult>
                    <Name>oss-example</Name>
                    <Prefix></Prefix>
                    <Marker></Marker>
                    <MaxKeys>100</MaxKeys>
                    <Delimiter></Delimiter>
                    <IsTruncated>false</IsTruncated>
                    <Contents>
                          <Key>fun/movie/001.avi</Key>
                          <LastModified>2012-02-24T08:43:07.000Z</LastModified>
                          <Type>Normal</Type>
                          <Size>344606</Size>
                          <StorageClass>Standard</StorageClass>
                          <Owner>
                              <ID>0022012****</ID>
                              <DisplayName>user-example</DisplayName>
                          </Owner>
                    </Contents>
                    <Contents>
                          <Key>fun/movie/007.avi</Key>
                          <LastModified>2012-02-24T08:43:27.000Z</LastModified>
                          <ETag>5B3C1A2E053D763E1B002CC607C5A0FE1****</ETag>
                          <Type>Normal</Type>
                          <Size>344606</Size>
                          <StorageClass>Standard</StorageClass>
                          <Owner>
                          </Owner>
                    </Contents>
                    <Contents>
                          <Key>fun/movie/007.avi</Key>
                          <LastModified>2012-02-24T08:43:27.000Z</LastModified>
                          <ETag>5B3C1A2E053D763E1B002CC607C5A0FE1****</ETag>
                          <Type>Normal</Type>
                          <Size>344606</Size>
                          <StorageClass>Standard</StorageClass>
                    </Contents>
                    <CommonPrefixes>
                    </CommonPrefixes>
                </ListBucketResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void ListPartsResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateListPartsResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <ListPartsResult>
                    <Bucket>multipart_upload</Bucket>
                    <Key>multipart.data</Key>
                    <UploadId>0004B999EF5A239BB9138C6227D69F95</UploadId>
                    <NextPartNumberMarker></NextPartNumberMarker>
                    <MaxParts>1000</MaxParts>
                    <IsTruncated>false</IsTruncated>
                    <Part>
                        <PartNumber>1</PartNumber>
                        <LastModified>2012-02-23T07:01:34.000Z</LastModified>
                        <Size>6291456</Size>
                    </Part>
                </ListPartsResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);

            data =
                @" 
                <ListPartsResult>
                    <Bucket>multipart_upload</Bucket>
                    <Key>multipart.data</Key>
                    <UploadId>0004B999EF5A239BB9138C6227D69F95</UploadId>
                    <NextPartNumberMarker></NextPartNumberMarker>
                    <MaxParts>1000</MaxParts>
                    <IsTruncated>false</IsTruncated>
                    <EncodingType></EncodingType>
                </ListPartsResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void PutObjectResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new PutObjectRequest("bucket", "key", new MemoryStream());
            var deserializer = factory.CreatePutObjectReusltDeserializer(request);
            var headers = new Dictionary<string, string>();
            string data = "";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            //empty
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void UploadPartCopyResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new PutObjectRequest("bucket", "key", new MemoryStream());
            var deserializer = factory.CreateUploadPartCopyResultDeserializer(1);
            var headers = new Dictionary<string, string>();
            headers.Add(HttpHeaders.QuotaDeltaSize, "100");
            string data =
                @" 
                <CopyPartResult>
                    <LastModified>2014-07-17T06:27:54.000Z </LastModified>
                    <ETag>5B3C1A2E053D763E1B002CC607C5****</ETag>
                </CopyPartResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);


            //invalid xml
            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);

            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void UploadPartResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateUploadPartResultDeserializer(1);
            var headers = new Dictionary<string, string>();
            string data = "";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void GetBucketTaggingResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new PutObjectRequest("bucket", "key", new MemoryStream());
            var deserializer = factory.CreateGetBucketTaggingResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <Tagging>
                    <TagSet>
                        <Tag>
                            <Key>tag2</Key>
                            <Value>jsmith</Value>
                        </Tag>
                    </TagSet>
                </Tagging>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 1);
            Assert.AreEqual(result.Tags[0].Key, "tag2");
            Assert.AreEqual(result.Tags[0].Value, "jsmith");

            data =
             @" 
                <Tagging>
                    <TagSet>
                        <Tag>
                            <Key>tag2</Key>
                        </Tag>
                    </TagSet>
                </Tagging>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 1);
            Assert.AreEqual(result.Tags[0].Key, "tag2");
            Assert.AreEqual(result.Tags[0].Value, null);

            data =
                @" 
                <Tagging>
                    <TagSet>
                    </TagSet>
                </Tagging>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 0);

            data =
                @" 
                <Tagging>
                </Tagging>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 0);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void GetObjectTaggingResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new PutObjectRequest("bucket", "key", new MemoryStream());
            var deserializer = factory.CreateGetObjectTaggingResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <Tagging>
                    <TagSet>
                        <Tag>
                            <Key>tag2</Key>
                            <Value>jsmith</Value>
                        </Tag>
                    </TagSet>
                </Tagging>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 1);
            Assert.AreEqual(result.Tags[0].Key, "tag2");
            Assert.AreEqual(result.Tags[0].Value, "jsmith");

            data =
             @" 
                <Tagging>
                    <TagSet>
                        <Tag>
                            <Key>tag2</Key>
                        </Tag>
                    </TagSet>
                </Tagging>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 1);
            Assert.AreEqual(result.Tags[0].Key, "tag2");
            Assert.AreEqual(result.Tags[0].Value, null);

            data =
                @" 
                <Tagging>
                    <TagSet>
                    </TagSet>
                </Tagging>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 0);

            data =
                @" 
                <Tagging>
                </Tagging>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 0);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void CreateGetBucketVersioningResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new PutObjectRequest("bucket", "key", new MemoryStream());
            var deserializer = factory.CreateGetBucketVersioningResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <VersioningConfiguration>
                    <Status>Enabled</Status>
                </VersioningConfiguration>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Status, VersioningStatus.Enabled);

            data =
                @" 
                <VersioningConfiguration>
                    <Status>Suspended</Status>
                </VersioningConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Status, VersioningStatus.Suspended);

            data =
                @" 
                <VersioningConfiguration>
                    <Status>Unknown</Status>
                </VersioningConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Status, VersioningStatus.Off);

            data =
                @" 
                <VersioningConfiguration>
                </VersioningConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Status, VersioningStatus.Off);

            data = "<VersioningConfiguration xmlns=\"http://doc.oss-cn-hangzhou.aliyuncs.com\"/>";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Status, VersioningStatus.Off);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void CreateListObjectVersionsResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new PutObjectRequest("bucket", "key", new MemoryStream());
            var deserializer = factory.CreateListObjectVersionsResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <ListVersionsResult>
                    <Name>oss-example</Name>
                    <Prefix></Prefix>
                    <KeyMarker>example</KeyMarker>
                    <VersionIdMarker>CAEQMxiBgICbof2D0BYiIGRhZjgwMzJiMjA3MjQ0ODE5MWYxZDYwMzJlZjU1****</VersionIdMarker>
                    <MaxKeys>200</MaxKeys>
                    <Delimiter></Delimiter>
                    <IsTruncated>false</IsTruncated>
                    <DeleteMarker>
                        <Key>example</Key>
                        <VersionId>CAEQMxiBgICAof2D0BYiIDJhMGE3N2M1YTI1NDQzOGY5NTkyNTI3MGYyMzJm****</VersionId>
                        <IsLatest>false</IsLatest>
                        <LastModified>2019-04-09T07:27:28.000Z</LastModified>
                        <Owner>
                          <ID>1234512528586****</ID>
                          <DisplayName>12345125285864390</DisplayName>
                        </Owner>
                    </DeleteMarker>
                    <Version>
                        <Key>example</Key>
                        <VersionId>CAEQMxiBgMDNoP2D0BYiIDE3MWUxNzgxZDQxNTRiODI5OGYwZGMwNGY3MzZjN****</VersionId>
                        <IsLatest>false</IsLatest>
                        <LastModified>2019-04-09T07:27:28.000Z</LastModified>
                        <ETag>250F8A0AE989679A22926A875F0A2****</ETag>
                        <Type>Normal</Type>
                        <Size>93731</Size>
                        <StorageClass>Standard</StorageClass>
                        <Owner>
                          <ID>1234512528586****</ID>
                          <DisplayName>12345125285864390</DisplayName>
                        </Owner>
                    </Version>
                    <Version>
                        <Key>pic.jpg</Key>
                        <VersionId>CAEQMxiBgMCZov2D0BYiIDY4MDllOTc2YmY5MjQxMzdiOGI3OTlhNTU0ODIx****</VersionId>
                        <IsLatest>true</IsLatest>
                        <LastModified>2019-04-09T07:27:28.000Z</LastModified>
                        <ETag>3663F7B0B9D3153F884C821E7CF4****</ETag>
                        <Type>Normal</Type>
                        <Size>574768</Size>
                        <StorageClass>Standard</StorageClass>
                        <Owner>
                          <ID>1234512528586****</ID>
                          <DisplayName>12345125285864390</DisplayName>
                        </Owner>
                    </Version>
                </ListVersionsResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.BucketName, "oss-example");
            Assert.AreEqual(result.Prefix, "");
            Assert.AreEqual(result.KeyMarker, "example");
            Assert.AreEqual(result.VersionIdMarker, "CAEQMxiBgICbof2D0BYiIGRhZjgwMzJiMjA3MjQ0ODE5MWYxZDYwMzJlZjU1****");
            Assert.AreEqual(result.MaxKeys, 200);
            Assert.AreEqual(result.Delimiter, "");
            Assert.AreEqual(result.IsTruncated, false);
            Assert.AreEqual(result.NextKeyMarker, null);
            Assert.AreEqual(result.NextVersionIdMarker, null);
            var allVersionSum = OssTestUtils.ToArray(result.ObjectVersionSummaries);
            var allDeleteMarkerSum = OssTestUtils.ToArray(result.DeleteMarkerSummaries);
            var allPrefixSum = OssTestUtils.ToArray(result.CommonPrefixes);
            Assert.AreEqual(allVersionSum.Count, 2);
            Assert.AreEqual(allVersionSum[0].BucketName, "oss-example");
            Assert.AreEqual(allVersionSum[0].Key, "example");
            Assert.AreEqual(allVersionSum[0].VersionId, "CAEQMxiBgMDNoP2D0BYiIDE3MWUxNzgxZDQxNTRiODI5OGYwZGMwNGY3MzZjN****");
            Assert.AreEqual(allVersionSum[0].IsLatest, false);
            Assert.AreEqual(allVersionSum[0].LastModified.ToUniversalTime(), DateTime.Parse("2019-04-09T07:27:28.000Z").ToUniversalTime());
            Assert.AreEqual(allVersionSum[0].ETag, "250F8A0AE989679A22926A875F0A2****");
            Assert.AreEqual(allVersionSum[0].Type, "Normal");
            Assert.AreEqual(allVersionSum[0].Size, 93731);
            Assert.AreEqual(allVersionSum[0].StorageClass, StorageClass.Standard.ToString());
            Assert.AreEqual(allVersionSum[0].Owner.DisplayName, "12345125285864390");
            Assert.AreEqual(allVersionSum[0].Owner.Id, "1234512528586****");

            Assert.AreEqual(allVersionSum[1].BucketName, "oss-example");
            Assert.AreEqual(allVersionSum[1].Key, "pic.jpg");
            Assert.AreEqual(allVersionSum[1].VersionId, "CAEQMxiBgMCZov2D0BYiIDY4MDllOTc2YmY5MjQxMzdiOGI3OTlhNTU0ODIx****");
            Assert.AreEqual(allVersionSum[1].IsLatest, true);


            Assert.AreEqual(allDeleteMarkerSum.Count, 1);
            Assert.AreEqual(allDeleteMarkerSum[0].BucketName, "oss-example");
            Assert.AreEqual(allDeleteMarkerSum[0].Key, "example");
            Assert.AreEqual(allDeleteMarkerSum[0].VersionId, "CAEQMxiBgICAof2D0BYiIDJhMGE3N2M1YTI1NDQzOGY5NTkyNTI3MGYyMzJm****");
            Assert.AreEqual(allDeleteMarkerSum[0].IsLatest, false);
            Assert.AreEqual(allDeleteMarkerSum[0].LastModified.ToUniversalTime(), DateTime.Parse("2019-04-09T07:27:28.000Z").ToUniversalTime());

            Assert.AreEqual(allPrefixSum.Count, 0);


            data =
                @" 
                <ListVersionsResult>
                    <Name>oss-example</Name>
                    <Prefix></Prefix>
                    <KeyMarker>example</KeyMarker>
                    <VersionIdMarker>CAEQMxiBgICbof2D0BYiIGRhZjgwMzJiMjA3MjQ0ODE5MWYxZDYwMzJlZjU1****</VersionIdMarker>
                    <MaxKeys>200</MaxKeys>
                    <Delimiter>/</Delimiter>
                    <IsTruncated>false</IsTruncated>
                    <CommonPrefixes>
                        <Prefix>123</Prefix>
                    </CommonPrefixes>
                    <CommonPrefixes>
                        <Prefix>abc</Prefix>
                    </CommonPrefixes>
                </ListVersionsResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.BucketName, "oss-example");
            Assert.AreEqual(result.Prefix, "");
            Assert.AreEqual(result.KeyMarker, "example");
            Assert.AreEqual(result.VersionIdMarker, "CAEQMxiBgICbof2D0BYiIGRhZjgwMzJiMjA3MjQ0ODE5MWYxZDYwMzJlZjU1****");
            Assert.AreEqual(result.MaxKeys, 200);
            Assert.AreEqual(result.Delimiter, "/");
            Assert.AreEqual(result.IsTruncated, false);
            Assert.AreEqual(result.NextKeyMarker, null);
            Assert.AreEqual(result.NextVersionIdMarker, null);
            allVersionSum = OssTestUtils.ToArray(result.ObjectVersionSummaries);
            allDeleteMarkerSum = OssTestUtils.ToArray(result.DeleteMarkerSummaries);
            allPrefixSum = OssTestUtils.ToArray(result.CommonPrefixes);
            Assert.AreEqual(allVersionSum.Count, 0);
            Assert.AreEqual(allDeleteMarkerSum.Count, 0);

            Assert.AreEqual(allPrefixSum.Count, 2);
            Assert.AreEqual(allPrefixSum[0], "123");
            Assert.AreEqual(allPrefixSum[1], "abc");


            data =
                @" 
                <ListVersionsResult>
                    <Name>oss-example</Name>
                    <Prefix></Prefix>
                    <KeyMarker>example</KeyMarker>
                    <VersionIdMarker>CAEQMxiBgICbof2D0BYiIGRhZjgwMzJiMjA3MjQ0ODE5MWYxZDYwMzJlZjU1****</VersionIdMarker>
                    <Delimiter>/</Delimiter>
                    <CommonPrefixes>
                    </CommonPrefixes>
                    <Version>
                        <Key>example</Key>
                        <VersionId>CAEQMxiBgMDNoP2D0BYiIDE3MWUxNzgxZDQxNTRiODI5OGYwZGMwNGY3MzZjN****</VersionId>
                        <IsLatest>false</IsLatest>
                        <LastModified>2019-04-09T07:27:28.000Z</LastModified>
                        <Type>Normal</Type>
                        <Size>93731</Size>
                        <StorageClass>Standard</StorageClass>
                    </Version>
                    <DeleteMarker>
                        <Key>example</Key>
                        <VersionId>CAEQMxiBgICAof2D0BYiIDJhMGE3N2M1YTI1NDQzOGY5NTkyNTI3MGYyMzJm****</VersionId>
                        <IsLatest>false</IsLatest>
                        <LastModified>2019-04-09T07:27:28.000Z</LastModified>
                    </DeleteMarker>
                </ListVersionsResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.BucketName, "oss-example");
            Assert.AreEqual(result.Prefix, "");
            Assert.AreEqual(result.KeyMarker, "example");
            Assert.AreEqual(result.VersionIdMarker, "CAEQMxiBgICbof2D0BYiIGRhZjgwMzJiMjA3MjQ0ODE5MWYxZDYwMzJlZjU1****");
            Assert.AreEqual(result.MaxKeys, 0);
            Assert.AreEqual(result.Delimiter, "/");
            Assert.AreEqual(result.IsTruncated, false);
            Assert.AreEqual(result.NextKeyMarker, null);
            Assert.AreEqual(result.NextVersionIdMarker, null);
            allVersionSum = OssTestUtils.ToArray(result.ObjectVersionSummaries);
            allDeleteMarkerSum = OssTestUtils.ToArray(result.DeleteMarkerSummaries);
            allPrefixSum = OssTestUtils.ToArray(result.CommonPrefixes);
            Assert.AreEqual(allVersionSum.Count, 1);
            Assert.AreEqual(allDeleteMarkerSum.Count, 1);
            Assert.AreEqual(allPrefixSum.Count, 0);
            Assert.AreEqual(allVersionSum[0].ETag, "");
            Assert.AreEqual(allVersionSum[0].Owner, null);

            Assert.AreEqual(allDeleteMarkerSum[0].Owner, null);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void DeleteObjectVersionsResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new PutObjectRequest("bucket", "key", new MemoryStream());
            var deserializer = factory.CreateDeleteObjectVersionsResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <DeleteResult>
                    <Deleted>
                       <Key>multipart.data</Key>
                       <DeleteMarker>true</DeleteMarker>
                       <DeleteMarkerVersionId>CAEQMhiBgIDXiaaB0BYiIGQzYmRkZGUxMTM1ZDRjOTZhNjk4YjRjMTAyZjhl****</DeleteMarkerVersionId>
                    </Deleted>
                    <Deleted>
                       <Key>test.jpg</Key>
                       <DeleteMarker>true</DeleteMarker>
                       <DeleteMarkerVersionId>CAEQMhiBgIDB3aWB0BYiIGUzYTA3YzliMzVmNzRkZGM5NjllYTVlMjYyYWEy****</DeleteMarkerVersionId>
                    </Deleted>
                </DeleteResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            var result = deserializer.Deserialize(xmlStream);
            var deletedObjectSum = OssTestUtils.ToArray(result.DeletedObjectSummaries);
            Assert.AreEqual(result.EncodingType, null);
            Assert.AreEqual(deletedObjectSum.Count, 2);
            Assert.AreEqual(deletedObjectSum[0].Key, "multipart.data");
            Assert.AreEqual(deletedObjectSum[0].VersionId, null);
            Assert.AreEqual(deletedObjectSum[0].DeleteMarker, true);
            Assert.AreEqual(deletedObjectSum[0].DeleteMarkerVersionId, "CAEQMhiBgIDXiaaB0BYiIGQzYmRkZGUxMTM1ZDRjOTZhNjk4YjRjMTAyZjhl****");
            Assert.AreEqual(deletedObjectSum[1].Key, "test.jpg");
            Assert.AreEqual(deletedObjectSum[1].VersionId, null);
            Assert.AreEqual(deletedObjectSum[1].DeleteMarker, true);
            Assert.AreEqual(deletedObjectSum[1].DeleteMarkerVersionId, "CAEQMhiBgIDB3aWB0BYiIGUzYTA3YzliMzVmNzRkZGM5NjllYTVlMjYyYWEy****");

            data =
                @" 
                <DeleteResult>
                    <Deleted>
                       <Key>multipart.data</Key>
                    </Deleted>
                    <Deleted>
                       <Key>test.jpg</Key>
                    </Deleted>
                    <Deleted>
                       <Key>demo.jpg</Key>
                    </Deleted>
                </DeleteResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            result = deserializer.Deserialize(xmlStream);
            deletedObjectSum = OssTestUtils.ToArray(result.DeletedObjectSummaries);
            Assert.AreEqual(deletedObjectSum.Count, 3);
            Assert.AreEqual(deletedObjectSum[0].Key, "multipart.data");
            Assert.AreEqual(deletedObjectSum[0].VersionId, null);
            Assert.AreEqual(deletedObjectSum[0].DeleteMarker, false);
            Assert.AreEqual(deletedObjectSum[0].DeleteMarkerVersionId, null);
            Assert.AreEqual(deletedObjectSum[1].Key, "test.jpg");
            Assert.AreEqual(deletedObjectSum[1].VersionId, null);
            Assert.AreEqual(deletedObjectSum[1].DeleteMarker, false);
            Assert.AreEqual(deletedObjectSum[1].DeleteMarkerVersionId, null);
            Assert.AreEqual(deletedObjectSum[2].Key, "demo.jpg");
            Assert.AreEqual(deletedObjectSum[2].VersionId, null);
            Assert.AreEqual(deletedObjectSum[2].DeleteMarker, false);
            Assert.AreEqual(deletedObjectSum[2].DeleteMarkerVersionId, null);

            data =
                @" 
                <DeleteResult>
                    <Deleted>
                       <Key>multipart.data</Key>
                       <VersionId>CAEQNRiBgIDyz.6C0BYiIGQ2NWEwNmVhNTA3ZTQ3MzM5ODliYjM1ZTdjYjA4****</VersionId>
                    </Deleted>
                </DeleteResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            result = deserializer.Deserialize(xmlStream);
            deletedObjectSum = OssTestUtils.ToArray(result.DeletedObjectSummaries);
            Assert.AreEqual(deletedObjectSum.Count, 1);
            Assert.AreEqual(deletedObjectSum[0].Key, "multipart.data");
            Assert.AreEqual(deletedObjectSum[0].VersionId, "CAEQNRiBgIDyz.6C0BYiIGQ2NWEwNmVhNTA3ZTQ3MzM5ODliYjM1ZTdjYjA4****");
            Assert.AreEqual(deletedObjectSum[0].DeleteMarker, false);
            Assert.AreEqual(deletedObjectSum[0].DeleteMarkerVersionId, null);

            data =
                @" 
                <DeleteResult>
                    <Deleted>
                       <Key>demo.jpg</Key>
                       <VersionId>CAEQNRiBgICEoPiC0BYiIGMxZWJmYmMzYjE0OTQ0ZmZhYjgzNzkzYjc2NjZk****</VersionId>
                       <DeleteMarker>true</DeleteMarker>
                       <DeleteMarkerVersionId>111111</DeleteMarkerVersionId>
                    </Deleted>
                </DeleteResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            result = deserializer.Deserialize(xmlStream);
            deletedObjectSum = OssTestUtils.ToArray(result.DeletedObjectSummaries);
            Assert.AreEqual(deletedObjectSum.Count, 1);
            Assert.AreEqual(deletedObjectSum[0].Key, "demo.jpg");
            Assert.AreEqual(deletedObjectSum[0].VersionId, "CAEQNRiBgICEoPiC0BYiIGMxZWJmYmMzYjE0OTQ0ZmZhYjgzNzkzYjc2NjZk****");
            Assert.AreEqual(deletedObjectSum[0].DeleteMarker, true);
            Assert.AreEqual(deletedObjectSum[0].DeleteMarkerVersionId, "111111");

            data =
                @" 
                <DeleteResult>
                    <EncodingType>url</EncodingType>
                    <Deleted>
                       <Key>demo%2Fjpg</Key>
                       <VersionId>CAEQNRiBgICEoPiC0BYiIGMxZWJmYmMzYjE0OTQ0ZmZhYjgzNzkzYjc2NjZk****</VersionId>
                       <DeleteMarker>true</DeleteMarker>
                       <DeleteMarkerVersionId>111111</DeleteMarkerVersionId>
                    </Deleted>
                </DeleteResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            result = deserializer.Deserialize(xmlStream);
            deletedObjectSum = OssTestUtils.ToArray(result.DeletedObjectSummaries);
            Assert.AreEqual(result.EncodingType, "url");
            Assert.AreEqual(deletedObjectSum.Count, 1);
            Assert.AreEqual(deletedObjectSum[0].Key, "demo/jpg");
            Assert.AreEqual(deletedObjectSum[0].VersionId, "CAEQNRiBgICEoPiC0BYiIGMxZWJmYmMzYjE0OTQ0ZmZhYjgzNzkzYjc2NjZk****");
            Assert.AreEqual(deletedObjectSum[0].DeleteMarker, true);
            Assert.AreEqual(deletedObjectSum[0].DeleteMarkerVersionId, "111111");

            data = "<DeleteResult></DeleteResult>";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            result = deserializer.Deserialize(xmlStream);
            deletedObjectSum = OssTestUtils.ToArray(result.DeletedObjectSummaries);
            Assert.AreEqual(result.EncodingType, null);
            Assert.AreEqual(deletedObjectSum.Count, 0);

            data = "";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            result = deserializer.Deserialize(xmlStream);
            deletedObjectSum = OssTestUtils.ToArray(result.DeletedObjectSummaries);
            Assert.AreEqual(result.EncodingType, null);
            Assert.AreEqual(deletedObjectSum.Count, 0);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void CreateSymlinkResultDeserializerTest()
        {
            var clientService = new ServiceClientMock(new ClientConfiguration());
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new GetObjectRequest("bucket", "key");
            var deserializer = factory.CreateCreateSymlinkResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data = "";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void ListLiveChannelResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateListLiveChannelResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <ListLiveChannelResult>
                    <Prefix></Prefix>
                    <Marker></Marker>
                    <MaxKeys>1</MaxKeys>
                    <IsTruncated>true</IsTruncated>
                    <NextMarker>channel-0</NextMarker>
                    <LiveChannel>
                        <Name>channel-0</Name>
                        <Description></Description>
                        <Status>disabled</Status>
                        <LastModified>2016-07-30T01:54:21.000Z</LastModified>
                        <PublishUrls>
                          <Url>rtmp://test-bucket.oss-cn-hangzhou.aliyuncs.com/live/channel-0</Url>
                        </PublishUrls>
                        <PlayUrls>
                          <Url>http://test-bucket.oss-cn-hangzhou.aliyuncs.com/channel-0/playlist.m3u8</Url>
                        </PlayUrls>
                    </LiveChannel>
                </ListLiveChannelResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Prefix, "");
            Assert.AreEqual(result.Marker, "");
            Assert.AreEqual(result.MaxKeys, 1);
            Assert.AreEqual(result.IsTruncated, true);
            Assert.AreEqual(result.NextMarker, "channel-0");
            var LiveChannels = OssTestUtils.ToArray(result.LiveChannels);
            Assert.AreEqual(LiveChannels.Count, 1);
            Assert.AreEqual(LiveChannels[0].Name, "channel-0");
            Assert.AreEqual(LiveChannels[0].Description, "");
            Assert.AreEqual(LiveChannels[0].Status, "disabled");
            Assert.AreEqual(LiveChannels[0].LastModified, DateUtils.ParseIso8601Date("2016-07-30T01:54:21.000Z"));
            Assert.AreEqual(LiveChannels[0].PublishUrl, "rtmp://test-bucket.oss-cn-hangzhou.aliyuncs.com/live/channel-0");
            Assert.AreEqual(LiveChannels[0].PlayUrl, "http://test-bucket.oss-cn-hangzhou.aliyuncs.com/channel-0/playlist.m3u8");

            data =
                @" 
                <ListLiveChannelResult>
                    <Prefix>channel</Prefix>
                    <Marker>channel</Marker>
                    <MaxKeys>10</MaxKeys>
                    <IsTruncated>false</IsTruncated>
                    <NextMarker>channel-1</NextMarker>
                    <LiveChannel>
                        <Name>channel-0</Name>
                        <Description>discrption</Description>
                        <Status>enable</Status>
                        <LastModified>2016-07-30T01:54:21.000Z</LastModified>
                        <PublishUrls>
                          <Url>rtmp://test-bucket.oss-cn-hangzhou.aliyuncs.com/live/channel-0</Url>
                        </PublishUrls>
                        <PlayUrls>
                          <Url>http://test-bucket.oss-cn-hangzhou.aliyuncs.com/channel-0/playlist.m3u8</Url>
                        </PlayUrls>
                    </LiveChannel>
                    <LiveChannel>
                        <Name>channel-1</Name>
                        <Description></Description>
                        <Status>disabled</Status>
                        <LastModified>2017-07-30T01:54:21.000Z</LastModified>
                        <PublishUrls>
                          <Url>rtmp://test-bucket.oss-cn-hangzhou.aliyuncs.com/live/channel-1</Url>
                        </PublishUrls>
                        <PlayUrls>
                          <Url>http://test-bucket.oss-cn-hangzhou.aliyuncs.com/channel-1/playlist.m3u8</Url>
                        </PlayUrls>
                    </LiveChannel>
                </ListLiveChannelResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Prefix, "channel");
            Assert.AreEqual(result.Marker, "channel");
            Assert.AreEqual(result.MaxKeys, 10);
            Assert.AreEqual(result.IsTruncated, false);
            Assert.AreEqual(result.NextMarker, "channel-1");
            LiveChannels = OssTestUtils.ToArray(result.LiveChannels);
            Assert.AreEqual(LiveChannels.Count, 2);
            Assert.AreEqual(LiveChannels[0].Name, "channel-0");
            Assert.AreEqual(LiveChannels[0].Description, "discrption");
            Assert.AreEqual(LiveChannels[0].Status, "enable");
            Assert.AreEqual(LiveChannels[0].LastModified, DateUtils.ParseIso8601Date("2016-07-30T01:54:21.000Z"));
            Assert.AreEqual(LiveChannels[0].PublishUrl, "rtmp://test-bucket.oss-cn-hangzhou.aliyuncs.com/live/channel-0");
            Assert.AreEqual(LiveChannels[0].PlayUrl, "http://test-bucket.oss-cn-hangzhou.aliyuncs.com/channel-0/playlist.m3u8");

            Assert.AreEqual(LiveChannels[1].Name, "channel-1");
            Assert.AreEqual(LiveChannels[1].Description, "");
            Assert.AreEqual(LiveChannels[1].Status, "disabled");
            Assert.AreEqual(LiveChannels[1].LastModified, DateUtils.ParseIso8601Date("2017-07-30T01:54:21.000Z"));
            Assert.AreEqual(LiveChannels[1].PublishUrl, "rtmp://test-bucket.oss-cn-hangzhou.aliyuncs.com/live/channel-1");
            Assert.AreEqual(LiveChannels[1].PlayUrl, "http://test-bucket.oss-cn-hangzhou.aliyuncs.com/channel-1/playlist.m3u8");

            data =
                @" 
                <ListLiveChannelResult>
                    <Prefix>channel</Prefix>
                    <Marker>channel</Marker>
                    <NextMarker></NextMarker>
                </ListLiveChannelResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Prefix, "channel");
            Assert.AreEqual(result.Marker, "channel");
            Assert.AreEqual(result.MaxKeys.HasValue, false);
            Assert.AreEqual(result.IsTruncated.HasValue, false);
            Assert.AreEqual(result.NextMarker, "");
            LiveChannels = OssTestUtils.ToArray(result.LiveChannels);
            Assert.AreEqual(LiveChannels.Count, 0);

            data =
                @" 
                <ListLiveChannelResult>
                </ListLiveChannelResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Prefix, null);
            Assert.AreEqual(result.Marker, null);
            Assert.AreEqual(result.MaxKeys.HasValue, false);
            Assert.AreEqual(result.IsTruncated.HasValue, false);
            Assert.AreEqual(result.NextMarker, null);
            LiveChannels = OssTestUtils.ToArray(result.LiveChannels);
            Assert.AreEqual(LiveChannels.Count, 0);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void GetLiveChannelHistoryResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetLiveChannelHistoryResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <LiveChannelHistory>
                  <LiveRecord>
                    <StartTime>2016-07-30T01:53:21.000Z</StartTime>
                    <EndTime>2016-07-30T01:53:31.000Z</EndTime>
                    <RemoteAddr>10.101.194.148:56861</RemoteAddr>
                  </LiveRecord>
                  <LiveRecord>
                    <StartTime>2016-07-30T01:53:35.000Z</StartTime>
                    <EndTime>2016-07-30T01:53:45.000Z</EndTime>
                    <RemoteAddr>10.101.194.148:57126</RemoteAddr>
                  </LiveRecord>
                  <LiveRecord>
                    <StartTime>2016-07-30T01:53:49.000Z</StartTime>
                    <EndTime>2016-07-30T01:53:59.000Z</EndTime>
                    <RemoteAddr>10.101.194.148:57577</RemoteAddr>
                  </LiveRecord>
                  <LiveRecord>
                    <StartTime>2016-07-30T01:54:04.000Z</StartTime>
                    <EndTime>2016-07-30T01:54:14.000Z</EndTime>
                    <RemoteAddr>10.101.194.148:57632</RemoteAddr>
                  </LiveRecord>
                </LiveChannelHistory>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            var LiveRecords = OssTestUtils.ToArray(result.LiveRecords);
            Assert.AreEqual(LiveRecords.Count, 4);
            Assert.AreEqual(LiveRecords[0].StartTime, DateUtils.ParseIso8601Date("2016-07-30T01:53:21.000Z"));
            Assert.AreEqual(LiveRecords[0].EndTime, DateUtils.ParseIso8601Date("2016-07-30T01:53:31.000Z"));
            Assert.AreEqual(LiveRecords[0].RemoteAddr, "10.101.194.148:56861");
            Assert.AreEqual(LiveRecords[3].StartTime, DateUtils.ParseIso8601Date("2016-07-30T01:54:04.000Z"));
            Assert.AreEqual(LiveRecords[3].EndTime, DateUtils.ParseIso8601Date("2016-07-30T01:54:14.000Z"));
            Assert.AreEqual(LiveRecords[3].RemoteAddr, "10.101.194.148:57632");

            data =
                @" 
                <LiveChannelHistory>
                  <LiveRecord>
                    <StartTime>2016-07-30T01:53:35.000Z</StartTime>
                    <EndTime>2016-07-30T01:53:45.000Z</EndTime>
                    <RemoteAddr>10.101.194.148:57126</RemoteAddr>
                  </LiveRecord>
                  <LiveRecord>
                    <StartTime>2016-07-30T01:53:49.000Z</StartTime>
                    <EndTime>2016-07-30T01:53:59.000Z</EndTime>
                    <RemoteAddr>10.101.194.148:57577</RemoteAddr>
                  </LiveRecord>
                </LiveChannelHistory>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            LiveRecords = OssTestUtils.ToArray(result.LiveRecords);
            Assert.AreEqual(LiveRecords.Count, 2);
            Assert.AreEqual(LiveRecords[0].StartTime, DateUtils.ParseIso8601Date("2016-07-30T01:53:35.000Z"));
            Assert.AreEqual(LiveRecords[0].EndTime, DateUtils.ParseIso8601Date("2016-07-30T01:53:45.000Z"));
            Assert.AreEqual(LiveRecords[0].RemoteAddr, "10.101.194.148:57126");
            Assert.AreEqual(LiveRecords[1].StartTime, DateUtils.ParseIso8601Date("2016-07-30T01:53:49.000Z"));
            Assert.AreEqual(LiveRecords[1].EndTime, DateUtils.ParseIso8601Date("2016-07-30T01:53:59.000Z"));
            Assert.AreEqual(LiveRecords[1].RemoteAddr, "10.101.194.148:57577");

            data =
                @" 
                <LiveChannelHistory>
                </LiveChannelHistory>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            LiveRecords = OssTestUtils.ToArray(result.LiveRecords);
            Assert.AreEqual(LiveRecords.Count, 0);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void GetLiveChannelStatResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetLiveChannelStatResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <LiveChannelStat>
                  <Status>Live</Status>
                  <ConnectedTime>2016-08-25T06:25:15.000Z</ConnectedTime>
                  <RemoteAddr>10.1.2.3:47745</RemoteAddr>
                  <Video>
                    <Width>1280</Width>
                    <Height>536</Height>
                    <FrameRate>24</FrameRate>
                    <Bandwidth>101</Bandwidth>
                    <Codec>H264</Codec>
                  </Video>
                  <Audio>
                    <Bandwidth>10</Bandwidth>
                    <SampleRate>44100</SampleRate>
                    <Codec>ADPCM</Codec>
                  </Audio>
                </LiveChannelStat>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Status, "Live");
            Assert.AreEqual(result.ConnectedTime, DateUtils.ParseIso8601Date("2016-08-25T06:25:15.000Z"));
            Assert.AreEqual(result.RemoteAddr, "10.1.2.3:47745");
            Assert.AreEqual(result.Width, 1280);
            Assert.AreEqual(result.Height, 536);
            Assert.AreEqual(result.FrameRate, 24);
            Assert.AreEqual(result.VideoBandwidth, 101);
            Assert.AreEqual(result.VideoCodec, "H264");
            Assert.AreEqual(result.SampleRate, 44100);
            Assert.AreEqual(result.AudioBandwidth, 10);
            Assert.AreEqual(result.AudioCodec, "ADPCM");


            data =
                @" 
                <LiveChannelStat>
                  <Status>Idle</Status>
                </LiveChannelStat>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Status, "Idle");

            data =
                @" 
                <LiveChannelStat>
                </LiveChannelStat>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void GetLiveChannelInfoResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetLiveChannelInfoResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <LiveChannelConfiguration>
                  <Description>description</Description>
                  <Status>enabled</Status>
                  <Target>
                    <Type>HLS</Type>
                    <FragDuration>2</FragDuration>
                    <FragCount>3</FragCount>
                    <PlaylistName>playlist.m3u8</PlaylistName>
                  </Target>
                </LiveChannelConfiguration>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Description, "description");
            Assert.AreEqual(result.Status, "enabled");
            Assert.AreEqual(result.Type, "HLS");
            Assert.AreEqual(result.FragDuration, 2);
            Assert.AreEqual(result.FragCount, 3);
            Assert.AreEqual(result.PlaylistName, "playlist.m3u8");

            data =
                @" 
                <LiveChannelConfiguration>
                  <Description></Description>
                  <Status>disabled</Status>
                </LiveChannelConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Description, "");
            Assert.AreEqual(result.Status, "disabled");

            data =
                @" 
                <LiveChannelConfiguration>
                </LiveChannelConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void CreateLiveChannelResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateCreateLiveChannelResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <CreateLiveChannelResult>
                  <PublishUrls>
                    <Url>rtmp://test-bucket.oss-cn-hangzhou.aliyuncs.com/live/test-channel</Url>
                  </PublishUrls>
                  <PlayUrls>
                    <Url>http://test-bucket.oss-cn-hangzhou.aliyuncs.com/test-channel/playlist.m3u8</Url>
                  </PlayUrls>
                </CreateLiveChannelResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.PublishUrl, "rtmp://test-bucket.oss-cn-hangzhou.aliyuncs.com/live/test-channel");
            Assert.AreEqual(result.PlayUrl, "http://test-bucket.oss-cn-hangzhou.aliyuncs.com/test-channel/playlist.m3u8");

            data =
                @" 
                <CreateLiveChannelResult>
                  <PublishUrls>
                    <Url>rtmp://test-bucket.oss-cn-hangzhou.aliyuncs.com/live/test-channel</Url>
                  </PublishUrls>
                </CreateLiveChannelResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.PublishUrl, "rtmp://test-bucket.oss-cn-hangzhou.aliyuncs.com/live/test-channel");
            Assert.AreEqual(result.PlayUrl, null);

            data =
                @" 
                <CreateLiveChannelResult>
                  <PublishUrls>
                  </PublishUrls>
                </CreateLiveChannelResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.PublishUrl, null);
            Assert.AreEqual(result.PlayUrl, null);

            data =
                @" 
                <CreateLiveChannelResult>
                  <PlayUrls>
                  </PlayUrls>
                </CreateLiveChannelResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.PublishUrl, null);
            Assert.AreEqual(result.PlayUrl, null);

            data =
                @" 
                <CreateLiveChannelResult>
                </CreateLiveChannelResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.PublishUrl, null);
            Assert.AreEqual(result.PlayUrl, null);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void GetVodPlaylistResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetVodPlaylistResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                #EXTM3U
                #EXT-X-VERSION:3
                #EXT-X-MEDIA-SEQUENCE:0
                #EXT-X-TARGETDURATION:13
                #EXTINF:7.120,
                1543895706266.ts
                #EXTINF:5.840,
                1543895706323.ts
                #EXT-X-ENDLIST
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Playlist, data);

            data = "";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Playlist, data);
        }

        [Test]
        public void GetBucketInventoryConfigurationResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetBucketInventoryConfigurationResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <InventoryConfiguration>
                     <Id>report1</Id>
                     <IsEnabled>true</IsEnabled>
                     <Destination>
                        <OSSBucketDestination>
                           <Format>CSV</Format>
                           <AccountId>1000000000000000</AccountId>
                           <RoleArn>acs:ram::1000000000000000:role/bucket-inventory-role</RoleArn>
                           <Bucket>acs:oss:::bucket_0001</Bucket>
                           <Prefix>prefix1</Prefix>
                           <Encryption>
                              <SSE-OSS/>
                           </Encryption>
                        </OSSBucketDestination>
                     </Destination>
                     <Schedule>
                        <Frequency>Daily</Frequency>
                     </Schedule>
                     <Filter>
                       <Prefix>myprefix/</Prefix>
                     </Filter>
                     <IncludedObjectVersions>All</IncludedObjectVersions>
                     <OptionalFields>
                        <Field>Size</Field>
                        <Field>LastModifiedDate</Field>
                        <Field>StorageClass</Field>
                        <Field>IsMultipartUploaded</Field>
                        <Field>EncryptionStatus</Field>
                        <Field>ETag</Field>
                     </OptionalFields>
                  </InventoryConfiguration>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            var config = result.Configuration;
            Assert.AreEqual(config.Id, "report1");
            Assert.AreEqual(config.IsEnabled, true);
            Assert.AreEqual(config.Destination.OSSBucketDestination.Format, InventoryFormat.CSV);
            Assert.AreEqual(config.Destination.OSSBucketDestination.AccountId, "1000000000000000");
            Assert.AreEqual(config.Destination.OSSBucketDestination.RoleArn, "acs:ram::1000000000000000:role/bucket-inventory-role");
            Assert.AreEqual(config.Destination.OSSBucketDestination.Bucket, "bucket_0001");
            Assert.AreEqual(config.Destination.OSSBucketDestination.Prefix, "prefix1");
            Assert.AreNotEqual(config.Destination.OSSBucketDestination.Encryption.SSEOSS, null);
            Assert.AreEqual(config.Destination.OSSBucketDestination.Encryption.SSEKMS, null);
            Assert.AreEqual(config.Schedule.Frequency, InventoryFrequency.Daily);
            Assert.AreEqual(config.Filter.Prefix, "myprefix/");
            Assert.AreEqual(config.IncludedObjectVersions, InventoryIncludedObjectVersions.All);
            Assert.AreEqual(config.OptionalFields[0], InventoryOptionalField.Size);
            Assert.AreEqual(config.OptionalFields[1], InventoryOptionalField.LastModifiedDate);
            Assert.AreEqual(config.OptionalFields[2], InventoryOptionalField.StorageClass);
            Assert.AreEqual(config.OptionalFields[3], InventoryOptionalField.IsMultipartUploaded);
            Assert.AreEqual(config.OptionalFields[4], InventoryOptionalField.EncryptionStatus);
            Assert.AreEqual(config.OptionalFields[5], InventoryOptionalField.ETag);

            data =
                @" 
                <InventoryConfiguration>
                     <Id>report1</Id>
                     <IsEnabled>false</IsEnabled>
                     <Destination>
                        <OSSBucketDestination>
                           <Format>CSV</Format>
                           <AccountId>1000000000000000</AccountId>
                           <RoleArn>acs:ram::1000000000000000:role/bucket-inventory-role</RoleArn>
                           <Bucket>acs:oss:::bucket_0001</Bucket>
                           <Prefix>prefix1</Prefix>
                           <Encryption>
                              <SSE-KMS/>
                           </Encryption>
                        </OSSBucketDestination>
                     </Destination>
                     <Schedule>
                        <Frequency>Weekly</Frequency>
                     </Schedule>
                     <IncludedObjectVersions>Current</IncludedObjectVersions>
                     <OptionalFields>
                     </OptionalFields>
                  </InventoryConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            config = result.Configuration;
            Assert.AreEqual(config.IsEnabled, false);
            Assert.AreEqual(config.Destination.OSSBucketDestination.Encryption.SSEOSS, null);
            Assert.AreNotEqual(config.Destination.OSSBucketDestination.Encryption.SSEKMS, null);
            Assert.AreEqual(config.Destination.OSSBucketDestination.Encryption.SSEKMS.KeyId, null);
            Assert.AreEqual(config.Filter, null);
            Assert.AreEqual(config.Schedule.Frequency, InventoryFrequency.Weekly);
            Assert.AreEqual(config.IncludedObjectVersions, InventoryIncludedObjectVersions.Current);
            Assert.AreEqual(config.OptionalFields.Count, 0);

            data =
                @" 
                <InventoryConfiguration>
                     <Id>report1</Id>
                     <IsEnabled>false</IsEnabled>
                     <Destination>
                        <OSSBucketDestination>
                           <Format>CSV</Format>
                           <AccountId>1000000000000000</AccountId>
                           <RoleArn>acs:ram::1000000000000000:role/bucket-inventory-role</RoleArn>
                           <Bucket>acs:oss:::bucket_0001</Bucket>
                           <Prefix>prefix1</Prefix>
                           <Encryption>
                             <SSE-KMS>
                               <KeyId>keyId</KeyId>
                             </SSE-KMS>
                           </Encryption>
                        </OSSBucketDestination>
                     </Destination>
                     <Schedule>
                        <Frequency>Weekly</Frequency>
                     </Schedule>
                     <IncludedObjectVersions>Current</IncludedObjectVersions>
                     <OptionalFields>
                     </OptionalFields>
                  </InventoryConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            config = result.Configuration;
            Assert.AreEqual(config.Destination.OSSBucketDestination.Encryption.SSEOSS, null);
            Assert.AreNotEqual(config.Destination.OSSBucketDestination.Encryption.SSEKMS, null);
            Assert.AreEqual(config.Destination.OSSBucketDestination.Encryption.SSEKMS.KeyId, "keyId");

            data =
                @" 
                <InventoryConfiguration>
                     <Id>report1</Id>
                     <IsEnabled>true</IsEnabled>
                     <Destination>
                        <OSSBucketDestination>
                           <Format>CSV</Format>
                           <AccountId>1000000000000000</AccountId>
                           <RoleArn>acs:ram::1000000000000000:role/bucket-inventory-role</RoleArn>
                           <Bucket>acs:oss:::bucket_0001</Bucket>
                           <Prefix>prefix1</Prefix>
                           <Encryption>
                           </Encryption>
                        </OSSBucketDestination>
                     </Destination>
                     <Schedule>
                        <Frequency>Daily</Frequency>
                     </Schedule>
                     <IncludedObjectVersions>All</IncludedObjectVersions>
                  </InventoryConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            config = result.Configuration;
            Assert.AreEqual(config.Destination.OSSBucketDestination.Encryption.SSEOSS, null);
            Assert.AreEqual(config.Destination.OSSBucketDestination.Encryption.SSEKMS, null);

            data =
                @" 
                <InventoryConfiguration>
                    <Destination>
                        <OSSBucketDestination>
                        </OSSBucketDestination>
                    </Destination>
                </InventoryConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            config = result.Configuration;
            Assert.AreNotEqual(config.Destination.OSSBucketDestination, null);
            Assert.AreEqual(config.Filter, null);
            Assert.AreEqual(config.Schedule, null);
            Assert.AreEqual(config.IncludedObjectVersions, InventoryIncludedObjectVersions.All);
            Assert.AreEqual(config.OptionalFields.Count, 0);

            data =
                @" 
                <InventoryConfiguration>
                </InventoryConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void ListBucketInventoryConfigurationResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateListBucketInventoryConfigurationResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <ListInventoryConfigurationsResult>
                    <InventoryConfiguration>
                        <Id>report1</Id>
                        <IsEnabled>true</IsEnabled>
                        <Destination>
                            <OSSBucketDestination>
                                <Format>CSV</Format>
                                <AccountId>1000000000000000</AccountId>
                                <RoleArn>acs:ram::1000000000000000:role/bucket-inventory-role</RoleArn>
                                <Bucket>acs:oss:::destination-bucket</Bucket>
                                <Prefix>prefix1</Prefix>
                            </OSSBucketDestination>
                        </Destination>
                        <Schedule>
                            <Frequency>Daily</Frequency>
                        </Schedule>
                        <Filter>
                            <Prefix>prefix/One</Prefix>
                        </Filter>
                        <IncludedObjectVersions>All</IncludedObjectVersions>
                        <OptionalFields>
                            <Field>Size</Field>
                            <Field>LastModifiedDate</Field>
                            <Field>StorageClass</Field>
                            <Field>IsMultipartUploaded</Field>
                            <Field>EncryptionStatus</Field>
                            <Field>ETag</Field>
                        </OptionalFields>
                    </InventoryConfiguration>
                    <InventoryConfiguration>
                        <Id>report2</Id>
                        <IsEnabled>true</IsEnabled>
                        <Destination>
                            <OSSBucketDestination>
                                <Format>CSV</Format>
                                <AccountId>1000000000000000</AccountId>
                                <RoleArn>acs:ram::1000000000000000:role/bucket-inventory-role</RoleArn>
                                <Bucket>acs:oss:::destination-bucket</Bucket>
                                <Prefix>prefix2</Prefix>
                            </OSSBucketDestination>
                        </Destination>
                        <Schedule>
                            <Frequency>Daily</Frequency>
                        </Schedule>
                        <Filter>
                            <Prefix>prefix/Two</Prefix>
                        </Filter>
                        <IncludedObjectVersions>All</IncludedObjectVersions>
                        <OptionalFields>
                            <Field>Size</Field>
                            <Field>LastModifiedDate</Field>
                            <Field>ETag</Field>
                            <Field>StorageClass</Field>
                            <Field>IsMultipartUploaded</Field>
                            <Field>EncryptionStatus</Field>
                        </OptionalFields>
                     </InventoryConfiguration>
                     <InventoryConfiguration>
                        <Id>report3</Id>
                        <IsEnabled>false</IsEnabled>
                        <Destination>
                            <OSSBucketDestination>
                                <Format>CSV</Format>
                                <AccountId>1000000000000000</AccountId>
                                <RoleArn>acs:ram::1000000000000000:role/bucket-inventory-role</RoleArn>
                                <Bucket>acs:oss:::destination-bucket</Bucket>
                                <Prefix>prefix3</Prefix>
                            </OSSBucketDestination>
                        </Destination>
                        <Schedule>
                            <Frequency>Daily</Frequency>
                        </Schedule>
                        <Filter>
                            <Prefix>prefix/Three</Prefix>
                        </Filter>
                        <IncludedObjectVersions>All</IncludedObjectVersions>
                    </InventoryConfiguration>
                    <IsTruncated>true</IsTruncated>
                    <NextContinuationToken>report2</NextContinuationToken> 
                </ListInventoryConfigurationsResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            var configs = OssTestUtils.ToArray(result.Configurations);
            Assert.AreEqual(configs.Count, 3);
            Assert.AreEqual(result.IsTruncated, true);
            Assert.AreEqual(result.NextContinuationToken, "report2");
            var config = configs[0];
            Assert.AreEqual(config.Id, "report1");
            Assert.AreEqual(config.IsEnabled, true);
            Assert.AreEqual(config.Destination.OSSBucketDestination.Format, InventoryFormat.CSV);
            Assert.AreEqual(config.Destination.OSSBucketDestination.AccountId, "1000000000000000");
            Assert.AreEqual(config.Destination.OSSBucketDestination.RoleArn, "acs:ram::1000000000000000:role/bucket-inventory-role");
            Assert.AreEqual(config.Destination.OSSBucketDestination.Bucket, "destination-bucket");
            Assert.AreEqual(config.Destination.OSSBucketDestination.Prefix, "prefix1");
            Assert.AreEqual(config.Destination.OSSBucketDestination.Encryption, null);
            Assert.AreEqual(config.Schedule.Frequency, InventoryFrequency.Daily);
            Assert.AreEqual(config.Filter.Prefix, "prefix/One");
            Assert.AreEqual(config.IncludedObjectVersions, InventoryIncludedObjectVersions.All);
            Assert.AreEqual(config.OptionalFields[0], InventoryOptionalField.Size);
            Assert.AreEqual(config.OptionalFields[1], InventoryOptionalField.LastModifiedDate);
            Assert.AreEqual(config.OptionalFields[2], InventoryOptionalField.StorageClass);
            Assert.AreEqual(config.OptionalFields[3], InventoryOptionalField.IsMultipartUploaded);
            Assert.AreEqual(config.OptionalFields[4], InventoryOptionalField.EncryptionStatus);
            Assert.AreEqual(config.OptionalFields[5], InventoryOptionalField.ETag);

            config = configs[2];
            Assert.AreEqual(config.Id, "report3");
            Assert.AreEqual(config.IsEnabled, false);
            Assert.AreEqual(config.Filter.Prefix, "prefix/Three");

            data =
                @" 
                <ListInventoryConfigurationsResult>
                    <IsTruncated>false</IsTruncated>
                    <NextContinuationToken></NextContinuationToken> 
                </ListInventoryConfigurationsResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            configs = OssTestUtils.ToArray(result.Configurations);
            Assert.AreEqual(configs.Count, 0);
            Assert.AreEqual(result.IsTruncated, false);
            Assert.AreEqual(result.NextContinuationToken, "");

            data =
                @" 
                <ListInventoryConfigurationsResult>
                </ListInventoryConfigurationsResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            configs = OssTestUtils.ToArray(result.Configurations);
            Assert.AreEqual(configs.Count, 0);
            Assert.AreEqual(result.IsTruncated, false);
            Assert.AreEqual(result.NextContinuationToken, null);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            xmlStream.Headers.Remove(HttpHeaders.ContentLength);
            xmlStream.Headers.Add(HttpHeaders.ContentLength, data.Length.ToString());
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void SerializerFactoryTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            Assert.AreNotEqual(factory, null);

            factory = SerializerFactory.GetFactory(null);
            Assert.AreNotEqual(factory, null);

            factory = SerializerFactory.GetFactory("text/json");
            Assert.AreEqual(factory, null);
        }

        [Test]
        public void SetBucketCorsRequestSerializerTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            var serializer = factory.CreateSetBucketCorsRequestSerializer();

            var rules = new List<CORSRule>();
            var rule = new CORSRule();
            //rule.AllowedHeaders = null;
            rule.AllowedMethods = null;
            //rule.AllowedOrigins = null;
            //rule.ExposeHeaders = null;
            rules.Add(new CORSRule());
            var request = new SetBucketCorsRequest("bucket");
            request.CORSRules = rules;
            var result = serializer.Serialize(request);
            Assert.AreNotEqual(result, null);
        }

        [Test]
        public void SetBucketLifecycleRequestSerializerTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            var serializer = factory.CreateSetBucketLifecycleRequestSerializer();

            var request = new SetBucketLifecycleRequest("bucket");
            var result = serializer.Serialize(request);
        }

        [Test]
        public void CreateLiveChannelRequestSerializerTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            var serializer = factory.CreateCreateLiveChannelRequestSerializer();

            var request = new CreateLiveChannelRequest("bucket", "channel");
            var result = serializer.Serialize(request);
            var reader = new StreamReader(result);
            var str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<Snapshot>"), false);

            request = new CreateLiveChannelRequest("bucket", "channel")
            {
                RoleName = "my role name",
                DestBucket = "my bucket",
                NotifyTopic = "topic",
                Interval = 10
            };
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<Snapshot>"), true);
            Assert.AreEqual(str.Contains("<RoleName>my role name</RoleName>"), true);
            Assert.AreEqual(str.Contains("<DestBucket>my bucket</DestBucket>"), true);
            Assert.AreEqual(str.Contains("<NotifyTopic>topic</NotifyTopic>"), true);
            Assert.AreEqual(str.Contains("<Interval>10</Interval>"), true);
            Assert.AreEqual(str.Contains("</Snapshot>"), true);
        }

        [Test]
        public void RestoreObjectRequestSerializerTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            var serializer = factory.CreateRestoreObjectRequestSerializer();

            var request = new RestoreObjectRequest("bucket", "object");
            var result = serializer.Serialize(request);
            var reader = new StreamReader(result);
            var str = reader.ReadToEnd();
            Assert.AreEqual(request.IsUseDefaultParameter(), true);
            Assert.AreEqual(str.Contains("<Days>1</Days>"), true);
            Assert.AreEqual(str.Contains("<JobParameters>\r\n    <Tier>Standard</Tier>\r\n  </JobParameters>"), true);

            request = new RestoreObjectRequest("bucket", "object")
            {
                Days = 10,
                Tier = TierType.Bulk
            };
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(request.IsUseDefaultParameter(), false);
            Assert.AreEqual(str.Contains("<Days>10</Days>"), true);
            Assert.AreEqual(str.Contains("<JobParameters>\r\n    <Tier>Bulk</Tier>\r\n  </JobParameters>"), true);

            request = new RestoreObjectRequest("bucket", "object")
            {
                Days = 9,
            };
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(request.IsUseDefaultParameter(), false);
            Assert.AreEqual(str.Contains("<Days>9</Days>"), true);
            Assert.AreEqual(str.Contains("<JobParameters>\r\n    <Tier>Standard</Tier>\r\n  </JobParameters>"), true);

            request = new RestoreObjectRequest("bucket", "object")
            {
                Tier = TierType.Expedited
            };
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(request.IsUseDefaultParameter(), false);
            Assert.AreEqual(str.Contains("<Days>1</Days>"), true);
            Assert.AreEqual(str.Contains("<JobParameters>\r\n    <Tier>Expedited</Tier>\r\n  </JobParameters>"), true);
        }

        [Test]
        public void XmlStreamSerializerTest()
        {
            var streamSerializer = new XmlStreamSerializer<ResponseMock>();
            try
            {
                streamSerializer.Serialize(null);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void TagTest()
        {
            var tag0 = new Tag { Key = "key", Value = "value" };
            var tag1 = new Tag { Key = "key", Value = "value" };
            var tag2 = new Tag { Key = "key", Value = "value2" };
            var tag3 = new Tag { Key = "key3", Value = "value" };
            var tag4 = new Tag { Key = "key" };
            var tag5 = new Tag { Value = "value" };

            Tag tag6 = null;

            Tag tag7 = tag0;

            Assert.IsTrue(tag0.Equals(tag1));
            Assert.IsTrue(tag0.Equals(tag7));
            Assert.IsTrue(tag0.Equals(tag0));

            Assert.IsFalse(tag0.Equals(tag2));
            Assert.IsFalse(tag0.Equals(tag3));
            Assert.IsFalse(tag0.Equals(tag4));
            Assert.IsFalse(tag0.Equals(tag5));
            Assert.IsFalse(tag0.Equals(tag6));
            Assert.IsFalse(tag4.Equals(tag5));
        }

        [Test]
        public void EndTest()
        {
            var hash = new HashingWrapperCrc64();
        }

        [Test]
        public void SelectObjectRequestDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateSelectObjectRequestDeserializer(new SelectObjectRequest("bucket", "key"));
            var headers = new Dictionary<string, string>();
            var content = new MemoryStream(50);
            //test frame
            byte[] continueFrame = new byte[] {
                1, 0x80, 0, 4,
                0, 0, 0, 8,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 158,
                0, 0, 0, 0};
            byte[] endFrame = new byte[] {
                1, 0x80, 0, 5,
                0, 0, 0, 0x14,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0xc8,
                0xbb, 0x57, 0xb1, 0x0b
            };
            byte[] dataFrame = new byte[] {
                1, 0x80, 0, 1,
                0, 0, 0, 18,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 158,
                97, 98, 99, 100, 101, 102, 103, 104, 105, 106,
                54, 113, 13, 83};

            //data

            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(dataFrame, 0, dataFrame.Length);
            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(endFrame, 0, endFrame.Length);

            content.Seek(0, SeekOrigin.Begin);

            headers["x-oss-select-output-raw"] = "false";
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);

            var buf = new byte[256];
            int offset = 0;
            for (int i = 1; i < 30; i++)
            {
                int got = result.Content.Read(buf, offset, i);
                offset += got;
            }

            Assert.AreEqual("abcdefghij".Length, offset);
            string str = Encoding.Default.GetString(buf, 0, offset);
            Assert.AreEqual("abcdefghij", str);

            //disable CRC
            byte[] dataFrame_without_crc = new byte[] {
                1, 0x80, 0, 1,
                0, 0, 0, 18,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 158,
                97, 98, 99, 100, 101, 102, 103, 104, 105, 106,
                0, 0, 0, 0};

            content = new MemoryStream();
            headers = new Dictionary<string, string>();
            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(dataFrame_without_crc, 0, dataFrame_without_crc.Length);
            content.Write(endFrame, 0, endFrame.Length);
            content.Seek(0, SeekOrigin.Begin);

            headers["x-oss-select-output-raw"] = "false";
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            int cnt = result.Content.Read(buf, 0, 256);
            Assert.AreEqual("abcdefghij".Length, cnt);
            str = Encoding.Default.GetString(buf, 0, cnt);
            Assert.AreEqual("abcdefghij", str);

            //invalid CRC
            byte[] dataFrame_invalid_crc = new byte[] {
                1, 0x80, 0, 1,
                0, 0, 0, 18,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 158,
                97, 98, 99, 100, 101, 102, 103, 104, 105, 106,
                1, 0, 0, 0};

            content = new MemoryStream();
            headers = new Dictionary<string, string>();
            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(dataFrame_invalid_crc, 0, dataFrame_invalid_crc.Length);
            content.Write(endFrame, 0, endFrame.Length);
            content.Seek(0, SeekOrigin.Begin);

            headers["x-oss-select-output-raw"] = "false";
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            try
            {
                cnt = result.Content.Read(buf, 0, 256);
                Assert.IsTrue(false);
            }
            catch (ClientException e)
            {
                Assert.IsTrue(e.Message.Contains("Payload checksum check fail server CRC"));
            }

            //raw data
            content = new MemoryStream(Encoding.ASCII.GetBytes("abcdef"));
            headers = new Dictionary<string, string>();

            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            cnt = result.Content.Read(buf, 0, 256);
            Assert.AreEqual("abcdef".Length, cnt);
            str = Encoding.Default.GetString(buf, 0, cnt);
            Assert.AreEqual("abcdef", str);

            //test endframe with error message
            byte[] endFrame_with_message = new byte[] {
                1, 0x80, 0, 5,
                0, 0, 0, 0x18,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0xc8,
                97, 98, 99, 100,
                0xbb, 0x57, 0xb1, 0x0b
            };

            content = new MemoryStream();
            headers = new Dictionary<string, string>();
            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(dataFrame, 0, dataFrame.Length);
            content.Write(endFrame_with_message, 0, endFrame_with_message.Length);
            content.Seek(0, SeekOrigin.Begin);

            headers["x-oss-select-output-raw"] = "false";
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            cnt = result.Content.Read(buf, 0, 256);
            Assert.AreEqual("abcdefghij".Length, cnt);
            str = Encoding.Default.GetString(buf, 0, cnt);
            Assert.AreEqual("abcdefghij", str);
            var inputstream = (SelectObjectStream)result.Content;
            Assert.AreEqual("abcd", inputstream.ErrorMessage);
            Assert.AreEqual(200, inputstream.StatusCode);

            //without endframe
            content = new MemoryStream();
            headers = new Dictionary<string, string>();
            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(dataFrame, 0, dataFrame.Length);
            content.Write(continueFrame, 0, continueFrame.Length);

            content.Seek(0, SeekOrigin.Begin);

            headers["x-oss-select-output-raw"] = "false";
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            cnt = result.Content.Read(buf, 0, 256);

            Assert.AreEqual("abcdefghij".Length, offset);
            str = Encoding.Default.GetString(buf, 0, offset);
            Assert.AreEqual("abcdefghij", str);

            //test endframe with long error message
            byte[] endFrame_with_long_message = new byte[] {
                1, 0x80, 0, 5,
                0, 0, 0, 152,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0xc8,
                97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                0xbb, 0x57, 0xb1, 0x0b
            };

            content = new MemoryStream();
            headers = new Dictionary<string, string>();
            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(dataFrame, 0, dataFrame.Length);
            content.Write(endFrame_with_long_message, 0, endFrame_with_long_message.Length);
            content.Seek(0, SeekOrigin.Begin);

            headers["x-oss-select-output-raw"] = "false";
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            cnt = result.Content.Read(buf, 0, 256);
            Assert.AreEqual("abcdefghij".Length, cnt);
            str = Encoding.Default.GetString(buf, 0, cnt);
            Assert.AreEqual("abcdefghij", str);
            inputstream = (SelectObjectStream)result.Content;
            Assert.AreEqual(128, inputstream.ErrorMessage.Length);
            Assert.AreEqual(200, inputstream.StatusCode);

            //bad data
            content = new MemoryStream(Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz"));
            headers = new Dictionary<string, string>();

            headers["x-oss-select-output-raw"] = "false";
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            try
            {
                cnt = result.Content.Read(buf, 0, 256);
            }
            catch (ClientException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void SelectObjectCSVInputFormatTest()
        {
            var inputFormat = new SelectObjectCSVInputFormat();
            Assert.AreEqual(null, inputFormat.AllowQuotedRecordDelimiter);
            Assert.AreEqual(null, inputFormat.FileHeaderInfo);

            Assert.AreEqual(null, inputFormat.RecordDelimiter);
            Assert.AreEqual(null, inputFormat.FieldDelimiter);
            Assert.AreEqual(null, inputFormat.QuoteCharacter);
            Assert.AreEqual(null, inputFormat.CommentCharacter);
            Assert.AreEqual(null, inputFormat.Range);

            Assert.AreEqual(CompressionType.None, inputFormat.CompressionType);
        }

        [Test]
        public void SelectObjectCSVOutputFormatTest()
        {
            var outputFormat = new SelectObjectCSVOutputFormat();
            Assert.AreEqual(null, outputFormat.OutputRawData);
            Assert.AreEqual(null, outputFormat.EnablePayloadCrc);

            Assert.AreEqual(null, outputFormat.RecordDelimiter);
            Assert.AreEqual(null, outputFormat.FieldDelimiter);
            Assert.AreEqual(null, outputFormat.KeepAllColumns);
            Assert.AreEqual(null, outputFormat.OutputHeader);
        }

        [Test]
        public void SelectObjectOptionsTest()
        {
            var options = new SelectObjectOptions();
            Assert.AreEqual(null, options.SkipPartialDataRecord);
            Assert.AreEqual(null, options.MaxSkippedRecordsAllowed);
        }

        [Test]
        public void SelectObjectJSONInputFormatTest()
        {
            var inputFormat = new SelectObjectJSONInputFormat();
            Assert.AreEqual(JSONType.DOCUMENT, inputFormat.Type);
            Assert.AreEqual(null, inputFormat.ParseJsonNumberAsString);
            Assert.AreEqual(null, inputFormat.Range);

            Assert.AreEqual(CompressionType.None, inputFormat.CompressionType);
        }

        [Test]
        public void SelectObjectJSONOutputFormatTest()
        {
            var outputFormat = new SelectObjectJSONOutputFormat();
            Assert.AreEqual(null, outputFormat.OutputRawData);
            Assert.AreEqual(null, outputFormat.EnablePayloadCrc);

            Assert.AreEqual(null, outputFormat.RecordDelimiter);
        }

        [Test]
        public void CreateSelectObjectRequestSerializerTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            var serializer = factory.CreateSelectObjectRequestSerializer();

            var request = new SelectObjectRequest("bucket", "key");
            var result = serializer.Serialize(request);
            var reader = new StreamReader(result);
            var str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<SelectRequest>"), true);
            Assert.AreEqual(str.Contains("<Expression"), true);
            Assert.AreEqual(str.Contains("<InputSerialization>"), false);
            Assert.AreEqual(str.Contains("<OutputSerialization>"), false);
            Assert.AreEqual(str.Contains("<Options>"), false);

            //Set CVS inputFormat, outputFormat, Opitons by default
            request = new SelectObjectRequest("bucket", "key");
            request.Expression = "select * from test";
            var inputFormat = new SelectObjectCSVInputFormat();
            var outputFormat = new SelectObjectCSVOutputFormat();
            var options = new SelectObjectOptions();
            request.InputFormat = inputFormat;
            request.OutputFormat = outputFormat;
            request.Options = options;
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<SelectRequest>"), true);
            Assert.AreEqual(str.Contains("<Expression>c2VsZWN0ICogZnJvbSB0ZXN0"), true);
            Assert.AreEqual(str.Contains("<InputSerialization>"), true);
            Assert.AreEqual(str.Contains("<CompressionType>None"), true);
            Assert.AreEqual(str.Contains("<CSV />"), true);
            Assert.AreEqual(str.Contains("<FileHeaderInfo"), false);
            Assert.AreEqual(str.Contains("<RecordDelimiter"), false);
            Assert.AreEqual(str.Contains("<FieldDelimiter"), false);
            Assert.AreEqual(str.Contains("<QuoteCharacter"), false);
            Assert.AreEqual(str.Contains("<CommentCharacter"), false);
            Assert.AreEqual(str.Contains("<Range"), false);
            Assert.AreEqual(str.Contains("<AllowQuotedRecordDelimiter"), false);

            Assert.AreEqual(str.Contains("<OutputSerialization>"), true);
            Assert.AreEqual(str.Contains("<RecordDelimiter"), false);
            Assert.AreEqual(str.Contains("<FieldDelimiter"), false);
            Assert.AreEqual(str.Contains("<KeepAllColumns"), false);
            Assert.AreEqual(str.Contains("<OutputRawData"), false);
            Assert.AreEqual(str.Contains("<EnablePayloadCrc"), false);
            Assert.AreEqual(str.Contains("<OutputHeader"), false);

            Assert.AreEqual(str.Contains("<Options />"), true);
            Assert.AreEqual(str.Contains("<SkipPartialDataRecord"), false);
            Assert.AreEqual(str.Contains("<MaxSkippedRecordsAllowed"), false);

            //
            //Set CVS inputFormat, Opitons with value
            request = new SelectObjectRequest("bucket", "key");
            request.Expression = "select * from test";
            inputFormat = new SelectObjectCSVInputFormat();
            options = new SelectObjectOptions();
            inputFormat.CompressionType = CompressionType.GZIP;
            inputFormat.RecordDelimiter = ",";  //LA==
            inputFormat.FieldDelimiter = ":";  //Og==
            inputFormat.QuoteCharacter = "\""; //Ig ==
            inputFormat.CommentCharacter = "#"; //Iw==
            inputFormat.Range = "line-range=0-20"; //Ig ==
            inputFormat.FileHeaderInfo = FileHeaderInfo.Ignore;
            inputFormat.AllowQuotedRecordDelimiter = true;

            options.MaxSkippedRecordsAllowed = 20;
            options.SkipPartialDataRecord = true;

            request.InputFormat = inputFormat;
            request.Options = options;
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<SelectRequest>"), true);
            Assert.AreEqual(str.Contains("<Expression>c2VsZWN0ICogZnJvbSB0ZXN0"), true);
            Assert.AreEqual(str.Contains("<InputSerialization>"), true);
            Assert.AreEqual(str.Contains("<CompressionType>GZIP"), true);
            Assert.AreEqual(str.Contains("<CSV>"), true);
            Assert.AreEqual(str.Contains("<RecordDelimiter>LA=="), true);
            Assert.AreEqual(str.Contains("<FieldDelimiter>Og=="), true);
            Assert.AreEqual(str.Contains("<QuoteCharacter>Ig=="), true);
            Assert.AreEqual(str.Contains("<CommentCharacter>Iw=="), true);
            Assert.AreEqual(str.Contains("<Range>line-range=0-20"), true);
            Assert.AreEqual(str.Contains("<FileHeaderInfo>Ignore"), true);
            Assert.AreEqual(str.Contains("<AllowQuotedRecordDelimiter>true"), true);

            Assert.AreEqual(str.Contains("<OutputSerialization"), false);

            Assert.AreEqual(str.Contains("<Options>"), true);
            Assert.AreEqual(str.Contains("<SkipPartialDataRecord>true"), true);
            Assert.AreEqual(str.Contains("<MaxSkippedRecordsAllowed>20"), true);

            //Set CVS outputFormat with value
            request = new SelectObjectRequest("bucket", "key");
            request.Expression = "select * from test";

            outputFormat = new SelectObjectCSVOutputFormat();
            outputFormat.RecordDelimiter = ",";  //LA==
            outputFormat.FieldDelimiter = ":";  //Og==
            outputFormat.KeepAllColumns = false;
            outputFormat.OutputHeader = true;
            outputFormat.OutputRawData = false;
            outputFormat.EnablePayloadCrc = true;

            request.OutputFormat = outputFormat;
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<SelectRequest>"), true);
            Assert.AreEqual(str.Contains("<Expression>c2VsZWN0ICogZnJvbSB0ZXN0"), true);
            Assert.AreEqual(str.Contains("<InputSerialization"), false);
            Assert.AreEqual(str.Contains("<OutputSerialization>"), true);
            Assert.AreEqual(str.Contains("<RecordDelimiter>LA=="), true);
            Assert.AreEqual(str.Contains("<FieldDelimiter>Og=="), true);
            Assert.AreEqual(str.Contains("<KeepAllColumns>false"), true);
            Assert.AreEqual(str.Contains("<OutputHeader>true"), true);
            Assert.AreEqual(str.Contains("<OutputRawData>false"), true);
            Assert.AreEqual(str.Contains("<EnablePayloadCrc>true"), true);

            Assert.AreEqual(str.Contains("<Options"), false);

            //Set JSON inputFormat, outputFormat, with default
            request = new SelectObjectRequest("bucket", "key");
            request.Expression = "select * from test";
            var jInputFormat = new SelectObjectJSONInputFormat();
            var jOutputFormat = new SelectObjectJSONOutputFormat();
            request.InputFormat = jInputFormat;
            request.OutputFormat = jOutputFormat;
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<SelectRequest>"), true);
            Assert.AreEqual(str.Contains("<Expression>c2VsZWN0ICogZnJvbSB0ZXN0"), true);
            Assert.AreEqual(str.Contains("<InputSerialization>"), true);
            Assert.AreEqual(str.Contains("<CompressionType>None"), true);
            Assert.AreEqual(str.Contains("<CSV"), false);
            Assert.AreEqual(str.Contains("<JSON>"), true);
            Assert.AreEqual(str.Contains("<Type>DOCUMENT"), true);
            Assert.AreEqual(str.Contains("<Range"), false);
            Assert.AreEqual(str.Contains("<ParseJsonNumberAsString"), false);

            Assert.AreEqual(str.Contains("<OutputSerialization />"), true);
            Assert.AreEqual(str.Contains("<RecordDelimiter"), false);
            Assert.AreEqual(str.Contains("<OutputRawData"), false);
            Assert.AreEqual(str.Contains("<EnablePayloadCrc"), false);

            Assert.AreEqual(str.Contains("<Options"), false);


            //Set JSON inputFormat, outputFormat, with value
            request = new SelectObjectRequest("bucket", "key");
            request.Expression = "select * from test";
            jInputFormat = new SelectObjectJSONInputFormat();
            jOutputFormat = new SelectObjectJSONOutputFormat();
            jInputFormat.CompressionType = CompressionType.GZIP;
            jInputFormat.Type = JSONType.LINES;
            jInputFormat.ParseJsonNumberAsString = true;
            jInputFormat.Range = "split-range=start-end";
            jOutputFormat.EnablePayloadCrc = false;
            jOutputFormat.OutputRawData = true;
            jOutputFormat.RecordDelimiter = ":";  //Og==
            request.InputFormat = jInputFormat;
            request.OutputFormat = jOutputFormat;
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<SelectRequest>"), true);
            Assert.AreEqual(str.Contains("<Expression>c2VsZWN0ICogZnJvbSB0ZXN0"), true);
            Assert.AreEqual(str.Contains("<InputSerialization>"), true);
            Assert.AreEqual(str.Contains("<CompressionType>GZIP"), true);
            Assert.AreEqual(str.Contains("<CSV"), false);
            Assert.AreEqual(str.Contains("<JSON>"), true);
            Assert.AreEqual(str.Contains("<Type>LINES"), true);
            Assert.AreEqual(str.Contains("<Range>split-range=start-end"), true);
            Assert.AreEqual(str.Contains("<ParseJsonNumberAsString>true"), true);

            Assert.AreEqual(str.Contains("<OutputSerialization>"), true);
            Assert.AreEqual(str.Contains("<RecordDelimiter>Og=="), false);
            Assert.AreEqual(str.Contains("<OutputRawData>true"), true);
            Assert.AreEqual(str.Contains("<EnablePayloadCrc>false"), true);

            Assert.AreEqual(str.Contains("<Options"), false);
        }

        [Test]
        public void CreateSelectObjectMetaCSVInputFormatTest()
        {
            var inputFormat = new CreateSelectObjectMetaCSVInputFormat();
            Assert.AreEqual(CompressionType.None, inputFormat.CompressionType);
            Assert.AreEqual(null, inputFormat.RecordDelimiter);
            Assert.AreEqual(null, inputFormat.FieldDelimiter);
            Assert.AreEqual(null, inputFormat.QuoteCharacter);
        }

        [Test]
        public void CreateSelectObjectMetaJSONInputFormatTest()
        {
            var inputFormat = new CreateSelectObjectMetaJSONInputFormat();
            Assert.AreEqual(CompressionType.None, inputFormat.CompressionType);
            Assert.AreEqual(JSONType.DOCUMENT, inputFormat.Type);
        }

        [Test]
        public void SelectObjectCsvMetaRequestSerializerTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            var serializer = factory.CreateSelectObjectCsvMetaRequestSerializer();

            var request = new CreateSelectObjectMetaRequest("bucket", "key");
            request.InputFormat = new CreateSelectObjectMetaCSVInputFormat();
            var result = serializer.Serialize(request);
            var reader = new StreamReader(result);
            var str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<CsvMetaRequest>"), true);
            Assert.AreEqual(str.Contains("<InputSerialization>"), true);
            Assert.AreEqual(str.Contains("<CompressionType>None"), true);
            Assert.AreEqual(str.Contains("<CSV />"), true);
            Assert.AreEqual(str.Contains("<OverwriteIfExists>false"), true);

            //
            request = new CreateSelectObjectMetaRequest("bucket", "key");
            var inputFormat = new CreateSelectObjectMetaCSVInputFormat();
            inputFormat.RecordDelimiter = ",";  //LA==
            inputFormat.FieldDelimiter = "#"; //Iw==
            inputFormat.QuoteCharacter = ":";  //Og==
            request.InputFormat = inputFormat;
            request.OverwriteIfExists = true;
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<CsvMetaRequest>"), true);
            Assert.AreEqual(str.Contains("<InputSerialization>"), true);
            Assert.AreEqual(str.Contains("<CompressionType>None"), true);
            Assert.AreEqual(str.Contains("<CSV>"), true);
            Assert.AreEqual(str.Contains("<RecordDelimiter>LA=="), true);
            Assert.AreEqual(str.Contains("<FieldDelimiter>Iw=="), true);
            Assert.AreEqual(str.Contains("<QuoteCharacter>Og=="), true);
            Assert.AreEqual(str.Contains("<OverwriteIfExists>true"), true);
        }

        [Test]
        public void SelectObjectJSONMetaRequestSerializerTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            var serializer = factory.CreateSelectObjectJsonMetaRequestSerializer();

            var request = new CreateSelectObjectMetaRequest("bucket", "key");
            request.InputFormat = new CreateSelectObjectMetaJSONInputFormat();
            var result = serializer.Serialize(request);
            var reader = new StreamReader(result);
            var str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<JsonMetaRequest>"), true);
            Assert.AreEqual(str.Contains("<InputSerialization>"), true);
            Assert.AreEqual(str.Contains("<CompressionType>None"), true);
            Assert.AreEqual(str.Contains("<JSON>"), true);
            Assert.AreEqual(str.Contains("<Type>DOCUMENT"), true);
            Assert.AreEqual(str.Contains("<OverwriteIfExists>false"), true);

            //
            request = new CreateSelectObjectMetaRequest("bucket", "key");
            var inputFormat = new CreateSelectObjectMetaJSONInputFormat();
            inputFormat.Type = JSONType.LINES;
            inputFormat.CompressionType = CompressionType.GZIP;
            request.InputFormat = inputFormat;
            request.OverwriteIfExists = true;
            result = serializer.Serialize(request);
            reader = new StreamReader(result);
            str = reader.ReadToEnd();
            Assert.AreEqual(str.Contains("<JsonMetaRequest>"), true);
            Assert.AreEqual(str.Contains("<InputSerialization>"), true);
            Assert.AreEqual(str.Contains("<CompressionType>GZIP"), true);
            Assert.AreEqual(str.Contains("<JSON>"), true);
            Assert.AreEqual(str.Contains("<Type>LINES"), true);
            Assert.AreEqual(str.Contains("<OverwriteIfExists>true"), true);
        }

        [Test]
        public void SelectObjectMetaRequestDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateSelectObjectMetaRequestDeserializer();
            var headers = new Dictionary<string, string>();
            var content = new MemoryStream(50);
            //test frame
            byte[] continueFrame = new byte[] {
                1, 0x80, 0, 4,
                0, 0, 0, 8,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 158,
                0, 0, 0, 0};
            byte[] csv_endFrame = new byte[] {
                1, 0x80, 0, 6,
                0, 0, 0, 36,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0xc8,
                0, 0, 0, 5,
                0, 0, 0, 0, 0, 0, 0, 20,
                0, 0, 0, 7,
                0, 0, 0, 0
            };

            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(csv_endFrame, 0, csv_endFrame.Length);

            content.Seek(0, SeekOrigin.Begin);

            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);

            Assert.AreEqual(200, result.Status);
            Assert.AreEqual(197, result.Offset);
            Assert.AreEqual(197, result.TotalScannedBytes);
            Assert.AreEqual(5, result.SplitsCount);
            Assert.AreEqual(20, result.RowsCount);
            Assert.AreEqual(7, result.ColumnsCount);

            //with message
            byte[] csv_endFrame_with_message = new byte[] {
                1, 0x80, 0, 6,
                0, 0, 0, 40,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0xc8,
                0, 0, 0, 5,
                0, 0, 0, 0, 0, 0, 0, 20,
                0, 0, 0, 8,
                97, 98, 99, 100,
                0, 0, 0, 0
            };
            content = new MemoryStream();
            content.Write(csv_endFrame_with_message, 0, csv_endFrame_with_message.Length);
            content.Seek(0, SeekOrigin.Begin);
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(200, result.Status);
            Assert.AreEqual(197, result.Offset);
            Assert.AreEqual(197, result.TotalScannedBytes);
            Assert.AreEqual(5, result.SplitsCount);
            Assert.AreEqual(20, result.RowsCount);
            Assert.AreEqual(8, result.ColumnsCount);
            Assert.AreEqual("abcd", result.ErrorMessage);

            //invalid crc
            byte[] csv_endFrame_with_invalid_crc = new byte[] {
                1, 0x80, 0, 6,
                0, 0, 0, 40,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0xc8,
                0, 0, 0, 5,
                0, 0, 0, 0, 0, 0, 0, 20,
                0, 0, 0, 8,
                97, 98, 99, 100,
                1, 0, 0, 0
            };
            content = new MemoryStream();
            content.Write(csv_endFrame_with_invalid_crc, 0, csv_endFrame_with_invalid_crc.Length);
            content.Seek(0, SeekOrigin.Begin);
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ClientException e)
            {
                Assert.IsTrue(e.Message.Contains("Payload checksum check fail server CRC"));
            }

            //json format
            byte[] json_endFrame_with_message = new byte[] {
                1, 0x80, 0, 7,
                0, 0, 0, 36,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0xc8,
                0, 0, 0, 6,
                0, 0, 0, 0, 0, 0, 0, 20,
                97, 98, 99, 100,
                0, 0, 0, 0
            };
            content = new MemoryStream();
            content.Write(json_endFrame_with_message, 0, json_endFrame_with_message.Length);
            content.Seek(0, SeekOrigin.Begin);
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(200, result.Status);
            Assert.AreEqual(197, result.Offset);
            Assert.AreEqual(197, result.TotalScannedBytes);
            Assert.AreEqual(6, result.SplitsCount);
            Assert.AreEqual(20, result.RowsCount);
            Assert.AreEqual(0, result.ColumnsCount);
            Assert.AreEqual("abcd", result.ErrorMessage);

            //invalid data format
            content = new MemoryStream(Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz"));
            headers = new Dictionary<string, string>();
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ClientException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            //without end format
            content = new MemoryStream();
            content.Write(continueFrame, 0, continueFrame.Length);
            content.Write(continueFrame, 0, continueFrame.Length);
            content.Seek(0, SeekOrigin.Begin);
            headers = new Dictionary<string, string>();
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ClientException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            //with long error message
            byte[] json_endFrame_with_long_message = new byte[] {
                1, 0x80, 0, 7,
                0, 0, 0, 164,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0, 0, 0, 0, 0xc5,
                0, 0, 0, 0xc9,
                0, 0, 0, 6,
                0, 0, 0, 0, 0, 0, 0, 20,
                97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100, 97, 98, 99, 100,
                0, 0, 0, 0
            };
            content = new MemoryStream();
            content.Write(json_endFrame_with_long_message, 0, json_endFrame_with_long_message.Length);
            content.Seek(0, SeekOrigin.Begin);
            headers = new Dictionary<string, string>();
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(201, result.Status);
            Assert.AreEqual(197, result.Offset);
            Assert.AreEqual(197, result.TotalScannedBytes);
            Assert.AreEqual(6, result.SplitsCount);
            Assert.AreEqual(20, result.RowsCount);
            Assert.AreEqual(0, result.ColumnsCount);
            Assert.AreEqual(128, result.ErrorMessage.Length);
        }
    }
}
