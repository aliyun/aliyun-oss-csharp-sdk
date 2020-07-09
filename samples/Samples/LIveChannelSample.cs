/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.IO;
using System.Net;
using Aliyun.OSS.Common;
using System.Text;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for the usage of Live Channel.
    /// </summary>
    public static class LiveChannelSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void LiveChannel(string bucketName)
        {
            CreateLiveChannelAndPushStream(bucketName);
            BuildVodPlaylist(bucketName);
        }

        public static void CreateLiveChannelAndPushStream(string bucketName)
        {
            string liveChannelName = "rtmp-test";
            try
            {
                //create a live channel by using default paramters
                var result = client.CreateLiveChannel(new CreateLiveChannelRequest(bucketName, liveChannelName));

                //enable live channel 
                client.SetLiveChannelStatus(new SetLiveChannelStatusRequest(bucketName, liveChannelName, "enabled"));

                //build a presigned push url
                var uri = client.GenerateRtmpPresignedUri(new GenerateRtmpPresignedUriRequest(bucketName, liveChannelName, "playlist.m3u8"));

                Console.WriteLine("push url: {0}", uri.ToString());

                //use ffmpeg to push stream 
                //ffmpeg -re -i demo.mp4 -c copy -f flv "rtmp://...."
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }

        public static void BuildVodPlaylist(string bucketName)
        {
            string liveChannelName = "rtmp-test";
            try
            {
                var request = new PostVodPlaylistRequest(bucketName, liveChannelName);
                request.PlaylistName = "vod.m3u8";
                request.StartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(1594286406);
                request.EndTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(1594286524);
                client.PostVodPlaylist(request);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }
    }
}
