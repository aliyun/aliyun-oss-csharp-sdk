/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Util;
using Aliyun.OSS.Model;

namespace Aliyun.OSS.Transform
{
    internal class SelectObjectRequestSerializer : RequestSerializer<SelectObjectRequest, SelectObjectRequestModel>
    {
        public SelectObjectRequestSerializer(ISerializer<SelectObjectRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(SelectObjectRequest request)
        {
            SelectObjectRequestModel requestModel = new SelectObjectRequestModel();
            requestModel.Expression = request.Sql;
            requestModel.InputSerialization = new InputSerializationModel();
            requestModel.InputSerialization.Csv = new CsvInputFormat()
            {
                RecordDelimiter = NewLineToString(request.InputNewLine),
                FieldDelimiter = request.InputDelimiter.ToString(),
                QuoteCharacter = request.InputQuote.ToString(),
                QuoteEscapeCharacter = request.InputQuote.ToString(),
                Comments = request.InputComment.ToString(),
                FileHeaderInfo = request.Header
            };

            requestModel.OutputSerialization = new OutputSerializationModel();
            requestModel.OutputSerialization.Csv = new CsvOutputFormat()
            {
                QuoteFields = request.OutputQuoteFields.ToString(),
                FieldDelimiter = request.OutputDelimiter.ToString(),
                RecordDelimiter = NewLineToString(request.OutputNewLine),
                QuoteCharacter = request.OutputQuote.ToString(),
                QuoteEscapeCharacter = request.OutputQuote.ToString()
            };

            return ContentSerializer.Serialize(requestModel);
        }

        private string NewLineToString(char c)
        {
            switch(c)
            {
                case '\n':
                    return "\\n";
                case '\r':
                    return "\\r";
                default:
                    return c.ToString();
            }
        }
    }
}
