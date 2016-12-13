using System;

namespace Aliyun.OSS.Test.Util
{
    internal class AccountSettings
    {
        public string OssEndpoint { get; set; }
        public string OssAccessKeyId { get; set; }
        public string OssAccessKeySecret { get; set; }

        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }

        private AccountSettings()
        {
        }

        public static AccountSettings Load()
        {
            //setup an AccountSettings object with endpoint, accessKeyId, and accessKeySecret
            var accountSettings = new AccountSettings
            {
                OssEndpoint = Config.Endpoint,
                OssAccessKeyId = Config.AccessKeyId,
                OssAccessKeySecret = Config.AccessKeySecret
            };
            //check if proxyHost and proxyPort has been configured, if yes, set proxyHost and proxyPort values
            if (string.IsNullOrEmpty(Config.ProxyHost) || string.IsNullOrEmpty(Config.ProxyPort))
                return accountSettings;

            accountSettings.ProxyHost = Config.ProxyHost;
            int proxyPort;
            if (!Int32.TryParse(Config.ProxyPort, out proxyPort))
            {
                throw new Exception("ProxyPort in configuration is not configured as a valid integer");
            }
            accountSettings.ProxyPort = proxyPort;
            accountSettings.ProxyUser = Config.ProxyUser;
            accountSettings.ProxyPassword = Config.ProxyPassword;

            return accountSettings;
        }
    }
}
