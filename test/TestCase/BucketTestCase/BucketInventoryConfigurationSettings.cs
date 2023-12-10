using Aliyun.OSS.Test.Util;
using NUnit.Framework;
using Aliyun.OSS.Common;
using System.IO;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public class BucketInventoryConfigurationSettingsTest
    {
        private static IOss _ossClient;
        private static string _bucketName;
        private static string _bucketName2;
        private static string _kmsId;

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName("bucket-inventory");
            _bucketName2 = _bucketName + "-dest";
            _ossClient.CreateBucket(_bucketName);
            _ossClient.CreateBucket(_bucketName2);

            var key = OssTestUtils.GetObjectKey("bucket-inventory");
            ObjectMetadata metadata = new ObjectMetadata();
            metadata.HttpMetadata.Add("x-oss-server-side-encryption", "KMS");
            _ossClient.PutObject(_bucketName, key, new MemoryStream(System.Text.Encoding.ASCII.GetBytes("hello")), metadata);
            var result = _ossClient.GetObjectMetadata(new GetObjectMetadataRequest(_bucketName, key));
            _kmsId = result.HttpMetadata["x-oss-server-side-encryption-key-id"] as string;
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
            OssTestUtils.CleanBucket(_ossClient, _bucketName2);
        }

        [Test]
        public void BucketInventoryConfigurationBasicTest()
        {
            //test case 1
            var config = new InventoryConfiguration();
            config.Id = "report1";
            config.IsEnabled = true;
            config.Filter = new InventoryFilter("filterPrefix");
            config.Destination = new InventoryDestination();
            config.Destination.OSSBucketDestination = new InventoryOSSBucketDestination();
            config.Destination.OSSBucketDestination.Format = InventoryFormat.CSV;
            config.Destination.OSSBucketDestination.AccountId = Config.RamUID;
            config.Destination.OSSBucketDestination.RoleArn = Config.RamRoleArn;
            config.Destination.OSSBucketDestination.Bucket = _bucketName2;
            config.Destination.OSSBucketDestination.Prefix = "prefix1";
            config.Destination.OSSBucketDestination.Encryption = new InventoryEncryption(new InventorySSEKMS(_kmsId));
            config.Schedule = new InventorySchedule(InventoryFrequency.Daily);
            config.IncludedObjectVersions = InventoryIncludedObjectVersions.All;
            config.OptionalFields.Add(InventoryOptionalField.Size);
            config.OptionalFields.Add(InventoryOptionalField.LastModifiedDate);
            config.OptionalFields.Add(InventoryOptionalField.StorageClass);
            config.OptionalFields.Add(InventoryOptionalField.IsMultipartUploaded);
            config.OptionalFields.Add(InventoryOptionalField.EncryptionStatus);
            config.OptionalFields.Add(InventoryOptionalField.ETag);

            _ossClient.SetBucketInventoryConfiguration(new SetBucketInventoryConfigurationRequest(_bucketName, config));

            var result = _ossClient.GetBucketInventoryConfiguration(new GetBucketInventoryConfigurationRequest(_bucketName, config.Id));
            
            Assert.AreEqual(result.Configuration.Id, "report1");
            Assert.AreEqual(result.Configuration.IsEnabled, true);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Format, InventoryFormat.CSV);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.AccountId, Config.RamUID);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.RoleArn, Config.RamRoleArn);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Bucket, _bucketName2);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Prefix, "prefix1");
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Encryption.SSEOSS, null);
            Assert.AreNotEqual(result.Configuration.Destination.OSSBucketDestination.Encryption.SSEKMS, null);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Encryption.SSEKMS.KeyId, _kmsId);
            Assert.AreEqual(result.Configuration.Schedule.Frequency, InventoryFrequency.Daily);
            Assert.AreEqual(result.Configuration.Filter.Prefix, "filterPrefix");
            Assert.AreEqual(result.Configuration.IncludedObjectVersions, InventoryIncludedObjectVersions.All);
            Assert.AreEqual(result.Configuration.OptionalFields.Count, 6);
            Assert.AreEqual(result.Configuration.OptionalFields[0], InventoryOptionalField.Size);
            Assert.AreEqual(result.Configuration.OptionalFields[1], InventoryOptionalField.LastModifiedDate);
            Assert.AreEqual(result.Configuration.OptionalFields[2], InventoryOptionalField.StorageClass);
            Assert.AreEqual(result.Configuration.OptionalFields[3], InventoryOptionalField.IsMultipartUploaded);
            Assert.AreEqual(result.Configuration.OptionalFields[4], InventoryOptionalField.EncryptionStatus);
            Assert.AreEqual(result.Configuration.OptionalFields[5], InventoryOptionalField.ETag);

            _ossClient.DeleteBucketInventoryConfiguration(new DeleteBucketInventoryConfigurationRequest(_bucketName, config.Id));

            //test case 2
            config = new InventoryConfiguration();
            config.Id = "report1";
            config.IsEnabled = false;
            config.Filter = new InventoryFilter("filterPrefix");
            config.Destination = new InventoryDestination();
            config.Destination.OSSBucketDestination = new InventoryOSSBucketDestination();
            config.Destination.OSSBucketDestination.Format = InventoryFormat.CSV;
            config.Destination.OSSBucketDestination.AccountId = Config.RamUID;
            config.Destination.OSSBucketDestination.RoleArn = Config.RamRoleArn;
            config.Destination.OSSBucketDestination.Bucket = _bucketName2;
            config.Destination.OSSBucketDestination.Prefix = "prefix1";
            config.Destination.OSSBucketDestination.Encryption = new InventoryEncryption(new InventorySSEOSS());
            config.Schedule = new InventorySchedule(InventoryFrequency.Weekly);
            config.IncludedObjectVersions = InventoryIncludedObjectVersions.Current;
            config.OptionalFields.Add(InventoryOptionalField.StorageClass);

            _ossClient.SetBucketInventoryConfiguration(new SetBucketInventoryConfigurationRequest(_bucketName, config));

            result = _ossClient.GetBucketInventoryConfiguration(new GetBucketInventoryConfigurationRequest(_bucketName, config.Id));

            Assert.AreEqual(result.Configuration.Id, "report1");
            Assert.AreEqual(result.Configuration.IsEnabled, false);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Format, InventoryFormat.CSV);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.AccountId, Config.RamUID);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.RoleArn, Config.RamRoleArn);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Bucket, _bucketName2);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Prefix, "prefix1");
            Assert.AreNotEqual(result.Configuration.Destination.OSSBucketDestination.Encryption.SSEOSS, null);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Encryption.SSEKMS, null);
            Assert.AreEqual(result.Configuration.Schedule.Frequency, InventoryFrequency.Weekly);
            Assert.AreEqual(result.Configuration.Filter.Prefix, "filterPrefix");
            Assert.AreEqual(result.Configuration.IncludedObjectVersions, InventoryIncludedObjectVersions.Current);
            Assert.AreEqual(result.Configuration.OptionalFields.Count, 1);
            Assert.AreEqual(result.Configuration.OptionalFields[0], InventoryOptionalField.StorageClass);


            _ossClient.DeleteBucketInventoryConfiguration(new DeleteBucketInventoryConfigurationRequest(_bucketName, config.Id));


            //test case 3
            config = new InventoryConfiguration();
            config.Id = "report1";
            config.IsEnabled = false;
            config.Destination = new InventoryDestination();
            config.Destination.OSSBucketDestination = new InventoryOSSBucketDestination();
            config.Destination.OSSBucketDestination.Format = InventoryFormat.CSV;
            config.Destination.OSSBucketDestination.AccountId = Config.RamUID;
            config.Destination.OSSBucketDestination.RoleArn = Config.RamRoleArn;
            config.Destination.OSSBucketDestination.Bucket = _bucketName2;
            config.Destination.OSSBucketDestination.Prefix = "prefix1";
            config.Schedule = new InventorySchedule(InventoryFrequency.Weekly);
            config.IncludedObjectVersions = InventoryIncludedObjectVersions.Current;

            _ossClient.SetBucketInventoryConfiguration(new SetBucketInventoryConfigurationRequest(_bucketName, config));

            result = _ossClient.GetBucketInventoryConfiguration(new GetBucketInventoryConfigurationRequest(_bucketName, config.Id));

            Assert.AreEqual(result.Configuration.Id, "report1");
            Assert.AreEqual(result.Configuration.IsEnabled, false);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Format, InventoryFormat.CSV);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.AccountId, Config.RamUID);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.RoleArn, Config.RamRoleArn);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Bucket, _bucketName2);
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Prefix, "prefix1");
            Assert.AreEqual(result.Configuration.Destination.OSSBucketDestination.Encryption, null);
            Assert.AreEqual(result.Configuration.Schedule.Frequency, InventoryFrequency.Weekly);
            Assert.AreEqual(result.Configuration.Filter.Prefix, "");
            Assert.AreEqual(result.Configuration.IncludedObjectVersions, InventoryIncludedObjectVersions.Current);
            Assert.AreEqual(result.Configuration.OptionalFields.Count, 0);

            _ossClient.DeleteBucketInventoryConfiguration(new DeleteBucketInventoryConfigurationRequest(_bucketName, config.Id));

            try
            { 
                _ossClient.GetBucketInventoryConfiguration(new GetBucketInventoryConfigurationRequest(_bucketName, config.Id));
                Assert.Fail("should not here");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "NoSuchInventory");
            }
        }

        [Test]
        public void ListBucketInventoryConfigurationTest()
        {
            //clear all inventory configurations first
            _ossClient.DeleteBucketInventoryConfiguration(new DeleteBucketInventoryConfigurationRequest(_bucketName, "report1"));

            int total = 104;
            for (int i = 0; i < total; i++)
            {
                var config = new InventoryConfiguration();
                config.Id = "test" + i.ToString("D4");
                config.IsEnabled = (i%4 == 0) ? true:false;
                config.Filter = (i % 4 != 0) ? new InventoryFilter("filterPrefix" + i.ToString("D4")) : null;
                config.Destination = new InventoryDestination();
                config.Destination.OSSBucketDestination = new InventoryOSSBucketDestination();
                config.Destination.OSSBucketDestination.Format = InventoryFormat.CSV;
                config.Destination.OSSBucketDestination.AccountId = Config.RamUID;
                config.Destination.OSSBucketDestination.RoleArn = Config.RamRoleArn;
                config.Destination.OSSBucketDestination.Bucket = _bucketName2;
                config.Destination.OSSBucketDestination.Prefix = "prefix" + i.ToString("D4");
                config.Schedule = new InventorySchedule(InventoryFrequency.Daily);
                config.IncludedObjectVersions = InventoryIncludedObjectVersions.All;
                _ossClient.SetBucketInventoryConfiguration(new SetBucketInventoryConfigurationRequest(_bucketName, config));
            }

            var result = _ossClient.ListBucketInventoryConfiguration(new ListBucketInventoryConfigurationRequest(_bucketName, ""));
            var configs = OssTestUtils.ToArray(result.Configurations);

            Assert.AreEqual(configs.Count, 100);
            Assert.AreEqual(result.IsTruncated, true);
            Assert.AreEqual(result.NextContinuationToken, "test" + 99.ToString("D4"));
            Assert.AreEqual(configs[0].Id, "test" + 0.ToString("D4"));
            Assert.AreEqual(configs[99].Id, "test" + 99.ToString("D4"));

            result = _ossClient.ListBucketInventoryConfiguration(new ListBucketInventoryConfigurationRequest(_bucketName, result.NextContinuationToken));
            configs = OssTestUtils.ToArray(result.Configurations);

            Assert.AreEqual(configs.Count, 4);
            Assert.AreEqual(result.IsTruncated, false);
            Assert.AreEqual(result.NextContinuationToken, null);
            Assert.AreEqual(configs[0].Id, "test" + 100.ToString("D4"));
            Assert.AreEqual(configs[3].Id, "test" + 103.ToString("D4"));

            result = _ossClient.ListBucketInventoryConfiguration(new ListBucketInventoryConfigurationRequest(_bucketName, null));
            configs = OssTestUtils.ToArray(result.Configurations);

            Assert.AreEqual(configs.Count, 100);
            Assert.AreEqual(result.IsTruncated, true);
            Assert.AreEqual(result.NextContinuationToken, "test" + 99.ToString("D4"));
            Assert.AreEqual(configs[0].Id, "test" + 0.ToString("D4"));
            Assert.AreEqual(configs[99].Id, "test" + 99.ToString("D4"));

        }
    }
}
