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
		public static readonly string DisabledAccessKeyId = ConfigurationManager.AppSettings["DisabledAccessKeyId"];
		public static readonly string DisabledAccessKeySecret = ConfigurationManager.AppSettings["DisabledAccessKeySecret"];
		public static readonly string SecondEndpoint = ConfigurationManager.AppSettings["SecondEndpoint"];
		public static readonly string UploadSampleFile = ConfigurationManager.AppSettings["UploadSampleFile"];
		public static readonly string MultiUploadSampleFile = ConfigurationManager.AppSettings["MultiUploadSampleFile"];
		public static readonly string DownloadFolder = ConfigurationManager.AppSettings["DownloadFolder"];
    }
}
