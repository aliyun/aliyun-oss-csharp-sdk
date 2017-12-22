using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectAclSettingsTest 
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _keyName;   

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
            //create sample object
            _keyName = OssTestUtils.GetObjectKey(_className);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }


        [Test]
        public void SetObjectAclApiTest() 
        {
            try
            {
                _ossClient.PutObject(_bucketName, _keyName, Config.UploadTestFile);

                // default
                var acl = _ossClient.GetObjectAcl(_bucketName, _keyName);
                Assert.AreEqual(acl.ACL, CannedAccessControlList.Default);

                // public read
                _ossClient.SetObjectAcl(_bucketName, _keyName, CannedAccessControlList.PublicRead);
                OssTestUtils.WaitForCacheExpire();
                acl = _ossClient.GetObjectAcl(_bucketName, _keyName);
                Assert.AreEqual(acl.ACL, CannedAccessControlList.PublicRead);

                // public read and write
                _ossClient.SetObjectAcl(_bucketName, _keyName, CannedAccessControlList.PublicReadWrite);
                OssTestUtils.WaitForCacheExpire();
                acl = _ossClient.GetObjectAcl(_bucketName, _keyName);
                Assert.AreEqual(acl.ACL, CannedAccessControlList.PublicReadWrite);

                // private
                _ossClient.SetObjectAcl(_bucketName, _keyName, CannedAccessControlList.Private);
                OssTestUtils.WaitForCacheExpire();
                acl = _ossClient.GetObjectAcl(_bucketName, _keyName);
                Assert.AreEqual(acl.ACL, CannedAccessControlList.Private);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, _keyName);
            }
        }

        [Test]
        public void SetObjectAclUseRequestTest()
        {
            try
            {
                _ossClient.PutObject(_bucketName, _keyName, Config.UploadTestFile);

                // default
                var acl = _ossClient.GetObjectAcl(_bucketName, _keyName);
                Assert.AreEqual(acl.ACL, CannedAccessControlList.Default);

                // public read
                _ossClient.SetObjectAcl(new SetObjectAclRequest(_bucketName, _keyName, CannedAccessControlList.PublicRead));
                OssTestUtils.WaitForCacheExpire();
                acl = _ossClient.GetObjectAcl(_bucketName, _keyName);
                Assert.AreEqual(acl.ACL, CannedAccessControlList.PublicRead);

                // public read and write
                _ossClient.SetObjectAcl(new SetObjectAclRequest(_bucketName, _keyName, CannedAccessControlList.PublicReadWrite));
                OssTestUtils.WaitForCacheExpire();
                acl = _ossClient.GetObjectAcl(_bucketName, _keyName);
                Assert.AreEqual(acl.ACL, CannedAccessControlList.PublicReadWrite);

                // private
                _ossClient.SetObjectAcl(new SetObjectAclRequest(_bucketName, _keyName, CannedAccessControlList.Private));
                OssTestUtils.WaitForCacheExpire();
                acl = _ossClient.GetObjectAcl(_bucketName, _keyName);
                Assert.AreEqual(acl.ACL, CannedAccessControlList.Private);
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, _keyName);
            }
        }

    }
}
