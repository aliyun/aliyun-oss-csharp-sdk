using System;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    public partial class BucketSettingsTest
    {
        [Test]
        public void BucketEncryptionBasicTest()
        {
            //default
            try
            {
                _ossClient.GetBucketEncryption(_bucketName);
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "NoSuchServerSideEncryptionRule");
            }
            var bInfo = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(bInfo.Bucket.ServerSideEncryptionRule.SSEAlgorithm, "None");
            Assert.AreEqual(bInfo.Bucket.ServerSideEncryptionRule.KMSMasterKeyID, null);

            //set to AES256
            var request = new SetBucketEncryptionRequest(_bucketName, "AES256");
            Assert.AreEqual(request.SSEAlgorithm, "AES256");
            Assert.AreEqual(request.KMSMasterKeyID, null);
            _ossClient.SetBucketEncryption(request);

            var result = _ossClient.GetBucketEncryption(_bucketName);
            Assert.AreEqual(result.SSEAlgorithm, "AES256");
            Assert.AreEqual(result.KMSMasterKeyID, null);

            bInfo = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(bInfo.Bucket.ServerSideEncryptionRule.SSEAlgorithm, "AES256");
            Assert.AreEqual(bInfo.Bucket.ServerSideEncryptionRule.KMSMasterKeyID, null);

            //SET TO KMS
            request = new SetBucketEncryptionRequest(_bucketName, "KMS", null);
            Assert.AreEqual(request.SSEAlgorithm, "KMS");
            Assert.AreEqual(request.KMSMasterKeyID, null);
            _ossClient.SetBucketEncryption(request);

            result = _ossClient.GetBucketEncryption(_bucketName);
            Assert.AreEqual(result.SSEAlgorithm, "KMS");
            Assert.AreEqual(result.KMSMasterKeyID, null);

            bInfo = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(bInfo.Bucket.ServerSideEncryptionRule.SSEAlgorithm, "KMS");
            Assert.AreEqual(bInfo.Bucket.ServerSideEncryptionRule.KMSMasterKeyID, "");

            //SET TO KMS WITH ID
            request = new SetBucketEncryptionRequest(_bucketName, "KMS", "kmsid");
            Assert.AreEqual(request.SSEAlgorithm, "KMS");
            Assert.AreEqual(request.KMSMasterKeyID, "kmsid");
            _ossClient.SetBucketEncryption(request);
            result = _ossClient.GetBucketEncryption(_bucketName);
            Assert.AreEqual(result.SSEAlgorithm, "KMS");
            Assert.AreEqual(result.KMSMasterKeyID, "kmsid");

            bInfo = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(bInfo.Bucket.ServerSideEncryptionRule.SSEAlgorithm, "KMS");
            Assert.AreEqual(bInfo.Bucket.ServerSideEncryptionRule.KMSMasterKeyID, "kmsid");

            //delete 
            _ossClient.DeleteBucketEncryption(_bucketName);
            bInfo = _ossClient.GetBucketInfo(_bucketName);
            Assert.AreEqual(bInfo.Bucket.ServerSideEncryptionRule.SSEAlgorithm, "None");
            Assert.AreEqual(bInfo.Bucket.ServerSideEncryptionRule.KMSMasterKeyID, null);
        }

        [Test]
        public void SetBucketEncryptionWithExceptionTest()
        {
            try
            {
                _ossClient.SetBucketEncryption(null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }
    }
}

