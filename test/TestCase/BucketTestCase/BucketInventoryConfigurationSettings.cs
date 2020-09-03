using System;
using Aliyun.OSS.Test.Util;
using System.Collections.Generic;
using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public partial class BucketSettingsTest
    {
        private static string _className2;
        private static string _destBucketName;

        [OneTimeSetUp]
        public static void ClassInitialize1()
        {
            _ossClient = OssClientFactory.CreateOssClient();
            _className2 = "BucketInventoryConfigurationTest";
            _className2 = _className2.Substring(_className2.LastIndexOf('.') + 1).ToLowerInvariant();
            _destBucketName = OssTestUtils.GetBucketName(_className2);
            _ossClient.CreateBucket(_destBucketName);
        }

        [OneTimeTearDown]
        public static void ClassCleanup2()
        {
            OssTestUtils.CleanBucket(_ossClient, _destBucketName);
        }

        [Test]
        public void SetBucketInventoryConfigurationTest()
        {
            try
            {
                var refer = new List<InventoryOptionalField>();
                refer.Add(InventoryOptionalField.Size);
                refer.Add(InventoryOptionalField.LastModifiedDate);
                refer.Add(InventoryOptionalField.ETag);
                refer.Add(InventoryOptionalField.StorageClass);
                refer.Add(InventoryOptionalField.IsMultipartUploaded);
                refer.Add(InventoryOptionalField.EncryptionStatus);

                var req = new SetBucketInventoryConfigurationRequest(_bucketName, refer);
                req.Id = "report1";
                req.IsEnabled = true;
                req.Prefix = "filterPrefix";

                var des = new OSSBucketDestination(InventoryFormat.CSV);
                des.AccountId = Config.RamUID;
                des.RoleArn = Config.RamRoleArn;
                des.Bucket = _destBucketName;
                des.Prefix = "prefix1";
                des.Encryption = "keyId";

                req.Destination = des;
                req.Schedule = InventoryFrequency.Daily;
                req.IncludedObjectVersions = InventoryIncludedObjectVersions.All;


                _ossClient.SetBucketInventoryConfiguration(req);

                var getresult = _ossClient.GetBucketInventoryConfiguration(_bucketName, "report1");
                Assert.AreEqual(getresult.InventoryConfiguration.Id, req.Id);
                Assert.AreEqual(getresult.InventoryConfiguration.IsEnabled, req.IsEnabled);
                Assert.AreEqual(getresult.InventoryConfiguration.filter.Prefix, req.Prefix);
                Assert.AreEqual(getresult.InventoryConfiguration.destination.OSSBucketDestination.Format, InventoryFormat.CSV);
                Assert.AreEqual(getresult.InventoryConfiguration.destination.OSSBucketDestination.AccountId, des.AccountId);
                Assert.AreEqual(getresult.InventoryConfiguration.destination.OSSBucketDestination.RoleArn, des.RoleArn);
                Assert.AreEqual(getresult.InventoryConfiguration.destination.OSSBucketDestination.Bucket, ("acs:oss:::"+ des.Bucket));
                Assert.AreEqual(getresult.InventoryConfiguration.destination.OSSBucketDestination.Prefix, des.Prefix);
                Assert.AreEqual(getresult.InventoryConfiguration.destination.OSSBucketDestination.encryption.SSEKMS.KeyId, des.Encryption);

                Assert.AreEqual(getresult.InventoryConfiguration.schedule.Frequency, req.Schedule);

                Assert.AreEqual(getresult.InventoryConfiguration.IncludedObjectVersions, req.IncludedObjectVersions);

                Assert.AreEqual(getresult.InventoryConfiguration.OptionalFields.Field, refer);

                _ossClient.DeleteBucketInventoryConfiguration(_bucketName, "report1");

                var req2 = new SetBucketInventoryConfigurationRequest(_bucketName, refer);
                req2.Id = "report1";
                req2.IsEnabled = true;
                req2.Prefix = "filterPrefix";

                var des2 = new OSSBucketDestination(InventoryFormat.CSV);
                des2.AccountId = Config.RamUID;
                des2.RoleArn = Config.RamRoleArn;
                des2.Bucket = _destBucketName;
                des2.Prefix = "prefix1";
                des2.IsEncryptionOSS = true;

                req2.Destination = des2;
                req2.Schedule = InventoryFrequency.Daily;
                req2.IncludedObjectVersions = InventoryIncludedObjectVersions.All;

                _ossClient.SetBucketInventoryConfiguration(req2);

                var getresult2 = _ossClient.GetBucketInventoryConfiguration(_bucketName, "report1");
                Assert.AreEqual(getresult2.InventoryConfiguration.Id, req.Id);
                Assert.AreEqual(getresult2.InventoryConfiguration.IsEnabled, req.IsEnabled);
                Assert.AreEqual(getresult2.InventoryConfiguration.filter.Prefix, req.Prefix);
                Assert.AreEqual(getresult2.InventoryConfiguration.destination.OSSBucketDestination.Format, InventoryFormat.CSV);
                Assert.AreEqual(getresult2.InventoryConfiguration.destination.OSSBucketDestination.AccountId, des.AccountId);
                Assert.AreEqual(getresult2.InventoryConfiguration.destination.OSSBucketDestination.RoleArn, des.RoleArn);
                Assert.AreEqual(getresult2.InventoryConfiguration.destination.OSSBucketDestination.Bucket, ("acs:oss:::" + des.Bucket));
                Assert.AreEqual(getresult2.InventoryConfiguration.destination.OSSBucketDestination.Prefix, des.Prefix);
                Assert.AreEqual(getresult2.InventoryConfiguration.destination.OSSBucketDestination.encryption.EncryptionOSS, "true");

                Assert.AreEqual(getresult2.InventoryConfiguration.schedule.Frequency, req.Schedule);

                Assert.AreEqual(getresult2.InventoryConfiguration.IncludedObjectVersions, req.IncludedObjectVersions);

                Assert.AreEqual(getresult2.InventoryConfiguration.OptionalFields.Field, refer);

                _ossClient.DeleteBucketInventoryConfiguration(_bucketName, "report1");


            }

            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }

        }

        [Test]
        public void ListBucketInventoryConfigurationTest()
        {
            try
            {
                for (int i = 1; i < 102; i++)
                {
                    var refer = new List<InventoryOptionalField>();
                    refer.Add(InventoryOptionalField.Size);
                    refer.Add(InventoryOptionalField.LastModifiedDate);
                    refer.Add(InventoryOptionalField.ETag);
                    refer.Add(InventoryOptionalField.StorageClass);
                    refer.Add(InventoryOptionalField.IsMultipartUploaded);
                    refer.Add(InventoryOptionalField.EncryptionStatus);

                    var req = new SetBucketInventoryConfigurationRequest(_bucketName, refer);
                    req.Id = i.ToString();
                    req.IsEnabled = true;
                    req.Prefix = "filterPrefix";

                    var des = new OSSBucketDestination(InventoryFormat.CSV);
                    des.AccountId = Config.RamUID;
                    des.RoleArn = Config.RamRoleArn;
                    des.Bucket = _destBucketName;
                    des.Prefix = "prefix1";
                    des.Encryption = "keyId";

                    req.Destination = des;
                    req.Schedule = InventoryFrequency.Daily;
                    req.IncludedObjectVersions = InventoryIncludedObjectVersions.All;

                    _ossClient.SetBucketInventoryConfiguration(req);
                }

                var result = _ossClient.ListBucketInventoryConfiguration(_bucketName, null);
                Assert.AreEqual(result.IsTruncated,true);
                Assert.AreEqual(result.NextContinuationToken,"98");
                Assert.AreEqual(result.BucketInventory[0].Id, "1");
                Assert.AreEqual(result.BucketInventory[0].IsEnabled, true);
                Assert.AreEqual(result.BucketInventory[0].filter.Prefix, "filterPrefix");
                Assert.AreEqual(result.BucketInventory[0].destination.OSSBucketDestination.Format, InventoryFormat.CSV);
                Assert.AreEqual(result.BucketInventory[0].destination.OSSBucketDestination.AccountId, Config.RamUID);
                Assert.AreEqual(result.BucketInventory[0].destination.OSSBucketDestination.RoleArn, Config.RamRoleArn);
                Assert.AreEqual(result.BucketInventory[0].destination.OSSBucketDestination.Bucket, ("acs:oss:::" + _destBucketName));
                Assert.AreEqual(result.BucketInventory[0].destination.OSSBucketDestination.Prefix, "prefix1");
                Assert.AreEqual(result.BucketInventory[0].schedule.Frequency, InventoryFrequency.Daily);
                Assert.AreEqual(result.BucketInventory[0].destination.OSSBucketDestination.encryption.SSEKMS.KeyId, "keyId");

                Assert.AreEqual(result.BucketInventory[0].IncludedObjectVersions, InventoryIncludedObjectVersions.All);


                _ossClient.ListBucketInventoryConfiguration(_bucketName, result.NextContinuationToken);


                for (int i = 1; i < 102; i++)
                {
                    _ossClient.DeleteBucketInventoryConfiguration(_bucketName, i.ToString());
                }

            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }
    }
}
