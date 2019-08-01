using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aliyun.OSS.Commands
{
    internal class ProcessObjectCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly string _key;
        private readonly string _style;
        private readonly string _o;
        private readonly string _b;

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        protected override string Key
        {
            get { return _key; }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, string>();
                parameters[RequestParameters.OSS_PROCESS] = null;
                return parameters;
            }
        }

        protected override Stream Content
        {
            get
            {
                byte[] array = Encoding.ASCII.GetBytes("x-oss-process=" + string.Format("{0}|sys/saveas,o_{1}", _style, Encode(_o)) + _b ?? ("b_" + Encode(_b)));
                MemoryStream stream = new MemoryStream(array);    
                return stream;
            }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                headers[HttpHeaders.ContentLength] = Content.Length.ToString();
                headers["itsvse"] = "application/x-www-form-urlencoded";
                return headers;
            }
        }

        /// <summary>
        /// 字符串编码
        /// </summary>
        /// <param name="text">待编码的文本字符串</param>
        /// <returns>编码的文本字符串.</returns>
        private static string Encode(string text)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(text);
            var base64 = Convert.ToBase64String(plainTextBytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return base64;
        }

        private ProcessObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, string key, string style, string o, string b=null)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(key);

            _bucketName = bucketName;
            _key = key;
            _style = style;
            _o = o;
            _b = b;
            this.ParametersInUri = true;
        }

        public static ProcessObjectCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                                 string bucketName, string key, string style, string o, string b = null)
        {
            return new ProcessObjectCommand(client, endpoint, context, bucketName, key, style, o, b);
        }
    }
}
