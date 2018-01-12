using Aliyun.OSS;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    [TestFixture]
    public partial class BucketSettingsTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;

#if UNITY_5_3_OR_NEWER
        [OneTimeSetUp]
#else
        [TestFixtureSetUp]
#endif
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            //prefix of bucket name used in current test class
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName(_className);
            _ossClient.CreateBucket(_bucketName);
        }

#if UNITY_5_3_OR_NEWER
        [OneTimeTearDown]
#else
        [TestFixtureTearDown]
#endif
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }
    }
}
