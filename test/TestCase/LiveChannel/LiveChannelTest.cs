using System;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.LiveChannelTestClass
{
    [TestFixture]
    public class LiveChannelTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName(_className);
            _ossClient.CreateBucket(_bucketName);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            // Clean up channel
            var result = _ossClient.ListLiveChannel(new ListLiveChannelRequest(_bucketName));
            foreach (var ch in result.LiveChannels)
            {
                _ossClient.DeleteLiveChannel(new DeleteLiveChannelRequest(_bucketName, ch.Name));
            }

            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        [Test]
        public void LiveChannelBasicTest()
        {
            string liveChannelName = "live-channel";
            var clcRequest = new CreateLiveChannelRequest(_bucketName, liveChannelName);
            clcRequest.Description = "just for test";
            clcRequest.Status = "enabled";
            clcRequest.FragDuration = 10;
            clcRequest.FragCount = 5;
            clcRequest.PlaylistName = "playlist1.m3u8";
            var clcResult = _ossClient.CreateLiveChannel(clcRequest);

            Assert.AreEqual(clcResult.PublishUrl.Contains("live/live-channel"), true);
            Assert.AreEqual(clcResult.PlayUrl.Contains("live-channel/playlist1.m3u8"), true);


            var infoResult = _ossClient.GetLiveChannelInfo(new GetLiveChannelInfoRequest(_bucketName, liveChannelName));
            Assert.AreEqual(infoResult.Description, "just for test");
            Assert.AreEqual(infoResult.Status, "enabled");
            Assert.AreEqual(infoResult.Type, "HLS");
            Assert.AreEqual(infoResult.FragDuration, 10);
            Assert.AreEqual(infoResult.FragCount, 5);
            Assert.AreEqual(infoResult.PlaylistName, "playlist1.m3u8");

            _ossClient.SetLiveChannelStatus(new SetLiveChannelStatusRequest(_bucketName, liveChannelName, "disabled"));

            infoResult = _ossClient.GetLiveChannelInfo(new GetLiveChannelInfoRequest(_bucketName, liveChannelName));
            Assert.AreEqual(infoResult.Status, "disabled");

            var statResult = _ossClient.GetLiveChannelStat(new GetLiveChannelStatRequest(_bucketName, liveChannelName));
            Assert.AreEqual(statResult.Status, "Disabled");

            var historyResult = _ossClient.GetLiveChannelHistory(new GetLiveChannelHistoryRequest(_bucketName, liveChannelName));
            var LiveRecords = OssTestUtils.ToArray(historyResult.LiveRecords);
            Assert.AreEqual(LiveRecords.Count, 0);

            var gvpRequest = new GetVodPlaylistRequest(_bucketName, liveChannelName);
            gvpRequest.StartTime = DateTime.Now;
            gvpRequest.EndTime = DateTime.Now.AddMinutes(100);
            try
            {
                var gvpResult = _ossClient.GetVodPlaylist(gvpRequest);
                Assert.IsTrue(false, "should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "InvalidArgument");
                Assert.AreEqual(e.Message, "No ts file found in specified time span.");
            }

            var pvpRequest = new PostVodPlaylistRequest(_bucketName, liveChannelName);
            pvpRequest.PlaylistName = "test-playlist.m3u8";
            pvpRequest.StartTime = DateTime.Now;
            pvpRequest.EndTime = DateTime.Now.AddMinutes(100);
            try
            {
                _ossClient.PostVodPlaylist(pvpRequest);
                Assert.IsTrue(false, "should not here.");
            }
            catch (OssException e)
            {
                Assert.AreEqual(e.ErrorCode, "InvalidArgument");
                Assert.AreEqual(e.Message, "No ts file found in specified time span.");
            }

            _ossClient.DeleteLiveChannel(new DeleteLiveChannelRequest(_bucketName, liveChannelName));
        }


        [Test]
        public void ListLiveChannelTest()
        {
            //prefix:live-channel
            for (int i = 0; i < 5; i++)
            {
                string liveChannelName = "live-channel" + i.ToString();
                var clcRequest = new CreateLiveChannelRequest(_bucketName, liveChannelName);
                var clcResult = _ossClient.CreateLiveChannel(clcRequest);
            }

            //prefix:live-tv
            for (int i = 0; i < 6; i++)
            {
                string liveChannelName = "live-tv" + i.ToString();
                var clcRequest = new CreateLiveChannelRequest(_bucketName, liveChannelName);
                var clcResult = _ossClient.CreateLiveChannel(clcRequest);
            }

            //prefix:vod-channel
            for (int i = 0; i < 3; i++)
            {
                string liveChannelName = "vod-channel" + i.ToString();
                var clcRequest = new CreateLiveChannelRequest(_bucketName, liveChannelName);
                var clcResult = _ossClient.CreateLiveChannel(clcRequest);
            }

            //list live-*
            var request = new ListLiveChannelRequest(_bucketName)
            {
                Prefix = "live-",
            };

            var result = _ossClient.ListLiveChannel(request);
            Assert.AreEqual(result.IsTruncated, false);
            Assert.AreEqual(result.NextMarker, "");
            Assert.AreEqual(result.Marker, "");
            Assert.AreEqual(result.Prefix, "live-");
            var liveChannels = OssTestUtils.ToArray(result.LiveChannels);
            Assert.AreEqual(liveChannels.Count, 11);

            //list vod-channel & with marker vod-channel0 
            request = new ListLiveChannelRequest(_bucketName)
            {
                Prefix = "vod-channel",
                Marker = "vod-channel0",
            };
            result = _ossClient.ListLiveChannel(request);
            Assert.AreEqual(result.IsTruncated, false);
            Assert.AreEqual(result.NextMarker, "");
            Assert.AreEqual(result.Marker, "vod-channel0");
            Assert.AreEqual(result.Prefix, "vod-channel");
            Assert.AreEqual(result.MaxKeys, 100);
            liveChannels = OssTestUtils.ToArray(result.LiveChannels);
            Assert.AreEqual(liveChannels[0].Name, "vod-channel1");
            Assert.AreEqual(liveChannels[1].Name, "vod-channel2");

            //list live-tv 1 by 1
            request = new ListLiveChannelRequest(_bucketName)
            {
                Prefix = "live-tv",
                MaxKeys = 1,
            };
            result = _ossClient.ListLiveChannel(request);
            Assert.AreEqual(result.MaxKeys, 1);
            Assert.AreEqual(result.IsTruncated, true);

            int cnt = 0;
            do
            {
                result = _ossClient.ListLiveChannel(request);
                Assert.AreEqual(result.MaxKeys, 1);
                liveChannels = OssTestUtils.ToArray(result.LiveChannels);
                Assert.AreEqual(liveChannels.Count, 1);
                cnt += liveChannels.Count;
                request.Marker = result.NextMarker;
            } while (result.IsTruncated == true);
            Assert.AreEqual(cnt, 6);
        }

        [Test]
        public void RtmpPresignedUrlTest()
        {
            string liveChannelName = "rtmp-test";
            var request = new CreateLiveChannelRequest(_bucketName, liveChannelName);
            request.Status = "enabled";
            request.FragDuration = 10;
            request.FragCount = 5;
            request.PlaylistName = "playlist.m3u8";
            var result = _ossClient.CreateLiveChannel(request);
            _ossClient.SetLiveChannelStatus(new SetLiveChannelStatusRequest(_bucketName, liveChannelName, "enabled"));
            var stat = _ossClient.GetLiveChannelStat(new GetLiveChannelStatRequest(_bucketName, liveChannelName));
            var uri = _ossClient.GenerateRtmpPresignedUri(new GenerateRtmpPresignedUriRequest(_bucketName, liveChannelName, ""));
            Assert.AreEqual(uri.Scheme, "rtmp");

            var uri1 = _ossClient.GenerateRtmpPresignedUri(new GenerateRtmpPresignedUriRequest(_bucketName, liveChannelName, "test.m3u8"));
            Assert.AreEqual(uri1.Scheme, "rtmp");
            Assert.AreEqual(uri1.ToString().Contains("playlistName=test.m3u8"), true);
        }
    }
}
