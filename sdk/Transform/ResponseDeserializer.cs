/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System.IO;
using Aliyun.OSS.Util;

using Aliyun.OSS.Common.Communication;

namespace Aliyun.OSS.Transform
{
    internal abstract class ResponseDeserializer<TResult, TModel> : IDeserializer<ServiceResponse, TResult>
    {
        protected IDeserializer<Stream, TModel> ContentDeserializer { get; private set; }

        public ResponseDeserializer(IDeserializer<Stream, TModel> contentDeserializer)
        {
            ContentDeserializer = contentDeserializer;
        }

        public abstract TResult Deserialize(ServiceResponse xmlStream);

        protected string Decode(string value, string decodeType)
        {
            if (decodeType.Equals(HttpUtils.UrlEncodingType))
            {
                return HttpUtils.DecodeUri(value);
            }
            return value;
        }
    }
}
