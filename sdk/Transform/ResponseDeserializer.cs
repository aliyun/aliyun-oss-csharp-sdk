/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Util;

using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Model;

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

        protected void DeserializeGeneric(ServiceResponse xmlStream, GenericResult result)
        {
            result.HttpStatusCode = xmlStream.StatusCode;

            if (xmlStream.Headers.ContainsKey(HttpHeaders.RequestId))
            {
                result.RequestId = xmlStream.Headers[HttpHeaders.RequestId];
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.ContentLength))
            {
                result.ContentLength = long.Parse(xmlStream.Headers[HttpHeaders.ContentLength]);
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.ServerElapsedTime))
            {
                result.ResponseMetadata.Add(HttpHeaders.ServerElapsedTime, xmlStream.Headers[HttpHeaders.ServerElapsedTime]);
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.Date))
            {
                result.ResponseMetadata.Add(HttpHeaders.Date, xmlStream.Headers[HttpHeaders.Date]);
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.ETag))
            {
                result.ResponseMetadata.Add(HttpHeaders.ETag, OssUtils.TrimQuotes(xmlStream.Headers[HttpHeaders.ETag]));
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.ContentType))
            {
                result.ResponseMetadata.Add(HttpHeaders.ContentType, xmlStream.Headers[HttpHeaders.ContentType]);
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.HashCrc64Ecma))
            {
                result.ResponseMetadata.Add(HttpHeaders.HashCrc64Ecma, xmlStream.Headers[HttpHeaders.HashCrc64Ecma]);
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.ContentMd5))
            {
                result.ResponseMetadata.Add(HttpHeaders.ContentMd5, xmlStream.Headers[HttpHeaders.ContentMd5]);
            }

            if (xmlStream.Headers.ContainsKey(HttpHeaders.QosDelayTime))
            {
                result.ResponseMetadata.Add(HttpHeaders.QosDelayTime, xmlStream.Headers[HttpHeaders.QosDelayTime]);
            }
        }
    }
}
