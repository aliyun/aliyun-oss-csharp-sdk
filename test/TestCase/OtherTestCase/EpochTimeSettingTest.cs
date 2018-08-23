using System;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.OtherTestClass
{
    [TestFixture]
    public class EpochTimeSettingTest
    {
        private static IOss _ossClient;

        private static readonly DateTime EpochTime = new DateTime(1970, 1, 1);

#if NETCOREAPP2_0
        [OneTimeSetUp]
#else
        [TestFixtureSetUp]
#endif
        public static void ClassInitialize()
        {
            //TODO: 
        }

#if NETCOREAPP2_0
        [OneTimeTearDown]
#else
        [TestFixtureTearDown]
#endif
        public static void ClassCleanup()
        {
            //TODO: 
        }

        [Test]
        public void EpochTimeSettingNormalTest()
        {
            ClientConfiguration conf = new ClientConfiguration();

            // Set custom time-adjust to local time on the assumption that
            // the local time is correct standard BEIJING time.
            var timeSpan = DateTime.UtcNow.Subtract(EpochTime);
            var localTicks = (long)timeSpan.TotalSeconds;
            conf.SetCustomEpochTicks(localTicks);
            AssertRequestTimeValidatity(conf);

            // Set custom time-adjust to local time after substracting 10 minutes.
            timeSpan = DateTime.UtcNow.AddMinutes(-10).Subtract(EpochTime);
            localTicks = (long)timeSpan.TotalSeconds;
            conf.SetCustomEpochTicks(localTicks);
            AssertRequestTimeValidatity(conf);

            // Set custom time-adjust to local time after adding 10 minutes.
            timeSpan = DateTime.UtcNow.AddMinutes(10).Subtract(EpochTime);
            localTicks = (long)timeSpan.TotalSeconds;
            conf.SetCustomEpochTicks(localTicks);
            AssertRequestTimeValidatity(conf);
        }

        private void AssertRequestTimeValidatity(ClientConfiguration conf)
        {
            var uri = new Uri(Config.Endpoint.ToLower().Trim().StartsWith("http") ? 
                Config.Endpoint.Trim() : "http://" + Config.Endpoint.Trim());
            _ossClient = new OssClient(uri, Config.AccessKeyId, Config.AccessKeySecret, conf);
            try
            {
                _ossClient.ListBuckets();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void EpochTimeSettingUnormalTest()
        {
            ClientConfiguration conf = new ClientConfiguration();

            // Set custom time-adjust to local time after substracting 30 minutes,
            // Note that valid time offset is 15 minutes.
            var timeSpan = DateTime.UtcNow.AddMinutes(-30).Subtract(EpochTime);
            var localTicks = (long)timeSpan.TotalSeconds;
            conf.SetCustomEpochTicks(localTicks);
            AssertRequestTimeTooSkewed(conf);

            // Set custom time-adjust to local time after adding 30 minutes,
            // Note that valid time offset is 15 minutes.
            timeSpan = DateTime.UtcNow.AddMinutes(30).Subtract(EpochTime);
            localTicks = (long)timeSpan.TotalSeconds;
            conf.SetCustomEpochTicks(localTicks);
            AssertRequestTimeTooSkewed(conf);
        }

        private void AssertRequestTimeTooSkewed(ClientConfiguration conf)
        {
            var uri = new Uri(Config.Endpoint.ToLower().Trim().StartsWith("http") ?
                Config.Endpoint.Trim() : "http://" + Config.Endpoint.Trim());
            _ossClient = new OssClient(uri, Config.AccessKeyId, Config.AccessKeySecret, conf);
            try
            {
                _ossClient.ListBuckets();
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.RequestTimeTooSkewed, e.ErrorCode);
            }
        }
    }
}
