/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Model;
using System;
using System.Text;
namespace Aliyun.OSS.Transform
{
    internal class SelectObjectCsvMetaRequestSerializer : RequestSerializer<CreateSelectObjectMetaRequest, CsvMetaRequestModel>
    {
        public SelectObjectCsvMetaRequestSerializer(ISerializer<CsvMetaRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(CreateSelectObjectMetaRequest request)
        {
            var model = new CsvMetaRequestModel();

            var inputFormat = (CreateSelectObjectMetaCSVInputFormat)request.InputFormat;

            model.InputFormat = new CreateSelectObjectMetaInputFormatModel();
            model.InputFormat.CompressionTypeInfo = request.InputFormat.CompressionType;

            model.InputFormat.CSV = new CreateSelectObjectMetaInputFormatModel.CSVModel();

            if (!string.IsNullOrEmpty(inputFormat.RecordDelimiter))
            {
                model.InputFormat.CSV.RecordDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(inputFormat.RecordDelimiter));
            }

            if (!string.IsNullOrEmpty(inputFormat.FieldDelimiter))
            {
                model.InputFormat.CSV.FieldDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(inputFormat.FieldDelimiter));
            }

            if (!string.IsNullOrEmpty(inputFormat.QuoteCharacter))
            {
                model.InputFormat.CSV.QuoteCharacter = Convert.ToBase64String(Encoding.UTF8.GetBytes(inputFormat.QuoteCharacter));
            }

            model.OverwriteIfExists = request.OverwriteIfExists;

            return ContentSerializer.Serialize(model);
        }
    }
}

