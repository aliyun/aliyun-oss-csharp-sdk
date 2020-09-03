/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("SelectRequest")]
    public class SelectObjectRequestModel
    {
        [XmlElement("Expression")]
        public string Expression { get; set; }

        [XmlElement("InputSerialization", IsNullable = true)]
        public InputFormatModel InputFormat { get; set; }
        public bool ShouldSerializeInputFormat()
        {
            return InputFormat != null;
        }

        [XmlElement("OutputSerialization", IsNullable = true)]
        public OutputFormatModel OutputFormat { get; set; }
        public bool ShouldSerializeOutputFormat()
        {
            return OutputFormat != null;
        }

        [XmlElement("Options", IsNullable = true)]
        public OptionsModel Options { get; set; }
        public bool ShouldSerializeOptions()
        {
            return Options != null;
        }

        public class InputFormatModel
        {
            [XmlElement("CompressionType")]
            public CompressionType CompressionType { get; set; }

            [XmlElement("CSV")]
            public InputCSV CSV { get; set; }
            public bool ShouldSerializeInputCSV()
            {
                return CSV != null;
            }

            public class InputCSV
            {
                [XmlElement("RecordDelimiter", IsNullable = true)]
                public string RecordDelimiter { get; set; }
                public bool ShouldSerializeRecordDelimiter()
                {
                    return !string.IsNullOrEmpty(RecordDelimiter);
                }

                [XmlElement("FieldDelimiter", IsNullable = true)]
                public string FieldDelimiter { get; set; }
                public bool ShouldSerializeFieldDelimiter()
                {
                    return !string.IsNullOrEmpty(FieldDelimiter);
                }

                [XmlElement("QuoteCharacter", IsNullable = true)]
                public string QuoteCharacter { get; set; }
                public bool ShouldSerializeQuoteCharacter()
                {
                    return !string.IsNullOrEmpty(QuoteCharacter);
                }

                [XmlElement("CommentCharacter", IsNullable = true)]
                public string CommentCharacter { get; set; }
                public bool ShouldSerializeCommentCharacter()
                {
                    return !string.IsNullOrEmpty(CommentCharacter);
                }

                [XmlElement("Range", IsNullable = true)]
                public string Range { get; set; }
                public bool ShouldSerializeRange()
                {
                    return !string.IsNullOrEmpty(Range);
                }

                [XmlElement("FileHeaderInfo", IsNullable = true)]
                public FileHeaderInfo? FileHeaderInfo { get; set; }
                public bool ShouldSerializeFileHeaderInfo()
                {
                    return FileHeaderInfo != null;
                }

                [XmlElement("AllowQuotedRecordDelimiter", IsNullable = true)]
                public bool? AllowQuotedRecordDelimiter { get; set; }
                public bool ShouldSerializeAllowQuotedRecordDelimiter()
                {
                    return AllowQuotedRecordDelimiter != null;
                }
            }

            [XmlElement("JSON")]
            public InputJSON JSON { get; set; }
            public bool ShouldSerializeJSON()
            {
                return JSON != null;
            }

            public class InputJSON
            {
                [XmlElement("Type")]
                public JSONType Type { get; set; }

                [XmlElement("ParseJsonNumberAsString", IsNullable = true)]
                public bool? ParseJsonNumberAsString { get; set; }
                public bool ShouldSerializeParseJsonNumberAsString()
                {
                    return ParseJsonNumberAsString != null;
                }

                [XmlElement("Range", IsNullable = true)]
                public string Range { get; set; }
                public bool ShouldSerializeRange()
                {
                    return !string.IsNullOrEmpty(Range);
                }
            }
        }

        public class OutputFormatModel
        {
            [XmlElement("CSV")]
            public OutputCSV CSV { get; set; }
            public bool ShouldSerializeCSV()
            {
                return CSV != null;
            }

            public class OutputCSV
            {
                [XmlElement("RecordDelimiter", IsNullable = true)]
                public string RecordDelimiter { get; set; }
                public bool ShouldSerializeRecordDelimiter()
                {
                    return !string.IsNullOrEmpty(RecordDelimiter);
                }

                [XmlElement("FieldDelimiter", IsNullable = true)]
                public string FieldDelimiter { get; set; }
                public bool ShouldSerializeFieldDelimiter()
                {
                    return !string.IsNullOrEmpty(FieldDelimiter);
                }
            }

            [XmlElement("KeepAllColumns", IsNullable = true)]
            public bool? KeepAllColumns { get; set; }
            public bool ShouldSerializeKeepAllColumns()
            {
                return KeepAllColumns != null;
            }

            [XmlElement("OutputHeader", IsNullable = true)]
            public bool? OutputHeader { get; set; }
            public bool ShouldSerializeOutputHeader()
            {
                return OutputHeader != null;
            }

            [XmlElement("JSON")]
            public OutputJSON JSON { get; set; }
            public bool ShouldSerializeJSON()
            {
                return JSON != null;
            }

            public class OutputJSON
            {
                [XmlElement("RecordDelimiter")]
                public string RecordDelimiter { get; set; }
            }

            [XmlElement("EnablePayloadCrc", IsNullable = true)]
            public bool? EnablePayloadCrc { get; set; }
            public bool ShouldSerializeEnablePayloadCrc()
            {
                return EnablePayloadCrc != null;
            }

            [XmlElement("OutputRawData", IsNullable = true)]
            public bool? OutputRawData { get; set; }
            public bool ShouldSerializeOutputRawData()
            {
                return OutputRawData != null;
            }
        }

        public class OptionsModel
        {
            [XmlElement("SkipPartialDataRecord", IsNullable = true)]
            public bool? SkipPartialDataRecord { get; set; }
            public bool ShouldSerializeSkipPartialDataRecord()
            {
                return SkipPartialDataRecord != null;
            }

            [XmlElement("MaxSkippedRecordsAllowed", IsNullable = true)]
            public int? MaxSkippedRecordsAllowed { get; set; }
            public bool ShouldSerializeMaxSkippedRecordsAllowed()
            {
                return MaxSkippedRecordsAllowed != null;
            }
        }
    }
}
