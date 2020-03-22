using System;
using System.Net;
using System.Text;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;
using Aliyun.OSS.Util;

using NUnit.Framework;
using System.IO;
using System.Collections.Generic;
using Aliyun.OSS.Model;
using System.Threading;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectVersioningTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();

            //get current class name, which is prefix of bucket/object
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName(_className);
            _ossClient.CreateBucket(_bucketName);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            cleanBucket(_bucketName);
        }

        [Test]
        public void ObjectBasicWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            //test put, get, head and getmeta 
            var content1 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
            var content2 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 2."));
            var etag1 = FileUtils.ComputeContentMd5(content1).ToUpperInvariant();
            var etag2 = FileUtils.ComputeContentMd5(content2).ToUpperInvariant();

            var key = OssTestUtils.GetObjectKey("ObjectBasicWithVersioningEnableTest");

            content1.Position = 0;
            var pResult = _ossClient.PutObject(_bucketName, key, content1);
            Assert.AreEqual(pResult.ETag, etag1);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId1 = pResult.VersionId;

            content2.Position = 0;
            pResult = _ossClient.PutObject(_bucketName, key, content2);
            Assert.AreEqual(pResult.ETag, etag2);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId2 = pResult.VersionId;

            Assert.AreNotEqual(versionId1, versionId2);
            Assert.AreNotEqual(etag1, etag2);

            //head 
            var request = new GetObjectMetadataRequest(_bucketName, key);
            var hResult = _ossClient.GetObjectMetadata(request);
            Assert.AreEqual(hResult.VersionId, versionId2);
            Assert.AreEqual(hResult.ETag, etag2);

            request.VersionId = versionId1;
            hResult = _ossClient.GetObjectMetadata(request);
            Assert.AreEqual(hResult.VersionId, versionId1);
            Assert.AreEqual(hResult.ETag, etag1);

            request.VersionId = versionId2;
            hResult = _ossClient.GetObjectMetadata(request);
            Assert.AreEqual(hResult.VersionId, versionId2);
            Assert.AreEqual(hResult.ETag, etag2);

            //Get
            var gRequest = new GetObjectRequest(_bucketName, key);
            var gResult = _ossClient.GetObject(gRequest);
            Assert.AreEqual(gResult.Metadata.VersionId, versionId2);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag2);

            gRequest.VersionId = versionId1;
            gResult = _ossClient.GetObject(gRequest);
            Assert.AreEqual(gResult.Metadata.VersionId, versionId1);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag1);

            gRequest.VersionId = versionId2;
            gResult = _ossClient.GetObject(gRequest);
            Assert.AreEqual(gResult.Metadata.VersionId, versionId2);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag2);

            var lResult = _ossClient.ListObjects(_bucketName, key);
            var objectSum = OssTestUtils.ToArray(lResult.ObjectSummaries);
            Assert.AreEqual(objectSum.Count, 1);

            var lvRequest = new ListObjectVersionsRequest(_bucketName);
            lvRequest.Prefix = key;
            var lvResult = _ossClient.ListObjectVersions(lvRequest);
            var objectVersionSum = OssTestUtils.ToArray(lvResult.ObjectVersionSummaries);
            Assert.AreEqual(objectVersionSum.Count, 2);

            //Delete
            var dRequest = new DeleteObjectRequest(_bucketName, key);
            var dResult = new DeleteObjectResult();
            Assert.AreEqual(dResult.DeleteMarker, false);
            dResult = _ossClient.DeleteObject(dRequest);
            Assert.AreNotEqual(dResult.VersionId.Length, 0);
            Assert.AreEqual(dResult.DeleteMarker, true);
            var dversionId = dResult.VersionId;

            //Get simple meta
            try
            {
                hResult = _ossClient.GetObjectMetadata(_bucketName, key);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            request = new GetObjectMetadataRequest(_bucketName, key);
            request.VersionId = versionId2;
            hResult = _ossClient.GetObjectMetadata(request);
            Assert.AreEqual(hResult.VersionId, versionId2);
            Assert.AreEqual(hResult.ETag, etag2);

            request.VersionId = versionId1;
            hResult = _ossClient.GetObjectMetadata(request);
            Assert.AreEqual(hResult.VersionId, versionId1);
            Assert.AreEqual(hResult.ETag, etag1);

            request.VersionId = versionId2;
            hResult = _ossClient.GetObjectMetadata(request);
            Assert.AreEqual(hResult.VersionId, versionId2);
            Assert.AreEqual(hResult.ETag, etag2);

            //list agian
            lResult = _ossClient.ListObjects(_bucketName, key);
            objectSum = OssTestUtils.ToArray(lResult.ObjectSummaries);
            Assert.AreEqual(objectSum.Count, 0);

            lvRequest = new ListObjectVersionsRequest(_bucketName);
            lvRequest.Prefix = key;
            lvResult = _ossClient.ListObjectVersions(lvRequest);
            objectVersionSum = OssTestUtils.ToArray(lvResult.ObjectVersionSummaries);
            var deleteMarkerSum = OssTestUtils.ToArray(lvResult.DeleteMarkerSummaries);
            Assert.AreEqual(objectVersionSum.Count, 2);
            Assert.AreEqual(deleteMarkerSum.Count, 1);

            //Get agian
            try
            {
                gRequest = new GetObjectRequest(_bucketName, key);
                gResult = _ossClient.GetObject(gRequest);
            } 
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "NoSuchKey");
            }

            gRequest.VersionId = versionId1;
            gResult = _ossClient.GetObject(gRequest);
            Assert.AreEqual(gResult.Metadata.VersionId, versionId1);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag1);

            gRequest.VersionId = versionId2;
            gResult = _ossClient.GetObject(gRequest);
            Assert.AreEqual(gResult.Metadata.VersionId, versionId2);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag2);

            //delete by version id
            dRequest.VersionId = dversionId;
            dResult = _ossClient.DeleteObject(dRequest);
            Assert.AreEqual(dResult.VersionId, dversionId);
            Assert.AreEqual(dResult.DeleteMarker, true);

            hResult = _ossClient.GetObjectMetadata(request);
            Assert.AreEqual(hResult.VersionId, versionId2);

            dRequest.VersionId = versionId1;
            dResult = _ossClient.DeleteObject(dRequest);
            Assert.AreEqual(dResult.VersionId, versionId1);
            Assert.AreEqual(dResult.DeleteMarker, false);

            dRequest.VersionId = versionId2;
            dResult = _ossClient.DeleteObject(dRequest);
            Assert.AreEqual(dResult.VersionId, versionId2);
            Assert.AreEqual(dResult.DeleteMarker, false);

            //list again
            lResult = _ossClient.ListObjects(_bucketName, key);
            objectSum = OssTestUtils.ToArray(lResult.ObjectSummaries);
            Assert.AreEqual(objectSum.Count, 0);

            lvRequest = new ListObjectVersionsRequest(_bucketName);
            lvRequest.Prefix = key;
            lvResult = _ossClient.ListObjectVersions(lvRequest);
            objectVersionSum = OssTestUtils.ToArray(lvResult.ObjectVersionSummaries);
            deleteMarkerSum = OssTestUtils.ToArray(lvResult.DeleteMarkerSummaries);
            Assert.AreEqual(objectVersionSum.Count, 0);
            Assert.AreEqual(deleteMarkerSum.Count, 0);
        }

        [Test]
        public void ObjectAclWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            //test put, get, head and getmeta 
            var content1 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
            var content2 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 2."));
            var etag1 = FileUtils.ComputeContentMd5(content1).ToUpperInvariant();
            var etag2 = FileUtils.ComputeContentMd5(content2).ToUpperInvariant();
            var key = OssTestUtils.GetObjectKey("ObjectBasicWithVersioningEnableTest");

            content1.Position = 0;
            var meta = new ObjectMetadata();
            meta.AddHeader("x-oss-object-acl", "private");
            var pRequest = new PutObjectRequest(_bucketName, key, content1, meta);
            var pResult = _ossClient.PutObject(pRequest);
            Assert.AreEqual(pResult.ETag, etag1);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId1 = pResult.VersionId;

            content2.Position = 0;
            pResult = _ossClient.PutObject(_bucketName, key, content2);
            Assert.AreEqual(pResult.ETag, etag2);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId2 = pResult.VersionId;

            Assert.AreNotEqual(versionId1, versionId2);
            Assert.AreNotEqual(etag1, etag2);

            var gaRequest = new GetObjectAclRequest(_bucketName, key);
            var gaResult = _ossClient.GetObjectAcl(gaRequest);
            Assert.AreEqual(gaResult.ACL, CannedAccessControlList.Default);

            gaRequest.VersionId = versionId1;
            gaResult = _ossClient.GetObjectAcl(gaRequest);
            Assert.AreEqual(gaResult.ACL, CannedAccessControlList.Private);

            gaRequest.VersionId = versionId2;
            gaResult = _ossClient.GetObjectAcl(gaRequest);
            Assert.AreEqual(gaResult.ACL, CannedAccessControlList.Default);


            //setAcl
            var saRequest = new SetObjectAclRequest(_bucketName, key, CannedAccessControlList.PublicRead);
            _ossClient.SetObjectAcl(saRequest);

            gaResult = _ossClient.GetObjectAcl(new GetObjectAclRequest(_bucketName, key));
            Assert.AreEqual(gaResult.ACL, CannedAccessControlList.PublicRead);

            saRequest = new SetObjectAclRequest(_bucketName, key, CannedAccessControlList.PublicReadWrite)
            {
                VersionId = versionId1
            };
            _ossClient.SetObjectAcl(saRequest);


            gaRequest = new GetObjectAclRequest(_bucketName, key)
            {
                VersionId = versionId1
            };

            gaResult = _ossClient.GetObjectAcl(gaRequest);
            Assert.AreEqual(gaResult.ACL, CannedAccessControlList.PublicReadWrite);
        }


        [Test]
        public void ObjectSymlinkWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            //test put, get, head and getmeta 
            var content1 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
            var content2 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 2."));
            var etag1 = FileUtils.ComputeContentMd5(content1).ToUpperInvariant();
            var etag2 = FileUtils.ComputeContentMd5(content2).ToUpperInvariant();
            var target1 = OssTestUtils.GetObjectKey("ObjectSymlinkWithVersioningEnableTest-1");
            var target2 = OssTestUtils.GetObjectKey("ObjectSymlinkWithVersioningEnableTest-2");
            var key = target1 + "-link";

            //1
            content1.Position = 0;
            var pResult = _ossClient.PutObject(_bucketName, target1, content1);
            Assert.AreEqual(pResult.ETag, etag1);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId1 = pResult.VersionId;

            var cRequest = new CreateSymlinkRequest(_bucketName, key, target1);
            var cResult = _ossClient.CreateSymlink(cRequest);
            //Assert.AreEqual(cResult.ETag, etag1);
            Assert.AreNotEqual(cResult.VersionId, 0);
            var sversionId1 = cResult.VersionId;


            //2
            content2.Position = 0;
            pResult = _ossClient.PutObject(_bucketName, target2, content2);
            Assert.AreEqual(pResult.ETag, etag2);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId2 = pResult.VersionId;

            cRequest = new CreateSymlinkRequest(_bucketName, key, target2);
            cResult = _ossClient.CreateSymlink(cRequest);
            //Assert.AreEqual(cResult.ETag, etag2);
            Assert.AreNotEqual(cResult.VersionId, 0);
            var sversionId2 = cResult.VersionId;


            //Get
            var gsRequest = new GetSymlinkRequest(_bucketName, key);
            var gsResult = _ossClient.GetSymlink(gsRequest);
            Assert.AreEqual(gsResult.ObjectMetadata.VersionId, sversionId2);
            Assert.AreEqual(gsResult.Target, target2);

            gsRequest.VersionId = sversionId1;
            gsResult = _ossClient.GetSymlink(gsRequest);
            Assert.AreEqual(gsResult.ObjectMetadata.VersionId, sversionId1);
            Assert.AreEqual(gsResult.Target, target1);

            gsRequest.VersionId = sversionId2;
            gsResult = _ossClient.GetSymlink(gsRequest);
            Assert.AreEqual(gsResult.ObjectMetadata.VersionId, sversionId2);
            Assert.AreEqual(gsResult.Target, target2);

            var gRequest = new GetObjectRequest(_bucketName, key);
            var gResult = _ossClient.GetObject(gRequest);
            Assert.AreEqual(gResult.Metadata.VersionId, sversionId2);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag2);

            gRequest.VersionId = sversionId1;
            gResult = _ossClient.GetObject(gRequest);
            Assert.AreEqual(gResult.Metadata.VersionId, sversionId1);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag1);

            gRequest.VersionId = sversionId2;
            gResult = _ossClient.GetObject(gRequest);
            Assert.AreEqual(gResult.Metadata.VersionId, sversionId2);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag2);
        }

        [Test]
        public void AppendObjectWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;


            var content1 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
            var content2 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 2."));
            var key = OssTestUtils.GetObjectKey("AppendObjectWithVersioningEnableTest");

            var aRequest = new AppendObjectRequest(_bucketName, key)
            {
                Content = content1,
                Position = 0
            };

            var aResult = _ossClient.AppendObject(aRequest);
            Assert.AreEqual(aResult.VersionId, "null");

            aRequest = new AppendObjectRequest(_bucketName, key)
            {
                Content = content2,
                Position = aResult.NextAppendPosition
            };
            aResult = _ossClient.AppendObject(aRequest);
            Assert.AreEqual(aResult.VersionId, "null");
        }

        [Test]
        public void RestoreObjectWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            var content1 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
            var content2 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 2."));
            var key = OssTestUtils.GetObjectKey("RestoreObjectWithVersioningEnableTest");
            var etag1 = FileUtils.ComputeContentMd5(content1).ToUpperInvariant();
            var etag2 = FileUtils.ComputeContentMd5(content2).ToUpperInvariant();

            content1.Position = 0;
            var meta = new ObjectMetadata();
            meta.AddHeader("x-oss-storage-class", "Archive");
            var pRequest = new PutObjectRequest(_bucketName, key, content1, meta);
            var pResult = _ossClient.PutObject(pRequest);
            Assert.AreEqual(pResult.ETag, etag1);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId1 = pResult.VersionId;

            content2.Position = 0;
            pResult = _ossClient.PutObject(_bucketName, key, content2, meta);
            Assert.AreEqual(pResult.ETag, etag2);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId2 = pResult.VersionId;

            Assert.AreNotEqual(versionId1, versionId2);
            Assert.AreNotEqual(etag1, etag2);

            var rRequest = new RestoreObjectRequest(_bucketName, key);
            var rResult = _ossClient.RestoreObject(rRequest);
            Assert.AreEqual(rResult.VersionId, versionId2);

            rRequest.VersionId = versionId1;
            rResult = _ossClient.RestoreObject(rRequest);
            Assert.AreEqual(rResult.VersionId, versionId1);
        }

        [Test]
        public void CopyObjectWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            var content1 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
            var content2 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 2."));
            var key = OssTestUtils.GetObjectKey("CopyObjectWithVersioningEnableTest");
            var etag1 = FileUtils.ComputeContentMd5(content1).ToUpperInvariant();
            var etag2 = FileUtils.ComputeContentMd5(content2).ToUpperInvariant();

            content1.Position = 0;
            var pRequest = new PutObjectRequest(_bucketName, key, content1);
            var pResult = _ossClient.PutObject(pRequest);
            Assert.AreEqual(pResult.ETag, etag1);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId1 = pResult.VersionId;

            content2.Position = 0;
            pResult = _ossClient.PutObject(_bucketName, key, content2);
            Assert.AreEqual(pResult.ETag, etag2);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId2 = pResult.VersionId;

            Assert.AreNotEqual(versionId1, versionId2);
            Assert.AreNotEqual(etag1, etag2);

            var cRequest = new CopyObjectRequest(_bucketName, key, _bucketName, key);
            var cResult = _ossClient.CopyObject(cRequest);
            Assert.AreEqual(cResult.CopySourceVersionId, versionId2);
            Assert.AreNotEqual(cResult.VersionId.Length, 0);
            var versionId3 = cResult.VersionId;

            cRequest = new CopyObjectRequest(_bucketName, key, _bucketName, key);
            cRequest.SourceVersionId = versionId1;
            cResult = _ossClient.CopyObject(cRequest);
            Assert.AreEqual(cResult.CopySourceVersionId, versionId1);
            Assert.AreNotEqual(cResult.VersionId.Length, 0);
            var versionId4 = cResult.VersionId;

            var gRequest = new GetObjectRequest(_bucketName, key);
            gRequest.VersionId = versionId3;
            var gResult = _ossClient.GetObject(gRequest);
            Assert.AreEqual(gResult.Metadata.VersionId, versionId3);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag2);

            gRequest.VersionId = versionId4;
            gResult = _ossClient.GetObject(gRequest);
            Assert.AreEqual(gResult.Metadata.VersionId, versionId4);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag1);
        }

        [Test]
        public void ObjectTaggingWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            var content1 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
            var content2 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 2."));
            var key = OssTestUtils.GetObjectKey("ObjectTaggingWithVersioningEnableTest");
            var etag1 = FileUtils.ComputeContentMd5(content1).ToUpperInvariant();
            var etag2 = FileUtils.ComputeContentMd5(content2).ToUpperInvariant();

            var tag1 = new Tag { Key ="key1", Value = "value1" };
            var tag2 = new Tag { Key = "key2", Value = "value2" };
            var tag3 = new Tag { Key = "key3", Value = "value3" };

            content1.Position = 0;
            var meta = new ObjectMetadata();
            meta.AddHeader("x-oss-tagging", "key1=value1&key2=value2");
            var pRequest = new PutObjectRequest(_bucketName, key, content1, meta);
            var pResult = _ossClient.PutObject(pRequest);
            Assert.AreEqual(pResult.ETag, etag1);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId1 = pResult.VersionId;

            content2.Position = 0;
            pResult = _ossClient.PutObject(_bucketName, key, content2);
            Assert.AreEqual(pResult.ETag, etag2);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId2 = pResult.VersionId;

            Assert.AreNotEqual(versionId1, versionId2);
            Assert.AreNotEqual(etag1, etag2);

            //get object tagging
            var gotRequest = new GetObjectTaggingRequest(_bucketName, key);
            var gotResult = _ossClient.GetObjectTagging(gotRequest);
            Assert.AreEqual(gotResult.Tags.Count, 0);
            Assert.AreEqual(gotResult.VersionId, versionId2);

            gotRequest.VersionId = versionId1;
            gotResult = _ossClient.GetObjectTagging(gotRequest);
            Assert.AreEqual(gotResult.VersionId, versionId1);
            Assert.AreEqual(gotResult.Tags.Count, 2);
            Assert.AreEqual(gotResult.Tags[0].Equals(tag1), true);
            Assert.AreEqual(gotResult.Tags[1].Equals(tag2), true);

            //set
            var sotRequest = new SetObjectTaggingRequest(_bucketName, key);
            sotRequest.VersionId = versionId1;
            sotRequest.AddTag(tag1);
            sotRequest.AddTag(tag2);
            sotRequest.AddTag(tag3);
            _ossClient.SetObjectTagging(sotRequest);

            gotRequest.VersionId = versionId2;
            gotResult = _ossClient.GetObjectTagging(gotRequest);
            Assert.AreEqual(gotResult.Tags.Count, 0);
            Assert.AreEqual(gotResult.VersionId, versionId2);

            gotRequest.VersionId = versionId1;
            gotResult = _ossClient.GetObjectTagging(gotRequest);
            Assert.AreEqual(gotResult.VersionId, versionId1);
            Assert.AreEqual(gotResult.Tags.Count, 3);
            Assert.AreEqual(gotResult.Tags[0].Equals(tag1), true);
            Assert.AreEqual(gotResult.Tags[1].Equals(tag2), true);
            Assert.AreEqual(gotResult.Tags[2].Equals(tag3), true);

            //delete
            var dotRequst = new DeleteObjectTaggingRequest(_bucketName, key);
            dotRequst.VersionId = versionId1;
            _ossClient.DeleteObjectTagging(dotRequst);

            gotRequest.VersionId = versionId1;
            gotResult = _ossClient.GetObjectTagging(gotRequest);
            Assert.AreEqual(gotResult.Tags.Count, 0);
            Assert.AreEqual(gotResult.VersionId, versionId1);
        }

        [Test]
        public void MultipartWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            var content1 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
            var content2 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 2."));
            var etag1 = FileUtils.ComputeContentMd5(content1).ToUpperInvariant();
            var etag2 = FileUtils.ComputeContentMd5(content2).ToUpperInvariant();

            var key = OssTestUtils.GetObjectKey("MultipartWithVersioningEnableTest-basic");
            var key1 = OssTestUtils.GetObjectKey("MultipartWithVersioningEnableTest-multi-1");
            var key2 = OssTestUtils.GetObjectKey("MultipartWithVersioningEnableTest-multi-2");

            content1.Position = 0;
            var pRequest = new PutObjectRequest(_bucketName, key, content1);
            var pResult = _ossClient.PutObject(pRequest);
            Assert.AreEqual(pResult.ETag, etag1);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId1 = pResult.VersionId;

            content2.Position = 0;
            pResult = _ossClient.PutObject(_bucketName, key, content2);
            Assert.AreEqual(pResult.ETag, etag2);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId2 = pResult.VersionId;

            Assert.AreNotEqual(versionId1, versionId2);
            Assert.AreNotEqual(etag1, etag2);

            var initResult = _ossClient.InitiateMultipartUpload(new InitiateMultipartUploadRequest(_bucketName, key1));
            Assert.AreNotEqual(initResult.UploadId.Length, 0);

            var ucRequest = new UploadPartCopyRequest(_bucketName, key1, _bucketName, key, initResult.UploadId);
            ucRequest.VersionId = versionId1;
            ucRequest.PartNumber = 1;
            ucRequest.BeginIndex = 0;
            ucRequest.PartSize = 18;
            var ucResult = _ossClient.UploadPartCopy(ucRequest);
            Assert.AreEqual(ucResult.CopySourceVersionId, versionId1);

            var cmRequest = new  CompleteMultipartUploadRequest(_bucketName, key1, initResult.UploadId);
            cmRequest.PartETags.Add(new PartETag(1, ucResult.ETag));
            var cmResult = _ossClient.CompleteMultipartUpload(cmRequest);
            Assert.AreNotEqual(cmResult.VersionId.Length, 0);
            var mversion1 = cmResult.VersionId;


            initResult = _ossClient.InitiateMultipartUpload(new InitiateMultipartUploadRequest(_bucketName, key2));
            Assert.AreNotEqual(initResult.UploadId.Length, 0);

            ucRequest = new UploadPartCopyRequest(_bucketName, key2, _bucketName, key, initResult.UploadId);
            ucRequest.PartNumber = 1;
            ucRequest.BeginIndex = 0;
            ucRequest.PartSize = 18;
            ucResult = _ossClient.UploadPartCopy(ucRequest);
            Assert.AreEqual(ucResult.CopySourceVersionId, versionId2);

            cmRequest = new CompleteMultipartUploadRequest(_bucketName, key2, initResult.UploadId);
            cmRequest.PartETags.Add(new PartETag(1, ucResult.ETag));
            cmResult = _ossClient.CompleteMultipartUpload(cmRequest);
            Assert.AreNotEqual(cmResult.VersionId.Length, 0);
            var mversion2 = cmResult.VersionId;

            var gResult = _ossClient.GetObject(_bucketName, key1);
            Assert.AreEqual(gResult.Metadata.VersionId, mversion1);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag1);

            gResult = _ossClient.GetObject(_bucketName, key2);
            Assert.AreEqual(gResult.Metadata.VersionId, mversion2);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag2);
        }

        [Test]
        public void DeleteObjecsWithVersioningEnableTest()
        {
            var bucketName = _bucketName + "-objects";

            _ossClient.CreateBucket(bucketName);

            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            //ObjectIdentifier
            var objectId = new ObjectIdentifier("key");
            Assert.AreEqual(objectId.Key, "key");
            Assert.AreEqual(objectId.VersionId, "");
            objectId = new ObjectIdentifier("key", "version");
            Assert.AreEqual(objectId.Key, "key");
            Assert.AreEqual(objectId.VersionId, "version");

            var keyPrefix = OssTestUtils.GetObjectKey("DeleteObjecsWithVersioningEnableTest");

            var keyList = new List<string>();
            var versionIdList = new List<string>();
            var deletedVersionIdList = new List<string>();

            //put objects
            for (int i = 0; i < 10; i++)
            {
                var key = keyPrefix + "-" +i.ToString();
                var content = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
                var pResult = _ossClient.PutObject(new PutObjectRequest(bucketName, key, content));
                keyList.Add(key);
                versionIdList.Add(pResult.VersionId);
            }

            //speical key
            {
                var key = keyPrefix + "-" + keyList.Count.ToString() + "\"&\'<>-";
                var content = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
                var pResult = _ossClient.PutObject(new PutObjectRequest(bucketName, key, content));
                keyList.Add(key);
                versionIdList.Add(pResult.VersionId);
            }

            //delete objects without version id
            var objectList = new List<ObjectIdentifier>();
            for (int i = 0; i < keyList.Count - 1; i++)
            {
                objectList.Add(new ObjectIdentifier(keyList[i]));
            }
            var dsRequest = new DeleteObjectVersionsRequest(bucketName, objectList, false);
            dsRequest.Objects.Add(new ObjectIdentifier(keyList[keyList.Count - 1]));

            var dsResult = _ossClient.DeleteObjectVersions(dsRequest);
            var deleteSum = OssTestUtils.ToArray(dsResult.DeletedObjectSummaries);
            Assert.AreEqual(deleteSum.Count, keyList.Count);
            
            for (int i = 0; i < keyList.Count; i++) {
                Assert.AreEqual(deleteSum[i].Key, keyList[i]);
                Assert.AreEqual(deleteSum[i].DeleteMarker,true);
                Assert.AreNotEqual(deleteSum[i].DeleteMarkerVersionId, null);
                Assert.AreEqual(deleteSum[i].VersionId, null);
                Assert.AreEqual(deleteSum[i].Key, keyList[i]);
                deletedVersionIdList.Add(deleteSum[i].DeleteMarkerVersionId);
            }

            var lsResult = _ossClient.ListObjects(bucketName);
            var objectsSum = OssTestUtils.ToArray(lsResult.ObjectSummaries);
            Assert.AreEqual(objectsSum.Count, 0);

            var lsvResult = _ossClient.ListObjectVersions(new ListObjectVersionsRequest(bucketName));
            var objectVersionSum = OssTestUtils.ToArray(lsvResult.ObjectVersionSummaries);
            var deleteMarkerSum = OssTestUtils.ToArray(lsvResult.DeleteMarkerSummaries);
            Assert.AreEqual(objectVersionSum.Count, keyList.Count);
            Assert.AreEqual(deleteMarkerSum.Count, keyList.Count);

            //deletes objects with version id
            objectList = new List<ObjectIdentifier>();
            for (int i = 0; i < keyList.Count; i++)
            {
                objectList.Add(new ObjectIdentifier(keyList[i], versionIdList[i]));
            }
            dsRequest = new DeleteObjectVersionsRequest(bucketName, objectList, false);
            dsResult = _ossClient.DeleteObjectVersions(dsRequest);

            deleteSum = OssTestUtils.ToArray(dsResult.DeletedObjectSummaries);
            Assert.AreEqual(deleteSum.Count, keyList.Count);
            for (int i = 0; i < keyList.Count; i++)
            {
                Assert.AreEqual(deleteSum[i].Key, keyList[i]);
                Assert.AreEqual(deleteSum[i].DeleteMarker, false);
                Assert.AreEqual(deleteSum[i].DeleteMarkerVersionId, null);
                Assert.AreEqual(deleteSum[i].VersionId, versionIdList[i]);
            }

            lsvResult = _ossClient.ListObjectVersions(new ListObjectVersionsRequest(bucketName));
            objectVersionSum = OssTestUtils.ToArray(lsvResult.ObjectVersionSummaries);
            deleteMarkerSum = OssTestUtils.ToArray(lsvResult.DeleteMarkerSummaries);
            Assert.AreEqual(objectVersionSum.Count, 0);
            Assert.AreEqual(deleteMarkerSum.Count, keyList.Count);

            //deletes delete markers with version id
            objectList = new List<ObjectIdentifier>();
            for (int i = 0; i < keyList.Count; i++)
            {
                objectList.Add(new ObjectIdentifier(keyList[i], deletedVersionIdList[i]));
            }
            dsRequest = new DeleteObjectVersionsRequest(bucketName, objectList, false);
            dsRequest.EncodingType = "url";
            dsResult = _ossClient.DeleteObjectVersions(dsRequest);

            deleteSum = OssTestUtils.ToArray(dsResult.DeletedObjectSummaries);
            Assert.AreEqual(deleteSum.Count, keyList.Count);
            for (int i = 0; i < keyList.Count; i++)
            {
                Assert.AreEqual(deleteSum[i].Key, keyList[i]);
                Assert.AreEqual(deleteSum[i].DeleteMarker, true);
                Assert.AreEqual(deleteSum[i].DeleteMarkerVersionId, deletedVersionIdList[i]);
                Assert.AreEqual(deleteSum[i].VersionId, deletedVersionIdList[i]);
            }

            lsvResult = _ossClient.ListObjectVersions(new ListObjectVersionsRequest(bucketName));
            objectVersionSum = OssTestUtils.ToArray(lsvResult.ObjectVersionSummaries);
            deleteMarkerSum = OssTestUtils.ToArray(lsvResult.DeleteMarkerSummaries);
            Assert.AreEqual(objectVersionSum.Count, 0);
            Assert.AreEqual(deleteMarkerSum.Count, 0);

            cleanBucket(bucketName);
        }

        [Test]
        public void ListObjecsWithVersioningEnableTest()
        {
            var bucketName = _bucketName + "-lsobjects";

            _ossClient.CreateBucket(bucketName);

            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            var keyPrefix = OssTestUtils.GetObjectKey("ListObjecsWithVersioningEnableTest");

            //Build Test Data
            int normalCnt = 10;
            var normalKeys = new List<string>();
            for (int i = 0; i < normalCnt; i++)
            {
                var key = keyPrefix + "/normal/" + i.ToString() + ".dat";
                normalKeys.Add(key);

                _ossClient.PutObject(new PutObjectRequest(bucketName, key, 
                    new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."))));

                _ossClient.DeleteObject(bucketName, key);

                _ossClient.PutObject(new PutObjectRequest(bucketName, key,
                    new MemoryStream(Encoding.ASCII.GetBytes("versioning test 2."))));

                _ossClient.PutObject(new PutObjectRequest(bucketName, key,
                    new MemoryStream(Encoding.ASCII.GetBytes("versioning test 3."))));
            }

            int xmlescapeCnt = 8;
            var xmlescapeKeys = new List<string>();
            for (int i = 0; i < xmlescapeCnt; i++)
            {
                var key = keyPrefix + "/xmlescape/" + i.ToString() + ".\"&\'<>-.dat";
                xmlescapeKeys.Add(key);

                _ossClient.PutObject(new PutObjectRequest(bucketName, key,
                    new MemoryStream(Encoding.ASCII.GetBytes("versioning test xmlescape 1."))));

                _ossClient.PutObject(new PutObjectRequest(bucketName, key,
                    new MemoryStream(Encoding.ASCII.GetBytes("versioning test xmlescape 2."))));
            }

            int hiddenCharsCnt = 5;
            var hiddenCharsKeys = new List<string>();
            for (int i = 0; i < hiddenCharsCnt; i++)
            {
                var hiddenCharByte = new byte[] { 0x1c, 0x1a };

                var key = keyPrefix + "/hiddenchar/" + System.Text.Encoding.UTF8.GetString(hiddenCharByte) 
                    + i.ToString() + ".dat";
                hiddenCharsKeys.Add(key);

                _ossClient.PutObject(new PutObjectRequest(bucketName, key,
                    new MemoryStream(Encoding.ASCII.GetBytes("versioning test hiddenchar 1."))));
            }

            //Test Marker
            var lovRequest = new ListObjectVersionsRequest(bucketName)
            {
                MaxKeys = 1
            };
            bool IsTruncated = false;
            int total = 0;
            do
            {
                var result = _ossClient.ListObjectVersions(lovRequest);
                var objectSum = OssTestUtils.ToArray(result.ObjectVersionSummaries);
                var deleteSum = OssTestUtils.ToArray(result.DeleteMarkerSummaries);
                var prefixSum = OssTestUtils.ToArray(result.CommonPrefixes);

                Assert.AreEqual(objectSum.Count + deleteSum.Count, 1);
                Assert.AreEqual(prefixSum.Count, 0);

                IsTruncated = result.IsTruncated;
                lovRequest.KeyMarker = result.NextKeyMarker;
                lovRequest.VersionIdMarker = result.NextVersionIdMarker;
                total++;
            } while (IsTruncated);

            Assert.AreEqual(total, (normalCnt * 4 + xmlescapeCnt * 2 + hiddenCharsCnt));


            //Test Delimiter
            lovRequest = new ListObjectVersionsRequest(bucketName)
            {
                MaxKeys = 1000,
                Prefix = keyPrefix + "/",
                Delimiter = "/"
            };
            { 
                var result = _ossClient.ListObjectVersions(lovRequest);
                var objectSum = OssTestUtils.ToArray(result.ObjectVersionSummaries);
                var deleteSum = OssTestUtils.ToArray(result.DeleteMarkerSummaries);
                var prefixSum = OssTestUtils.ToArray(result.CommonPrefixes);
                Assert.AreEqual(objectSum.Count + deleteSum.Count, 0);
                Assert.AreEqual(prefixSum.Count, 3);
                Assert.AreEqual(prefixSum[0], keyPrefix + "/hiddenchar/");
                Assert.AreEqual(prefixSum[1], keyPrefix + "/normal/");
                Assert.AreEqual(prefixSum[2], keyPrefix + "/xmlescape/");
            }

            //Test UrlEncoding
            lovRequest = new ListObjectVersionsRequest(bucketName)
            {
                MaxKeys = 1000,
                Prefix = keyPrefix + "/hiddenchar/",
                EncodingType = "url"
            };
            {
                var result = _ossClient.ListObjectVersions(lovRequest);
                var objectSum = OssTestUtils.ToArray(result.ObjectVersionSummaries);
                var deleteSum = OssTestUtils.ToArray(result.DeleteMarkerSummaries);
                var prefixSum = OssTestUtils.ToArray(result.CommonPrefixes);
                Assert.AreEqual(objectSum.Count, hiddenCharsKeys.Count);
                for (int i = 0; i < objectSum.Count; i++)
                {
                    Assert.AreEqual(objectSum[i].Key, hiddenCharsKeys[i]);
                }
            }

            //Test UrlEncoding
            lovRequest = new ListObjectVersionsRequest(bucketName)
            {
                MaxKeys = 1000,
                Prefix = keyPrefix + "/xmlescape/",
                EncodingType = ""
            };
            {
                var result = _ossClient.ListObjectVersions(lovRequest);
                var objectSum = OssTestUtils.ToArray(result.ObjectVersionSummaries);
                var deleteSum = OssTestUtils.ToArray(result.DeleteMarkerSummaries);
                var prefixSum = OssTestUtils.ToArray(result.CommonPrefixes);
                Assert.AreEqual(objectSum.Count, xmlescapeKeys.Count * 2);
            }

            cleanBucket(bucketName);
        }

        [Test]
        public void SignUrlWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            //test put, get, head and getmeta 
            var content1 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 1."));
            var content2 = new MemoryStream(Encoding.ASCII.GetBytes("versioning test 2."));
            var etag1 = FileUtils.ComputeContentMd5(content1).ToUpperInvariant();
            var etag2 = FileUtils.ComputeContentMd5(content2).ToUpperInvariant();

            var key = OssTestUtils.GetObjectKey("SignUrlWithVersioningEnableTest");

            content1.Position = 0;
            var pResult = _ossClient.PutObject(_bucketName, key, content1);
            Assert.AreEqual(pResult.ETag, etag1);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId1 = pResult.VersionId;

            content2.Position = 0;
            pResult = _ossClient.PutObject(_bucketName, key, content2);
            Assert.AreEqual(pResult.ETag, etag2);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId2 = pResult.VersionId;

            Assert.AreNotEqual(versionId1, versionId2);
            Assert.AreNotEqual(etag1, etag2);

            var urlRequest = new GeneratePresignedUriRequest(_bucketName, key, SignHttpMethod.Get);
            var urlResult = _ossClient.GeneratePresignedUri(urlRequest);
            Assert.AreEqual(urlRequest.QueryParams.ContainsKey("versionId"), false);

            var gResult = _ossClient.GetObject(urlResult);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag2);

            urlRequest = new GeneratePresignedUriRequest(_bucketName, key, SignHttpMethod.Get);
            urlRequest.AddQueryParam("versionId", versionId1);
            urlResult = _ossClient.GeneratePresignedUri(urlRequest);
            Assert.AreEqual(urlRequest.QueryParams.Contains(new KeyValuePair<string, string>("versionId", versionId1)), true);
            gResult = _ossClient.GetObject(urlResult);
            Assert.AreEqual(FileUtils.ComputeContentMd5(gResult.Content).ToUpperInvariant(), etag1);
        }

        [Test]
        public void ObjectOperationWithoutVersioningTest()
        {
            var bucketName = _bucketName + "-noversion";
            _ossClient.CreateBucket(bucketName);

            var key = OssTestUtils.GetObjectKey("ObjectOperationWithoutVersioningTest");
            var content = new MemoryStream(Encoding.ASCII.GetBytes("versioning test"));

            var pResult = _ossClient.PutObject(bucketName, key, content);
            Assert.AreEqual(pResult.VersionId, null);

            var gRequest = new GetObjectRequest(bucketName, key);
            Assert.AreEqual(gRequest.VersionId, null);
            var gResult = _ossClient.GetObject(bucketName, key);
            Assert.AreEqual(gResult.Metadata.VersionId, null);

            var gmRequest = new GetObjectMetadataRequest(bucketName, key);
            Assert.AreEqual(gmRequest.VersionId, null);
            var gmResult = _ossClient.GetObjectMetadata(bucketName, key);
            Assert.AreEqual(gResult.Metadata.VersionId, null);

            var key_link = key +  "-link";
            var csResult = _ossClient.CreateSymlink(bucketName, key_link, key);
            Assert.AreEqual(csResult.VersionId, null);

            var key_copy = key + "-copy";
            var coRequest = new CopyObjectRequest(bucketName, key, bucketName, key_link);
            Assert.AreEqual(coRequest.SourceVersionId, null);
            var coResult = _ossClient.CopyObject(coRequest);
            Assert.AreEqual(coResult.VersionId, null);

            var key_append = key + "-append";
            var aoRequest = new AppendObjectRequest(bucketName, key_append)
            {
                Content = new MemoryStream(Encoding.ASCII.GetBytes("versioning test"))
            };
            var aResult = _ossClient.AppendObject(aoRequest);
            Assert.AreEqual(aResult.VersionId, null);

            var dRequest = new DeleteObjectRequest(bucketName, key);
            Assert.AreEqual(dRequest.VersionId, null);
            var dResult = _ossClient.DeleteObject(dRequest);
            Assert.AreEqual(dResult.VersionId, null);

            var saRequest = new SetObjectAclRequest(bucketName, key, CannedAccessControlList.Private);
            Assert.AreEqual(saRequest.VersionId, null);

            var gaRequest = new GetObjectAclRequest(bucketName, key);
            Assert.AreEqual(gaRequest.VersionId, null);

            var sotRequest = new SetObjectTaggingRequest(bucketName, key);
            Assert.AreEqual(sotRequest.VersionId, null);

            var gotRequest = new GetObjectTaggingRequest(bucketName, key);
            Assert.AreEqual(gotRequest.VersionId, null);

            var dotRequest = new DeleteObjectTaggingRequest(bucketName, key);
            Assert.AreEqual(dotRequest.VersionId, null);

            cleanBucket(bucketName);
        }

        [Test]
        public void ResumableUploadWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            var fileName = Config.UploadTestFile;
            var fileInfo = new FileInfo(fileName);
            var fileSize = fileInfo.Length;

            //  < PartSize
            var key = OssTestUtils.GetObjectKey("ResumableUploadWithVersioningEnableTest");

            var request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = fileSize + 1,
            };

            var result = _ossClient.ResumableUploadObject(request);
            Assert.IsTrue(result.ETag.Length > 0);
            Assert.AreNotEqual(result.VersionId.Length, 0);
            var versionId1 = result.VersionId;
            var headResult = _ossClient.GetObjectMetadata(_bucketName, key);
            Assert.AreEqual(headResult.ContentLength, fileSize);

            // > PartSize with multi thread
            key = OssTestUtils.GetObjectKey(_className);

            request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = 100 * 1024,
            };

            result = _ossClient.ResumableUploadObject(request);
            Assert.IsTrue(result.ETag.Length > 0);
            Assert.AreNotEqual(result.VersionId.Length, 0);
            var versionId2 = result.VersionId;
            headResult = _ossClient.GetObjectMetadata(_bucketName, key);
            Assert.AreEqual(headResult.ContentLength, fileSize);

            // > PartSize with single thread
            key = OssTestUtils.GetObjectKey(_className);

            request = new UploadObjectRequest(_bucketName, key, fileName)
            {
                PartSize = 100 * 1024,
                ParallelThreadCount = 1
            };

            result = _ossClient.ResumableUploadObject(request);
            Assert.IsTrue(result.ETag.Length > 0);
            Assert.AreNotEqual(result.VersionId.Length, 0);
            var versionId3 = result.VersionId;
            headResult = _ossClient.GetObjectMetadata(_bucketName, key);
            Assert.AreEqual(headResult.ContentLength, fileSize);

            Assert.AreNotEqual(versionId1, versionId2);
            Assert.AreNotEqual(versionId1, versionId3);
            Assert.AreNotEqual(versionId2, versionId3);
        }

        [Test]
        public void ResumableDownloadWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            var key = OssTestUtils.GetObjectKey(_className);

            var fileName = Config.UploadTestFile;
            var fileInfo = new FileInfo(fileName);
            var fileSize = fileInfo.Length;

            var pResult = _ossClient.PutObject(_bucketName, key, fileName);
            Assert.AreNotEqual(pResult.VersionId.Length, 0);
            var versionId = pResult.VersionId;
            _ossClient.DeleteObject(_bucketName, key);

            //  < PartSize
            var request = new DownloadObjectRequest(_bucketName, key, targetFile)
            {
                PartSize = fileSize + 1,
                CheckpointDir = Config.DownloadFolder
            };
            try
            {
                _ossClient.ResumableDownloadObject(request);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
            request.VersionId = versionId;
            var metadata = _ossClient.ResumableDownloadObject(request);
            var expectedETag = metadata.ETag;
            var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
            Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            FileUtils.DeleteFile(targetFile);


            // > PartSize with multi thread
            request = new DownloadObjectRequest(_bucketName, key, targetFile)
            {
                PartSize = 100 * 1024,
                ParallelThreadCount = 2,
                CheckpointDir = Config.DownloadFolder
            };
            try
            {
                _ossClient.ResumableDownloadObject(request);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
            request.VersionId = versionId;
            metadata = _ossClient.ResumableDownloadObject(request);
            expectedETag = metadata.ETag;
            downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
            Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            FileUtils.DeleteFile(targetFile);

            // > PartSize with single thread
            request = new DownloadObjectRequest(_bucketName, key, targetFile)
            {
                PartSize = 100 * 1024,
                ParallelThreadCount = 1,
                CheckpointDir = Config.DownloadFolder
            };
            try
            {
                _ossClient.ResumableDownloadObject(request);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
            request.VersionId = versionId;
            metadata = _ossClient.ResumableDownloadObject(request);
            expectedETag = metadata.ETag;
            downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
            Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            FileUtils.DeleteFile(targetFile);
        }

        [Test]
        public void ResumableCopyWithVersioningEnableTest()
        {
            _ossClient.SetBucketVersioning(new SetBucketVersioningRequest(_bucketName, VersioningStatus.Enabled));

            var info = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(info.Bucket.Versioning, VersioningStatus.Enabled);

            if (info.Bucket.Versioning != VersioningStatus.Enabled)
                return;

            var targetKey = OssTestUtils.GetObjectKey(_className);
            var sourcekey = OssTestUtils.GetObjectKey(_className);

            _ossClient.PutObject(_bucketName, sourcekey, Config.UploadTestFile);
            var sourceMeta = _ossClient.GetObjectMetadata(_bucketName, sourcekey);

            _ossClient.DeleteObject(_bucketName, sourcekey);

            //  < PartSize
            var copyRequest = new CopyObjectRequest(_bucketName, sourcekey, _bucketName, targetKey);
            try
            {
                _ossClient.ResumableCopyObject(copyRequest, null, sourceMeta.ContentLength + 1);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
            copyRequest.SourceVersionId = sourceMeta.VersionId;
            _ossClient.ResumableCopyObject(copyRequest, Config.DownloadFolder, sourceMeta.ContentLength + 1);
            var targetMeta = _ossClient.GetObjectMetadata(_bucketName, targetKey);
            Assert.AreEqual(targetMeta.Crc64, sourceMeta.Crc64);

            // > PartSize
            copyRequest = new CopyObjectRequest(_bucketName, sourcekey, _bucketName, targetKey);
            try
            {
                _ossClient.ResumableCopyObject(copyRequest, null, 100 * 1024);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
            copyRequest.SourceVersionId = sourceMeta.VersionId;
            _ossClient.ResumableCopyObject(copyRequest, Config.DownloadFolder, 100 * 1024);
            targetMeta = _ossClient.GetObjectMetadata(_bucketName, targetKey);
            Assert.AreEqual(targetMeta.Crc64, sourceMeta.Crc64);
        }

        [Test]
        public void DeleteObjectVersionsRequestArgmentTest()
        {
            try
            {
                var request = new DeleteObjectVersionsRequest(_bucketName, null);

                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var request = new DeleteObjectVersionsRequest(_bucketName, new List<ObjectIdentifier>());

                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var objects = new List<ObjectIdentifier>();
                for (int i = 0; i < 1001; i++)
                    objects.Add(new ObjectIdentifier(i.ToString()));
                var request = new DeleteObjectVersionsRequest(_bucketName, objects);

                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        private static void cleanBucket(string bucketName)
        {
            if (!_ossClient.DoesBucketExist(bucketName))
                return;

            var result = _ossClient.ListObjectVersions(new ListObjectVersionsRequest(bucketName));
            foreach (var o in result.ObjectVersionSummaries)
            {
                var delRequest = new DeleteObjectRequest(bucketName, o.Key)
                {
                    VersionId = o.VersionId
                };
                _ossClient.DeleteObject(delRequest);
            }

            foreach (var o in result.DeleteMarkerSummaries)
            {
                var delRequest = new DeleteObjectRequest(bucketName, o.Key)
                {
                    VersionId = o.VersionId
                };
                _ossClient.DeleteObject(delRequest);
            }

            OssTestUtils.CleanBucket(_ossClient, bucketName);
        }
    }
}