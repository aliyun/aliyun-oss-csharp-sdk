using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    [TestFixture]
    public partial class BucketSettingsTest
    {

        [Test]
        public void SetBucketAclApiTest()
        {
            //set to read
            _ossClient.SetBucketAcl(_bucketName, CannedAccessControlList.PublicRead);
            OssTestUtils.WaitForCacheExpire();
            var acl = _ossClient.GetBucketAcl(_bucketName);
            Assert.AreEqual(acl.ACL, CannedAccessControlList.PublicRead);

            //set to readwrite
            _ossClient.SetBucketAcl(_bucketName, CannedAccessControlList.PublicReadWrite);
            OssTestUtils.WaitForCacheExpire();
            acl = _ossClient.GetBucketAcl(_bucketName);
            Assert.AreEqual(acl.ACL, CannedAccessControlList.PublicReadWrite);

            //set to private
            _ossClient.SetBucketAcl(_bucketName, CannedAccessControlList.Private);
            OssTestUtils.WaitForCacheExpire();
            acl = _ossClient.GetBucketAcl(_bucketName);
            Assert.AreEqual(acl.ACL, CannedAccessControlList.Private);
        }

        [Test]
        public void SetBucketAclUseRequestTest()
        {
            _ossClient.SetBucketAcl(
                new SetBucketAclRequest(_bucketName, CannedAccessControlList.PublicRead));
            OssTestUtils.WaitForCacheExpire();
            var acl = _ossClient.GetBucketAcl(_bucketName);
            Assert.AreEqual(acl.ACL, CannedAccessControlList.PublicRead);

            //set to readwrite
            _ossClient.SetBucketAcl(
                new SetBucketAclRequest(_bucketName, CannedAccessControlList.PublicReadWrite));
            OssTestUtils.WaitForCacheExpire();
            acl = _ossClient.GetBucketAcl(_bucketName);
            Assert.AreEqual(acl.ACL, CannedAccessControlList.PublicReadWrite);

            //set to private
            _ossClient.SetBucketAcl(
                new SetBucketAclRequest(_bucketName, CannedAccessControlList.Private));
            OssTestUtils.WaitForCacheExpire();
            acl = _ossClient.GetBucketAcl(_bucketName);
            Assert.AreEqual(acl.ACL, CannedAccessControlList.Private);
            Assert.AreEqual(acl.ToString().IndexOf("ACL=Private") != -1, true);
        }

        [Test]
        public void SetBucketAclUseDefaultTest()
        {
            try
            {
                _ossClient.SetBucketAcl(
                    new SetBucketAclRequest(_bucketName, CannedAccessControlList.Default));
                Assert.Fail("Bucket creation should fail ");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.InvalidArgument, e.ErrorCode);
            }
        }
    }
}
