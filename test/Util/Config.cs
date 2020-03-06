using System.Configuration;

namespace Aliyun.OSS.Test.Util
{
    public static class Config
    {
		public static readonly string Endpoint = ConfigurationManager.AppSettings["Endpoint"];
		public static readonly string AccessKeyId = ConfigurationManager.AppSettings["AccessKeyId"];
		public static readonly string AccessKeySecret = ConfigurationManager.AppSettings["AccessKeySecret"];
		public static readonly string ProxyHost = ConfigurationManager.AppSettings["ProxyHost"];
		public static readonly string ProxyPort = ConfigurationManager.AppSettings["ProxyPort"];
        public static readonly string ProxyUser = ConfigurationManager.AppSettings["ProxyUser"];
        public static readonly string ProxyPassword = ConfigurationManager.AppSettings["ProxyPassword"];
        public static readonly string CallbackServer = ConfigurationManager.AppSettings["CallbackServer"];
		public static readonly string DisabledAccessKeyId = ConfigurationManager.AppSettings["DisabledAccessKeyId"];
		public static readonly string DisabledAccessKeySecret = ConfigurationManager.AppSettings["DisabledAccessKeySecret"];
		public static readonly string SecondEndpoint = ConfigurationManager.AppSettings["SecondEndpoint"];
		public static readonly string UploadTestFile = ConfigurationManager.AppSettings["UploadTestFile"];
		public static readonly string MultiUploadTestFile = ConfigurationManager.AppSettings["MultiUploadTestFile"];
        public static readonly string ImageTestFile = ConfigurationManager.AppSettings["ImageTestFile"];
		public static readonly string DownloadFolder = ConfigurationManager.AppSettings["DownloadFolder"];
        public static readonly string PayerAccessKeyId = ConfigurationManager.AppSettings["PayerAccessKeyId"];
        public static readonly string PayerAccessKeySecret = ConfigurationManager.AppSettings["PayerAccessKeySecret"];
        public static readonly string PayerUid = ConfigurationManager.AppSettings["PayerUid"];
    }
}
