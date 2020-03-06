using System;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    internal class Config
    {
        public static string AccessKeyId = "LTAISlFUZjVkLuLX";

        public static string AccessKeySecret = "WDwecIlqCQVQxmUFjN432u1mEmDN8P";

        public static string Endpoint = "http://oss-cn-shanghai.aliyuncs.com";

        public static string DirToDownload = "D:\\tmp";

        public static string FileToUpload = "D:\\tmp\\dump_http.pcapng";

        public static string BigFileToUpload = "<your local big file to upload>";
        public static string ImageFileToUpload = "<your local image file to upload>";
        public static string CallbackServer = "<your callback server uri>";
    }
}