using System;
using System.Collections.Generic;
using System.Threading;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    [TestFixture]
    public class BucketBasicOperationTest
    {
        private static IOss _ossClient;
        private static string _className;

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            //prefix of bucket name used in current test class
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
        }

        [TestFixtureTearDown]
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

            foreach(var bucket in testBuckets)
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

        [Ignore]
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

        [Ignore]
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
        [Ignore]
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
                catch(OssException e)
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
            try {
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
                try {
                    _ossClient.DeleteBucket(bucketName);
                } catch(Exception)
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
        #endregion

        #region List Buckets

        [Test]
        public void ListBucketspagingTest()
        {
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

            //delete the new created bucket
            _ossClient.DeleteBucket(bucketName);
            OssTestUtils.WaitForCacheExpire();
            Assert.IsFalse(OssTestUtils.BucketExists(_ossClient, bucketName),
                string.Format("Bucket {0} should not exist after deletion", bucketName));
        }

        #endregion
    }
}
