using Aliyun.OSS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aliyun.OSS.Samples
{
    public static class ProcessObjectSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void ProcessObject(string bucketName, string key, string style, string o, string b =null)
        {
            try
            {
                client.ProcessObject(bucketName, key, style, o, b);

                Console.WriteLine("ProcessObject bucket name:{0} succeeded ", bucketName);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
        }
    }
}
