/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Util;

namespace Aliyun.OSS.Commands
{
    internal class HeadObjectCommand : OssCommand
    {
        private readonly string _bucketName;
        private readonly string _key;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Head; }
        }

        protected override string Bucket
        {
            get { return _bucketName; }
        }

        protected override string Key
        {
            get { return _key; }
        }

        protected HeadObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                    string bucketName, string key)
            : base(client, endpoint, context)
        {
            OssUtils.CheckBucketName(bucketName);
            OssUtils.CheckObjectKey(key);

            _bucketName = bucketName;
            _key = key;
        }

        public static HeadObjectCommand Create(IServiceClient client, Uri endpoint,
                                              ExecutionContext context,
                                              string bucketName, string key)
        {
            return new HeadObjectCommand(client, endpoint, context, bucketName, key);
        }
    }

    internal class HeadCsvObjectCommand : HeadObjectCommand{
        private HeadCsvObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                  string bucketName, string key)
            : base(client, endpoint, context, bucketName, key)
        {
        }

        public static new HeadCsvObjectCommand Create(IServiceClient client, Uri endpoint,
                                              ExecutionContext context,
                                              string bucketName, string key)
        {
            return new HeadCsvObjectCommand(client, endpoint, context, bucketName, key);
        }

        protected override IDictionary<String, String> Parameters
        {
            get {
                var parameters = base.Parameters;
                parameters[RequestParameters.OSS_PROCESS] = RequestParameters.CSV_META;
                return parameters;
            }
        }

        protected override HttpMethod Method
        {
            get { return HttpMethod.Get; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = base.Headers;
                headers[OssHeaders.SelectInputRecordDelimiter] = "\\r\\n";
                headers[OssHeaders.SelectInputFieldDelimiter] = ",";
                headers[OssHeaders.SelectInputQuoteCharacter] = "\"";
                headers[OssHeaders.SelectInputFileHeader] = SelectObjectRequest.HeaderInfo.Ignore.ToString();
                return headers;
            }
        }
    }
}
