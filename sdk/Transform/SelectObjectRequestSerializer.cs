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
    internal class SelectObjectRequestSerializer : RequestSerializer<SelectObjectRequest, SelectObjectRequestModel>
    {
        public SelectObjectRequestSerializer(ISerializer<SelectObjectRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(SelectObjectRequest request)
        {
            var model = new SelectObjectRequestModel
            {
                Expression = string.IsNullOrEmpty(request.Expression)? "":Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Expression)),
            };

            //input
            if (request.InputFormat != null)
            {
                model.InputFormat = new SelectObjectRequestModel.InputFormatModel();
                model.InputFormat.CompressionType = request.InputFormat.CompressionType;

                if (request.InputFormat.GetType() == typeof(SelectObjectCSVInputFormat))
                {
                    var inputFormat = (SelectObjectCSVInputFormat)request.InputFormat;
                    model.InputFormat.CSV = new SelectObjectRequestModel.InputFormatModel.InputCSV();

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

                    if (!string.IsNullOrEmpty(inputFormat.CommentCharacter))
                    {
                        model.InputFormat.CSV.CommentCharacter = Convert.ToBase64String(Encoding.UTF8.GetBytes(inputFormat.CommentCharacter));
                    }

                    if (!string.IsNullOrEmpty(inputFormat.Range))
                    {
                        model.InputFormat.CSV.Range = inputFormat.Range;
                    }

                    model.InputFormat.CSV.FileHeaderInfo = inputFormat.FileHeaderInfo;

                    model.InputFormat.CSV.AllowQuotedRecordDelimiter = inputFormat.AllowQuotedRecordDelimiter;
                }
                else if (request.InputFormat.GetType() == typeof(SelectObjectJSONInputFormat))
                {
                    var inputFormat = (SelectObjectJSONInputFormat)request.InputFormat;
                    model.InputFormat.JSON = new SelectObjectRequestModel.InputFormatModel.InputJSON();
                    model.InputFormat.JSON.Type = inputFormat.Type;
                    model.InputFormat.JSON.Range = inputFormat.Range;
                    model.InputFormat.JSON.ParseJsonNumberAsString = inputFormat.ParseJsonNumberAsString;
                }
            }

            //output
            if (request.OutputFormat != null)
            {
                model.OutputFormat = new SelectObjectRequestModel.OutputFormatModel();

                model.OutputFormat.EnablePayloadCrc = request.OutputFormat.EnablePayloadCrc;
                model.OutputFormat.OutputRawData = request.OutputFormat.OutputRawData;

                if (request.OutputFormat.GetType() == typeof(SelectObjectCSVOutputFormat))
                {
                    var outputFormat = (SelectObjectCSVOutputFormat)request.OutputFormat;
                    model.OutputFormat.CSV = new SelectObjectRequestModel.OutputFormatModel.OutputCSV();

                    if (!string.IsNullOrEmpty(outputFormat.RecordDelimiter))
                    {
                        model.OutputFormat.CSV.RecordDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(outputFormat.RecordDelimiter));
                    }

                    if (!string.IsNullOrEmpty(outputFormat.FieldDelimiter))
                    {
                        model.OutputFormat.CSV.FieldDelimiter = Convert.ToBase64String(Encoding.UTF8.GetBytes(outputFormat.FieldDelimiter));
                    }

                    model.OutputFormat.KeepAllColumns = outputFormat.KeepAllColumns;
                    model.OutputFormat.OutputHeader = outputFormat.OutputHeader;
                }
                else if (request.InputFormat.GetType() == typeof(SelectObjectJSONOutputFormat))
                {
                    var outputFormat = (SelectObjectJSONOutputFormat)request.OutputFormat;
                    model.OutputFormat.JSON = new SelectObjectRequestModel.OutputFormatModel.OutputJSON();
                    model.OutputFormat.JSON.RecordDelimiter = outputFormat.RecordDelimiter;
                }
            }

            //options
            if (request.Options != null)
            {
                model.Options = new SelectObjectRequestModel.OptionsModel();
                model.Options.MaxSkippedRecordsAllowed = request.Options.MaxSkippedRecordsAllowed;
                model.Options.SkipPartialDataRecord = request.Options.SkipPartialDataRecord;
            }
            
            return ContentSerializer.Serialize(model);
        }
    }
}

