using System.Configuration;
using System;
using System.IO;
namespace Aliyun.OSS.Test.Util
{
    // Please update the values of the following variables before running the test cases;
    public static class Config
    {
        public static readonly string Endpoint = "";  // your endpoint
        public static readonly string AccessKeyId = ""; // your access key id
        public static readonly string AccessKeySecret = ""; // your access key secret
        public static readonly string ProxyHost = "";   // proxy host
        public static readonly string ProxyPort = "3128";
        public static readonly string ProxyUser = "";
        public static readonly string ProxyPassword = "";
        public static readonly string CallbackServer = "";  // callback server
        public static readonly string DisabledAccessKeyId = "";
        public static readonly string DisabledAccessKeySecret = "";
        public static readonly string SecondEndpoint = "";
        public static readonly string UploadTestFile = "";  // path to upload test file
        public static readonly string MultiUploadTestFile = "";  // path to multi upload test file--it should be 1MB or bigger
        public static readonly string ImageTestFile = ""; // path to upload test image file
        public static readonly string DownloadFolder = ""; // path to download folder
        public static readonly string PayerAccessKeyId = "";  // payer access key id
        public static readonly string PayerAccessKeySecret = ""; // payer access key secret
        public static readonly string PayerUid = ""; // payer uid
    }
}
