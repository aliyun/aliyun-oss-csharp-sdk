using System;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    internal class Config
    {
        public static string AccessKeyId = "";

        public static string AccessKeySecret = "";

        public static string Endpoint = "";

        public static string DirToDownload = "";

        public static string FileToUpload = "";

        public static string BigFileToUpload = "<your local big file to upload>";
        public static string ImageFileToUpload = "<your local image file to upload>";
        public static string CallbackServer = "<your callback server uri>";
    }
}