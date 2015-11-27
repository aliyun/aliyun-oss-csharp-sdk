namespace Aliyun.OSS.Test.Util
{
    public static class Config
    {
        public static readonly string Endpoint = Properties.Settings.Default.Endpoint;
        public static readonly string AccessKeyId = Properties.Settings.Default.AccessKeyId;
        public static readonly string AccessKeySecret = Properties.Settings.Default.AccessKeySecret;
        public static readonly string ProxyHost = Properties.Settings.Default.ProxyHost;
        public static readonly string ProxyPort = Properties.Settings.Default.ProxyPort;
        public static readonly string DisabledAccessKeyId = Properties.Settings.Default.DisabledAccessKeyId;
        public static readonly string DisabledAccessKeySecret = Properties.Settings.Default.DisabledAccessKeySecret;
        public static readonly string SecondEndpoint = Properties.Settings.Default.SecondEndpoint;
        public static readonly string UploadSampleFile = Properties.Settings.Default.UploadSampleFile;
        public static readonly string MultiUploadSampleFile = Properties.Settings.Default.MultiUploadSampleFile;
        public static readonly string UploadSampleFolder = Properties.Settings.Default.UploadSampleFolder;
        public static readonly string DownloadFolder = Properties.Settings.Default.DownloadFolder;
    }
}
