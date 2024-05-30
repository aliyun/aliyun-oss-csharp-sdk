using System;
using System.IO;
namespace Aliyun.OSS.Test.Util
{
    // Please update the values of the following variables before running the test cases;
    public static class Config
    {
        private static string _endpoint = null; // your endpoint
        private static string _region = null; // your endpoint
        private static string _accessKeyId = null; // your access key id
        private static string _accessKeySecret = null; // your access key secret
        private static string _proxyHost = ""; // proxy host
        private static string _proxyPort = "3180";
        private static string _proxyUser = "";
        private static string _proxyPassword = "";
        private static string _callbackServer = null; // callback server
        private static string _payerAccessKeyId = null; // payer access key id
        private static string _payerAccessKeySecret = null; // payer access key secret
        private static string _payerUid = null; // payer uid
        private static string _ramRoleArn = null; // the ram role arn
        private static string _ramUID = null; // the ram role uid

        public static string Endpoint
        {
           get {return _endpoint?? Environment.GetEnvironmentVariable("OSS_TEST_ENDPOINT");}
        }

        public static string Region
        {
           get {return _region?? Environment.GetEnvironmentVariable("OSS_TEST_REGION");}
        }

        public static string AccessKeyId
        {
            get { return _accessKeyId?? Environment.GetEnvironmentVariable("OSS_TEST_ACCESS_KEY_ID"); }
        }

        public static string AccessKeySecret
        {
            get { return _accessKeySecret ?? Environment.GetEnvironmentVariable("OSS_TEST_ACCESS_KEY_SECRET"); }
        }

        public static string ProxyHost
        {
            get { return _proxyHost ?? Environment.GetEnvironmentVariable("OSS_TEST_PROXY_HOST"); }
        }

        public static string ProxyPort
        {
            get { return _proxyPort ?? Environment.GetEnvironmentVariable("OSS_TEST_PROXY_PORT"); }
        }

        public static string ProxyUser
        {
            get { return _proxyUser ?? Environment.GetEnvironmentVariable("OSS_TEST_PROXY_USER"); }
        }

        public static string ProxyPassword
        {
            get { return _proxyPassword ?? Environment.GetEnvironmentVariable("OSS_TEST_PROXY_PASSWORD"); }
        }

        public static string CallbackServer
        {
            get { return _callbackServer ?? Environment.GetEnvironmentVariable("OSS_TEST_CALLBACK_URL"); }
        }

        public static string PayerAccessKeyId
        {
            get { return _payerAccessKeyId ?? Environment.GetEnvironmentVariable("OSS_TEST_PAYER_ACCESS_KEY_ID"); }
        }

        public static string PayerAccessKeySecret
        {
            get { return _payerAccessKeySecret ?? Environment.GetEnvironmentVariable("OSS_TEST_PAYER_ACCESS_KEY_SECRET"); }
        }

        public static string PayerUid
        {
            get { return _payerUid ?? Environment.GetEnvironmentVariable("OSS_TEST_PAYER_UID"); }
        }

        public static string RamRoleArn
        {
            get { return _ramRoleArn ?? Environment.GetEnvironmentVariable("OSS_TEST_RAM_ROLE_ARN"); }
        }

        public static string RamUID
        {
            get { return _ramUID ?? Environment.GetEnvironmentVariable("OSS_TEST_RAM_UID"); }
        }

        //
        private static string _uploadTestFile = null;
        private static string _multiUploadTestFile = null;
        private static string _imageTestFile = null;

        public static string ImageTestFile
        {
            get
            {
                if (string.IsNullOrEmpty(_imageTestFile))
                {
                    _imageTestFile = Path.Combine(DownloadFolder, "example.jpg");
                }
                return _imageTestFile;
            }
        }

        public static string MultiUploadTestFile
        {
            get
            {
                if (string.IsNullOrEmpty(_multiUploadTestFile))
                {
                    _multiUploadTestFile = Path.Combine(DownloadFolder, "testfile-25M.dat");
                    FileUtils.PrepareSampleFile(_multiUploadTestFile, 25*1024);
                }
                return _multiUploadTestFile;
            }
        }

        public static string UploadTestFile
        {
            get
            {
                if (string.IsNullOrEmpty(_uploadTestFile))
                {
                    _uploadTestFile = Path.Combine(DownloadFolder, "testfile-110k.dat");
                    FileUtils.PrepareSampleFile(_uploadTestFile, 110);
                }
                return _uploadTestFile;
            }
        }

        public static string DownloadFolder
        {
            get {
#if TEST_DOTNETCORE
                return Environment.CurrentDirectory;
#else
                return Path.Combine(Environment.CurrentDirectory, "test");
#endif
            }
        }
    }
}
