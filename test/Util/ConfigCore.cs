using System.Configuration;
using System;
using System.IO;
namespace Aliyun.OSS.Test.Util
{
    // Please update the values of the following variables before running the test cases;
    public static class Config
    {
        public static string Endpoint = "http://oss-cn-shenzhen.aliyuncs.com";  // your endpoint
        public static string AccessKeyId = "LTAISlFUZjVkLuLX"; // your access key id
        public static string AccessKeySecret = "WDwecIlqCQVQxmUFjN432u1mEmDN8P"; // your access key secret
        public static string ProxyHost = "";   // proxy host
        public static string ProxyPort = "3128";
        public static string ProxyUser = "";
        public static string ProxyPassword = "";
        public static string CallbackServer = "http://47.106.170.239:7090";  // callback server
        public static string DisabledAccessKeyId = "DisabledAccessKeyId";
        public static string DisabledAccessKeySecret = "DisabledAccessKeySecret";
        public static string SecondEndpoint = "http://oss-cn-hangzhou.aliyuncs.com";
        public static string UploadTestFile = "D:\\oss\\test";  // path to upload test file
        public static string MultiUploadTestFile = "D:\\oss\\test";  // path to multi upload test file--it should be 1MB or bigger
        public static string ImageTestFile = "D:\\oss\\test\\example.jpg"; // path to upload test image file
        public static string DownloadFolder = "D:\\oss\\test"; // path to download folder
        public static string PayerAccessKeyId = "LTAIpbO7JYL3xnB5";  // path to multi upload test file--it should be 1MB or bigger
        public static string PayerAccessKeySecret = "nzktzfwJPD13RRnUTuKYaLvCGJ0Zm0"; // path to upload test image file
        public static string PayerUid = "208987761725310137"; // path to download folder
    }

    internal class EnvConfig
    {
        public EnvConfig()
        {
            loadFromEnv();
        }

        private void loadFromEnv()
        {
            if (Config.Endpoint == "")
            {
                Config.Endpoint = Environment.GetEnvironmentVariable("OSS_TEST_ENDPOINT");
            }

            if (Config.AccessKeyId == "")
            {
                Config.AccessKeyId = Environment.GetEnvironmentVariable("OSS_TEST_ACCESS_KEY_ID");
            }

            if (Config.AccessKeySecret == "")
            {
                Config.AccessKeySecret = Environment.GetEnvironmentVariable("OSS_TEST_ACCESS_KEY_SECRET");
            }
        }
    }
}
