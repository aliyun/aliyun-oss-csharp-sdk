/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.IO;
using Aliyun.OSS.Util;
using Aliyun.OSS.Model;
using Aliyun.OSS.Domain;
using System;
using System.Text;
namespace Aliyun.OSS.Transform
{
    internal class SelectObjectRequestSerializer : RequestSerializer<SelectObjectRequest, SelectObjectRequestModel>
    {
        public SelectObjectRequestSerializer(ISerializer<SelectObjectRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(SelectObjectRequest request)
        {
            var model = new SelectObjectRequestModel
            {
                Expression = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Expression)),
                Option = new SelectObjectRequestModel.Options()
            };

            if (request.InputFormatType == InputFormatTypes.CSV)
            {
                model.InputFormats = new SelectObjectRequestModel.InputFormat();
                model.InputFormats.CompressionTypes = request.InputFormats.CompressionTypes;
                model.InputFormats.CSVs = new SelectObjectRequestModel.InputFormat.InputCSV();
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

                model.InputFormats.CSVs.FileHeaderInfos = request.InputFormats.CSVs.FileHeaderInfos;

                if (request.InputFormats.CSVs.CommentCharacter == null)
                {
                    model.InputFormats.CSVs.CommentCharacter = Convert.ToBase64String(Encoding.UTF8.GetBytes(""));
                }
                else
                {
                    model.InputFormats.CSVs.CommentCharacter = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.InputFormats.CSVs.CommentCharacter.Substring(0, 1)));
                }
                if (request.lineRangeIsSet)
                {
                    model.InputFormats.CSVs.Range = "line-range=" + request.lineRangeStart.ToString() + "-" + request.lineRangeEnd.ToString();
                }
                else if (request.splitRangeIsSet)
                {
                    model.InputFormats.CSVs.Range = "split-range=" + request.splitRangeStart.ToString() + "-" + request.splitRangeEnd.ToString();
                }

                model.OutputFormats = new SelectObjectRequestModel.OutputFormat();
                model.OutputFormats.CSVs = new SelectObjectRequestModel.OutputFormat.OutputCSV();

                if (request.OutputFormats.CSVs.FieldDelimiter == null)
                {
                    model.OutputFormats.CSVs.FieldDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(""));
                }
                else
                {
                    model.OutputFormats.CSVs.FieldDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.OutputFormats.CSVs.FieldDelimiter.Substring(0, 1)));
                }
                model.OutputFormats.CSVs.RecordDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.OutputFormats.CSVs.RecordDelimiter));

                model.OutputFormats.KeepAllColumns = request.OutputFormats.KeepAllColumns;
                model.OutputFormats.OutputRawData = request.OutputFormats.OutputRawData;
                model.OutputFormats.OutputHeader = request.OutputFormats.OutputHeader;
                model.OutputFormats.EnablePayloadCrc = request.OutputFormats.EnablePayloadCrc;

            }
            else
            {
                model.InputFormats = new SelectObjectRequestModel.InputFormat();
                model.InputFormats.CompressionTypes = request.InputFormats.CompressionTypes;
                model.InputFormats.JSONInputs = new SelectObjectRequestModel.InputFormat.JSONInput();
                model.InputFormats.JSONInputs.Types = request.InputFormats.JSONInputs.Types;
                model.InputFormats.JSONInputs.ParseJsonNumberAsString = request.InputFormats.JSONInputs.ParseJsonNumberAsString;

                if (request.lineRangeIsSet)
                {
                    model.InputFormats.JSONInputs.Range = "line-range=" + request.lineRangeStart.ToString() + "-" + request.lineRangeEnd.ToString();
                }
                else if (request.splitRangeIsSet)
                {
                    model.InputFormats.JSONInputs.Range = "split-range=" + request.splitRangeStart.ToString() + "-" + request.splitRangeEnd.ToString();
                }

                model.OutputFormats = new SelectObjectRequestModel.OutputFormat();
                model.OutputFormats.JSONOutputs = new SelectObjectRequestModel.OutputFormat.JSONOutput();
                model.OutputFormats.JSONOutputs.RecordDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.OutputFormats.JSONOutputs.RecordDelimiter));

                model.OutputFormats.KeepAllColumns = request.OutputFormats.KeepAllColumns;
                model.OutputFormats.OutputRawData = request.OutputFormats.OutputRawData;
                model.OutputFormats.OutputHeader = request.OutputFormats.OutputHeader;
                model.OutputFormats.EnablePayloadCrc = request.OutputFormats.EnablePayloadCrc;
            }

            model.Option.MaxSkippedRecordsAllowed = request.MaxSkippedRecordsAllowed;
            model.Option.SkipPartialDataRecord = request.SkipPartialDataRecord;

            return ContentSerializer.Serialize(model);
        }
    }
}

