using System.Configuration;
using System;
using System.IO;
namespace Aliyun.OSS.Test.Util
{
    // Please update the values of the following variables before running the test cases;
    public static class Config
    {
        public static readonly string Endpoint = "10.101.200.203:8086";
        public static readonly string AccessKeyId = "LTAIJPXxMLocA0fD";
        public static readonly string AccessKeySecret = "l8SjZPosYFR8cHn7jR05SOUFd2u0T7";
        public static readonly string ProxyHost = "";
        public static readonly string ProxyPort = "";
        public static readonly string ProxyUser = "";
        public static readonly string ProxyPassword = "";
        public static readonly string CallbackServer = "";
        public static readonly string DisabledAccessKeyId = "";
        public static readonly string DisabledAccessKeySecret = "";
        public static readonly string SecondEndpoint = "";
        public static readonly string UploadTestFile = "/Users/qi.xu/Downloads/wikiticker-index.s3.json";
        public static readonly string MultiUploadTestFile = "/Users/qi.xu/Documents/存储产品-日志服务-v1.pptx";
        public static readonly string ImageTestFile = "/Users/qi.xu/Downloads/self.jpg";
        public static readonly string DownloadFolder = "/Users/qi.xu/Downloads/CSharp";
    }
}
