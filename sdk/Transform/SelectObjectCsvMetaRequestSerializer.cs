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

            model.InputFormats = new CreateSelectObjectMetaRequestModel.InputFormat();

            model.InputFormats.CompressionTypes = request.InputFormats.CompressionTypes;
            model.InputFormats.CSVs = new CreateSelectObjectMetaRequestModel.InputFormat.InputCSV();

            model.InputFormats.CSVs.RecordDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.InputFormats.CSVs.RecordDelimiter));

            if (request.InputFormats.CSVs.FieldDelimiter == null)
            {
                model.InputFormats.CSVs.FieldDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(""));
            }
            else
            {
                model.InputFormats.CSVs.FieldDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.InputFormats.CSVs.FieldDelimiter.Substring(0, 1)));
            }

            if (request.InputFormats.CSVs.QuoteCharacter == null)
            {
                model.InputFormats.CSVs.QuoteCharacter = Convert.ToBase64String(Encoding.UTF8.GetBytes(""));
            }
            else
            {
                model.InputFormats.CSVs.QuoteCharacter = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.InputFormats.CSVs.QuoteCharacter.Substring(0, 1)));
            }

            model.OverwriteIfExists = request.OverwriteIfExists;

            return ContentSerializer.Serialize(model);
        }
    }
}

