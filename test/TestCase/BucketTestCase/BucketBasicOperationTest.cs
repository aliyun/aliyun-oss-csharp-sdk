using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;
using Aliyun.OSS.Util;
using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    [TestFixture]
    public class BucketBasicOperationTest
    {
        private static IOss _ossClient;
        private static string _className;

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            //prefix of bucket name used in current test class
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            //Bucket is limited resources, so double check to clean up all remain
            //buckets created by current test class

            var testBuckets = new List<string>();

            var listBuckets = _ossClient.ListBuckets();
            foreach (var bucket in listBuckets)
            {
                if (bucket.Name.StartsWith(_className))
                {
                    testBuckets.Add(bucket.Name);
                }
            }

            foreach (var bucket in testBuckets)
            {
                OssTestUtils.CleanBucket(_ossClient, bucket);
            }
        }

#region Create Bucket Cases
        [Test]
        public void CreateAndDeleteBucketTest()
        {
            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);

            //assert bucket does not exist
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist before creation", bucketName));

            //create a new bucket
            _ossClient.CreateBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsTrue(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should exist after creation", bucketName));

            //delete the new created bucket
            _ossClient.DeleteBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist after deletion", bucketName));
        }

        [Test]
        public void CreateAndDeleteIABucketTest()
        {
            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);

            //assert bucket does not exist
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist before creation", bucketName));

            //create a new bucket
            _ossClient.CreateBucket(bucketName, StorageClass.IA);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsTrue(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should exist after creation", bucketName));

            var objectName = bucketName + "firstobject";
            _ossClient.PutObject(bucketName, objectName, new MemoryStream());

            var objMeta = _ossClient.GetObjectMetadata(bucketName, objectName);

            Assert.AreEqual(objMeta.HttpMetadata["x-oss-storage-class"], StorageClass.IA.ToString());
            _ossClient.DeleteObject(bucketName, objectName);

            //delete the new created bucket
            _ossClient.DeleteBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist after deletion", bucketName));
        }

        [Test]
        public void CreateAndDeleteArchiveBucketTest()
        {
            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);

            //assert bucket does not exist
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist before creation", bucketName));

            //create a new bucket
            _ossClient.CreateBucket(bucketName, StorageClass.Archive);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsTrue(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should exist after creation", bucketName));

            var objectName = bucketName + "firstobject";
            _ossClient.PutObject(bucketName, objectName, new MemoryStream());

            var objMeta = _ossClient.GetObjectMetadata(bucketName, objectName);

            Assert.AreEqual(objMeta.HttpMetadata["x-oss-storage-class"], StorageClass.Archive.ToString());
            _ossClient.DeleteObject(bucketName, objectName);

            //delete the new created bucket
            _ossClient.DeleteBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist after deletion", bucketName));
        }

        [Ignore("Ignore")]
        public void CreateAndDeleteBucketSecondRegionTest()
        {
            var settings = AccountSettings.Load();
            //point to region (Beijing) other than Hangzhou
            settings.OssEndpoint = Config.SecondEndpoint;
            var ossClient = OssClientFactory.CreateOssClient(settings);

            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);

            //assert bucket does not exist
            Assert.IsFalse(OssTestUtils.BucketExists(ossClient, bucketName),
                string.Format("Bucket {0} should not exist before creation", bucketName));

            //create a new bucket
            ossClient.CreateBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsTrue(OssTestUtils.BucketExists(ossClient, bucketName),
                string.Format("Bucket {0} should exist after creation", bucketName));

            //delete the bucket
            ossClient.DeleteBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsFalse(OssTestUtils.BucketExists(ossClient, bucketName),
                string.Format("Bucket {0} should not exist after deletion", bucketName));
        }

        [Test]
        public void CreateAndDeleteBucketDefaultRegionTest()
        {
            var settings = AccountSettings.Load();
            //point to default region
            var ossClient = new OssClient(settings.OssEndpoint, settings.OssAccessKeyId, settings.OssAccessKeySecret);

            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);

            //assert bucket does not exist
            Assert.IsFalse(OssTestUtils.BucketExists(ossClient, bucketName),
                string.Format("Bucket {0} should not exist before creation", bucketName));

            //create a new bucket
            ossClient.CreateBucket(bucketName);
            Assert.IsTrue(ossClient.DoesBucketExist(bucketName),
                string.Format("Bucket {0} should exist after creation", bucketName));

            //delete the bucket
            ossClient.DeleteBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsFalse(ossClient.DoesBucketExist(bucketName),
                string.Format("Bucket {0} should not exist after deletion", bucketName));
        }

        [Test]
        public void CreateBucketInvalidNameTest()
        {
            foreach (var invalidBucketName in OssTestUtils.InvalidBucketNamesList)
            {
                try
                {
                    _ossClient.CreateBucket(invalidBucketName);
                    Assert.Fail("Invalid bucket name {0} should not be created successfully", invalidBucketName);
                }
                catch (ArgumentException)
                {
                    Assert.IsTrue(true);
                }
            }
        }

        [Ignore("Ignore")]
        public void CreateBucketWithDuplicatedNameDifferentLocationTest()
        {
            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);

            var settings = AccountSettings.Load();
            //point to location (Beijing) other than Hangzhou
            settings.OssEndpoint = Config.SecondEndpoint;
            var ossSecondClient = OssClientFactory.CreateOssClient(settings);
            //create the bucket
            _ossClient.CreateBucket(bucketName);

            try
            {
                //TODO: due to 5596363, user can see buckets in all regions, though only specify one region
                //assert bucket does not exist
                //Assert.IsFalse(OssTestUtils.BucketExists(ossSecondClient, bucketName));
                Assert.IsTrue(OssTestUtils.BucketExists(ossSecondClient, bucketName));
                //try to create bucket with same name on different location
                ossSecondClient.CreateBucket(bucketName);
                Assert.Fail("Bucket creation should be failed with dup name and different location");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.BucketAlreadyExists, e.ErrorCode);
            }
            finally
            {
                _ossClient.DeleteBucket(bucketName);
            }

        }

        //bucket number max to 10
        private const int MaxAllowedBucketNumber = 10;
        [Ignore("Ignore")]
        public void CreateBucketWhenBucketNumberReachesLimitTest()
        {
            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);
            var bucketNames = new List<string>();

            try
            {
                //get number of existing buckets
                var existingBucket = OssTestUtils.ToArray<Bucket>(_ossClient.ListBuckets()).Count;
                var remainCount = MaxAllowedBucketNumber - existingBucket;

                for (var i = 0; i < remainCount; i++)
                {
                    bucketNames.Add(OssTestUtils.GetBucketName(_className));
                    //sleep for 100ms, so that the bucket name won't be duplicated
                    Thread.Sleep(100);
                }

                //create buckets in parallel, so that the total number reaches 10
                //Parallel.ForEach(bucketNames, singleBucketName => _ossClient.CreateBucket(singleBucketName));

                try
                {
                    //try to create a new bucket
                    _ossClient.CreateBucket(bucketName);
                    Assert.Fail("Bucket creation should fail when number of existing Bucketes reaches limit");
                }
                catch (OssException e)
                {
                    Assert.AreEqual(OssErrorCode.TooManyBuckets, e.ErrorCode);
                }
            }
            finally
            {
                //Parallel.ForEach(bucketNames, singleBucketName =>
                foreach (var singleBucketName in bucketNames)
                {
                    if (OssTestUtils.BucketExists(_ossClient, singleBucketName))
                    {
                        _ossClient.DeleteBucket(singleBucketName);
                    }
                }

                if (OssTestUtils.BucketExists(_ossClient, bucketName))
                {
                    _ossClient.DeleteBucket(bucketName);
                }
            }
        }

        [Test]
        public void CreateBucketByCreateBucketRequest()
        {
            //create a new bucket by default, acl is private, storage class is standard
            var bucketName = OssTestUtils.GetBucketName(_className);
            var request = new CreateBucketRequest(bucketName);
            _ossClient.CreateBucket(request);
            Assert.IsTrue(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should exist after creation", bucketName));

            var result = _ossClient.GetBucketInfo(bucketName);
            Assert.AreEqual(result.Bucket.AccessControlList.Grant, CannedAccessControlList.Private);
            Assert.AreEqual(result.Bucket.StorageClass, StorageClass.Standard);
            Assert.AreEqual(result.Bucket.DataRedundancyType, DataRedundancyType.LRS);

            _ossClient.DeleteBucket(bucketName);
            OssTestUtils.WaitForCacheExpire(1);

            //create a public bucket with IA 
            bucketName = OssTestUtils.GetBucketName(_className);
            request = new CreateBucketRequest(bucketName, StorageClass.IA, CannedAccessControlList.PublicReadWrite);
            request.DataRedundancyType = DataRedundancyType.LRS;

            _ossClient.CreateBucket(request);
            Assert.IsTrue(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should exist after creation", bucketName));

            result = _ossClient.GetBucketInfo(bucketName);
            Assert.AreEqual(result.Bucket.AccessControlList.Grant, CannedAccessControlList.PublicReadWrite);
            Assert.AreEqual(result.Bucket.StorageClass, StorageClass.IA);
            Assert.AreEqual(result.Bucket.DataRedundancyType, DataRedundancyType.LRS);

            _ossClient.DeleteBucket(bucketName);


            //create a public bucket with IA , ZRS
            bucketName = OssTestUtils.GetBucketName(_className);
            request = new CreateBucketRequest(bucketName, StorageClass.IA, CannedAccessControlList.PublicReadWrite);
            request.DataRedundancyType = DataRedundancyType.ZRS;
            _ossClient.CreateBucket(request);
            Assert.IsTrue(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should exist after creation", bucketName));

            result = _ossClient.GetBucketInfo(bucketName);
            Assert.AreEqual(result.Bucket.AccessControlList.Grant, CannedAccessControlList.PublicReadWrite);
            Assert.AreEqual(result.Bucket.StorageClass, StorageClass.IA);
            Assert.AreEqual(result.Bucket.DataRedundancyType, DataRedundancyType.ZRS);

            _ossClient.DeleteBucket(bucketName);
        }
        #endregion

        #region Delete Bucket Cases
        [Test]
        public void DeleteNonExistBucketTest()
        {
            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);

            //assert bucket does not exist
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName));

            try
            {
                //delete bucket which does not exit
                _ossClient.DeleteBucket(bucketName);
                Assert.Fail("Deleting a bucket that does not exist should not success!");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.NoSuchBucket, e.ErrorCode);
            }
        }

        [Test]
        public void DeleteBucketInvalidNameTest()
        {
            //Parallel.ForEach(OssTestUtils.InvalidBucketNamesList, invalidBucketName =>
            foreach (var invalidBucketName in OssTestUtils.InvalidBucketNamesList)
            {
                try
                {
                    _ossClient.DeleteBucket(invalidBucketName);
                    Assert.Fail("Invalid bucket name should not be created successfully: {0}", invalidBucketName);
                }
                catch (ArgumentException)
                {
                    Assert.IsTrue(true);
                }
            }
        }
#endregion

#region Dose Bucket Exists Cases
        [Test]
        public void DoesBucketExistTestWithBucketExist()
        {
            const string bucketName = "exist-bucket";
            try
            {
                var result = _ossClient.CreateBucket(bucketName);
                Assert.AreEqual(bucketName, result.Name);

                bool isExist = _ossClient.DoesBucketExist(bucketName);
                Assert.IsTrue(isExist);
            }
            catch (Exception e)
            {
                Assert.False(true, e.Message);
            }
            finally
            {
                OssTestUtils.CleanBucket(_ossClient, bucketName);
            }
        }

        [Test]
        public void DoesBucketExistTestWithBucketNotExist()
        {
            try
            {
                const string bucketName = "not-exist-bucket";
                try
                {
                    _ossClient.DeleteBucket(bucketName);
                }
                catch (Exception)
                {
                    //nothing
                }

                bool isExist = _ossClient.DoesBucketExist(bucketName);
                Assert.False(isExist);
            }
            catch (Exception e)
            {
                Assert.True(false, e.Message);
            }
        }

        [Test]
        public void DoesBucketExistTestWithException()
        {
            try
            {
                _ossClient.DoesBucketExist("");
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.True(true, e.Message);
            }

            try
            {
                _ossClient.DoesBucketExist("Invalid-Bucket");
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.True(true, e.Message);
            }

            var bucketName1 = OssTestUtils.GetBucketName(_className);
            try
            {
                var client = new OssClient(Config.Endpoint, Config.AccessKeyId, "invalid-sk");
                _ossClient.CreateBucket(bucketName1, StorageClass.IA);
                client.DoesBucketExist(bucketName1);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.True(true, e.Message);
            }
            finally
            {
                _ossClient.DeleteBucket(bucketName1);
            }
        }
        #endregion

        #region List Buckets

        [Test]
        public void ListBucketspagingTest()
        {
            for (int i = 0; i < 5; i++)
            {
                var bucketName = OssTestUtils.GetBucketName(_className);
                _ossClient.CreateBucket(bucketName);
            }

            var lbRequest = new ListBucketsRequest
            {
                Prefix = _className,
                MaxKeys = 2
            };

            while (true)
            {
                var ibRes = _ossClient.ListBuckets(lbRequest);
                if (ibRes.IsTruncated == true)
                {
                    lbRequest.Marker = ibRes.NextMaker;
                }
                else
                {
                    break;
                }
            }

            //prifix null
            lbRequest.Prefix = null;
            lbRequest.Marker = null;
            lbRequest.MaxKeys = 2;
            _ossClient.ListBuckets(lbRequest);

            //MaxKeys null
            lbRequest.Prefix = _className;
            lbRequest.Marker = null;
            lbRequest.MaxKeys = null;
            _ossClient.ListBuckets(lbRequest);
        }

        /*
        [Test]
        public void DeleteTestBuckets()
        {
            ListBucketsRequest bucketReq = new ListBucketsRequest()
            {
                Prefix = "object"
            };
            var buckets = _ossClient.ListBuckets(bucketReq);
            foreach(Bucket bucket in buckets.Buckets)
            {
                var objList = _ossClient.ListObjects(bucket.Name);
                foreach(var obj in objList.ObjectSummaries)
                {
                    _ossClient.DeleteObject(obj.BucketName, obj.Key);
                }

                var uploads = _ossClient.ListMultipartUploads(new ListMultipartUploadsRequest(bucket.Name));
                foreach(var upload in uploads.MultipartUploads)
                {
                    AbortMultipartUploadRequest req = new AbortMultipartUploadRequest(bucket.Name, upload.Key, upload.UploadId);
                    _ossClient.AbortMultipartUpload(req);
                }

                _ossClient.DeleteBucket(bucket.Name);
            }
        }*/
#endregion

#region GetBucketInfo
        [Test]
        public void GetBucketInfoTest()
        {
            var bucketName = OssTestUtils.GetBucketName(_className);
            try
            {
                //assert bucket does not exist
                Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                    string.Format("Bucket {0} should not exist before creation", bucketName));

                //create a new bucket
                _ossClient.CreateBucket(bucketName, StorageClass.IA);
                OssTestUtils.WaitForCacheExpire();

                BucketInfo bucketInfo = _ossClient.GetBucketInfo(bucketName);
                Assert.AreEqual(bucketInfo.Bucket.AccessControlList.Grant, CannedAccessControlList.Private);

                _ossClient.SetBucketAcl(bucketName, CannedAccessControlList.PublicRead);
                OssTestUtils.WaitForCacheExpire();
                bucketInfo = _ossClient.GetBucketInfo(bucketName);
                Assert.AreEqual(bucketInfo.Bucket.AccessControlList.Grant, CannedAccessControlList.PublicRead);

                _ossClient.SetBucketAcl(bucketName, CannedAccessControlList.PublicReadWrite);
                OssTestUtils.WaitForCacheExpire();
                bucketInfo = _ossClient.GetBucketInfo(bucketName);
                Assert.AreEqual(bucketInfo.Bucket.AccessControlList.Grant, CannedAccessControlList.PublicReadWrite);

                Assert.IsNotNull(bucketInfo.Bucket.Location);
                Assert.IsNotNull(bucketInfo.Bucket.ExtranetEndpoint);
                Assert.IsNotNull(bucketInfo.Bucket.IntranetEndpoint);

                Assert.AreEqual(bucketInfo.Bucket.StorageClass, StorageClass.IA);
                Assert.AreEqual(bucketInfo.Bucket.Name, bucketName);
                Assert.IsTrue(bucketInfo.Bucket.ToString().Contains(bucketName));
            }
            finally
            {
                _ossClient.DeleteBucket(bucketName);
            }
        }

        [Test]
        public void GetNonExistBucketInfoTest()
        {
            var bucketName = OssTestUtils.GetBucketName(_className);

            //assert bucket does not exist
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist before creation", bucketName));

            try
            {
                _ossClient.GetBucketInfo(bucketName);
                Assert.Fail();
            }
            catch(OssException exception)
            {
                Assert.AreEqual(exception.ErrorCode, "NoSuchBucket");
            }
        }

        [Test]
        public void GetNullBucketInfoTest()
        {
            try{
                _ossClient.GetBucketInfo(null);
                Assert.Fail();
            }
            catch(ArgumentException)
            {
            }
        }
#endregion

#region GetBucketStat
        [Test]
        public void GetBucketStatTest()
        {
            var bucketName = OssTestUtils.GetBucketName(_className);
            var objName = OssTestUtils.GetObjectKey(_className);
            try
            {
                //assert bucket does not exist
                Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                    string.Format("Bucket {0} should not exist before creation", bucketName));

                //create a new bucket
                _ossClient.CreateBucket(bucketName);
                OssTestUtils.WaitForCacheExpire();

                _ossClient.PutObject(bucketName, objName, new MemoryStream(new byte[] { 1, 2, 3 }));

                BucketStat bucketStat = _ossClient.GetBucketStat(bucketName);
                Assert.AreEqual(bucketStat.Storage, 3);
                Assert.AreEqual(bucketStat.ObjectCount, 1);
                Assert.AreEqual(bucketStat.MultipartUploadCount, 0);

                _ossClient.DeleteObject(bucketName, objName);
            }
            finally
            {
                _ossClient.DeleteBucket(bucketName);
            }
        }

        [Test]
        public void GetNonExistBucketStatTest()
        {
            var bucketName = OssTestUtils.GetBucketName(_className);

            //assert bucket does not exist
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist before creation", bucketName));

            try
            {
                _ossClient.GetBucketStat(bucketName);
                Assert.Fail();
            }
            catch (OssException exception)
            {
                Assert.AreEqual(exception.ErrorCode, "NoSuchBucket");
            }
        }

        [Test]
        public void GetNullBucketStatTest()
        {
            try
            {
                _ossClient.GetBucketStat(null);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }

        #endregion

        #region Get Bucket Location Cases

        [Test]
        public void GetBucketLocationTest()
        {
            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);

            //assert bucket does not exist
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist before creation", bucketName));

            //create a new bucket
            _ossClient.CreateBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsTrue(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should exist after creation", bucketName));

            //get bucket location
            var locRes = _ossClient.GetBucketLocation(bucketName);
            Assert.IsTrue(locRes.Location.StartsWith("oss-"),
                string.Format("Bucket Location {0} should start with 'oss-' but actual {1}", bucketName, locRes.Location));

            //delete the new created bucket
            _ossClient.DeleteBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist after deletion", bucketName));
        }

        #endregion

        #region Get Bucket Metadata Cases

        [Test]
        public void GetBucketMetadataTest()
        {
            //get a random bucketName
            var bucketName = OssTestUtils.GetBucketName(_className);

            //assert bucket does not exist
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist before creation", bucketName));

            //create a new bucket
            _ossClient.CreateBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsTrue(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should exist after creation", bucketName));

            //get bucket metadata
            var metedata = _ossClient.GetBucketMetadata(bucketName);
            Assert.IsTrue(metedata.BucketRegion.StartsWith("oss-"),
                string.Format("Bucket Region {0} should start with 'oss-' but actual {1}", bucketName, metedata.BucketRegion));
            Assert.IsTrue(metedata.HttpMetadata[HttpHeaders.BucketRegion].StartsWith("oss-"),
                string.Format("Bucket Region {0} should start with 'oss-' but actual {1}", bucketName, metedata.BucketRegion));

            //delete the new created bucket
            _ossClient.DeleteBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist after deletion", bucketName));

            var metadata = new BucketMetadata();
            Assert.AreEqual(metadata.BucketRegion, null);
        }

        #endregion
    }
}
