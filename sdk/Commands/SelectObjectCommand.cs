/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Handlers;
using Aliyun.OSS.Util;
using Aliyun.OSS.Transform;

namespace Aliyun.OSS.Commands
{
    internal class SelectObjectCommand : OssCommand<OssObject>
    {
        private readonly SelectObjectRequest _selectObjectRequest;

        protected override HttpMethod Method
        {
            get { return HttpMethod.Get; }
        }

        protected override string Bucket
        {
            get { return _selectObjectRequest.BucketName; }
        }

        protected override string Key
        {
            get { return _selectObjectRequest.Key; }
        }

        protected override IDictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                _selectObjectRequest.Populate(headers);
                headers.Add(OssHeaders.SelectInputFileHeader, _selectObjectRequest.Header.ToString());
                headers.Add(OssHeaders.SelectInputCompression, _selectObjectRequest.Compression.ToString());
                headers.Add(OssHeaders.SelectInputFieldDelimiter, _selectObjectRequest.InputDelimiter.ToString());
                headers.Add(OssHeaders.SelectInputRecordDelimiter, GetNewLineString(_selectObjectRequest.InputNewLine));
                headers.Add(OssHeaders.SelectInputQuoteCharacter, _selectObjectRequest.InputQuote.ToString());
                headers.Add(OssHeaders.SelectOutputFieldDelimiter, _selectObjectRequest.OutputDelimiter.ToString());
                headers.Add(OssHeaders.SelectOutputQuoteCharacter, _selectObjectRequest.OutputQuote.ToString());
                headers.Add(OssHeaders.SelectOutputRecordDelimiter, GetNewLineString(_selectObjectRequest.OutputNewLine));
                headers.Add(OssHeaders.SelectOutputQuoteFields, _selectObjectRequest.OutputQuoteFields.ToString());
                headers.Add(OssHeaders.SelectOutputKeepAllColumns, _selectObjectRequest.KeepAllColumns.ToString());
                return headers;
            }
        }

        protected override IDictionary<string, string> Parameters
        {
            get
            {
                /*return new Dictionary<string, string>()
                {
                    { RequestParameters.SUBRESOURCE_SELECT, null }
                };*/

                return new Dictionary<string, string>()
                {
                    {RequestParameters.OSS_PROCESS, RequestParameters.CSV_SELECT},
                    {RequestParameters.SQL, _selectObjectRequest.Sql}
                };
            }
        }

        protected override Stream Content
        {
            get
            {
                return null;
                //return SerializerFactory.GetFactory().CreateSelectObjectRequestSerializer()
                                        //.Serialize(_selectObjectRequest);
            }
        }

        protected override bool LeaveResponseOpen
        {
            get { return true; }
        }

        private SelectObjectCommand(IServiceClient client, Uri endpoint, ExecutionContext context,
                                 IDeserializer<ServiceResponse, OssObject> deserializer,
                                 SelectObjectRequest selectObjectRequest)
            : base(client, endpoint, context, deserializer)
        {
            _selectObjectRequest = selectObjectRequest;
        }

        private string GetNewLineString(char newLine){
            switch(newLine)
            {
                case '\n':
                    return "\\n";
                case '\t':
                    return "\\t";
                case '\v':
                    return "\\v";
                case '\r':
                    return "\\r";
                default:
                    return newLine.ToString();
            }
        }

        public static SelectObjectCommand Create(IServiceClient client, Uri endpoint, ExecutionContext context,
                                              SelectObjectRequest selectObjectRequest)
        {
            OssUtils.CheckBucketName(selectObjectRequest.BucketName);
            OssUtils.CheckObjectKey(selectObjectRequest.Key);

            return new SelectObjectCommand(client, endpoint, context,
                                 DeserializerFactory.GetFactory().CreateSelectObjectResultDeserializer(selectObjectRequest, client),
                                 selectObjectRequest);
        }
    }
}
