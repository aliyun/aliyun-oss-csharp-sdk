/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    [XmlRoot("SelectRequest")]
    public class SelectObjectRequestModel
    {
        [XmlElement("Expression")]
        public string Expression { get; set; }

        [XmlElement("InputSerialization", IsNullable = true)]
        public InputFormat InputFormats { get; set; }

        public bool ShouldSerializeInputFormats()
        {
            return InputFormats != null;
        }

        public class InputFormat
        {
            [XmlElement("CompressionType")]
            public CompressionType CompressionTypes { get; set; }

            [XmlElement("CSV")]
            public InputCSV CSVs { get; set; }
            public bool ShouldSerializeInputCSV()
            {
                return CSVs != null;
            }

            public class InputCSV
            {
                [XmlElement("RecordDelimiter")]
                public string RecordDelimiter { get; set; }

                [XmlElement("FieldDelimiter")]
                public string FieldDelimiter { get; set; }

                [XmlElement("QuoteCharacter")]
                public string QuoteCharacter { get; set; }

                [XmlElement("FileHeaderInfo")]
                public FileHeaderInfo? FileHeaderInfos { get; set; }

                public bool ShouldSerializeFileHeaderInfos()
                {
                    return FileHeaderInfos != null;
                }

                [XmlElement("CommentCharacter")]
                public string CommentCharacter { get; set; }

                public bool ShouldSerializeCommentCharacter()
                {
                    return CommentCharacter != null;
                }

                [XmlElement("Range")]
                public string Range { get; set; }

                public bool ShouldSerializeRange()
                {
                    return Range != null;
                }
            }

            [XmlElement("JSON")]
            public JSONInput JSONInputs { get; set; }
            public bool ShouldSerializeJSONInputs()
            {
                return JSONInputs != null;
            }

            public class JSONInput
            {
                [XmlElement("Type")]
                public JsonType? Types { get; set; }
                public bool ShouldSerializeTypes()
                {
                    return Types != null;
                }

                [XmlElement("ParseJsonNumberAsString")]
                public bool? ParseJsonNumberAsString { get; set; }
                public bool ShouldSerializeParseJsonNumberAsString()
                {
                    return ParseJsonNumberAsString != null;
                }

                [XmlElement("Range")]
                public string Range { get; set; }

                public bool ShouldSerializeRange()
                {
                    return Range != null;
                }
            }
        }

        [XmlElement("OutputSerialization", IsNullable = true)]
        public OutputFormat OutputFormats { get; set; }

        public bool ShouldSerializeOutputFormats()
        {
            return OutputFormats != null;
        }

        public class OutputFormat
        {
            [XmlElement("CSV")]
            public OutputCSV CSVs { get; set; }
            public bool ShouldSerializeCSVs()
            {
                return CSVs != null;
            }

            public class OutputCSV
            {
                [XmlElement("RecordDelimiter")]
                public string RecordDelimiter { get; set; }

                [XmlElement("FieldDelimiter")]
                public string FieldDelimiter { get; set; }
            }

            [XmlElement("KeepAllColumns")]
            public bool KeepAllColumns { get; set; }

            [XmlElement("OutputRawData")]
            public bool OutputRawData { get; set; }

            [XmlElement("OutputHeader")]
            public bool OutputHeader { get; set; }

            [XmlElement("EnablePayloadCrc")]
            public bool EnablePayloadCrc { get; set; }

            [XmlElement("JSON")]
            public JSONOutput JSONOutputs { get; set; }
            public bool ShouldSerializeJSONOutputs()
            {
                return JSONOutputs != null;
            }

            public class JSONOutput
            {
                [XmlElement("RecordDelimiter")]
                public string RecordDelimiter { get; set; }
            }
        }

        [XmlElement("Options")]
        public Options Option { get; set; }

        public class Options
        {
            [XmlElement("SkipPartialDataRecord")]
            public bool SkipPartialDataRecord { get; set; }

            [XmlElement("MaxSkippedRecordsAllowed")]
            public long MaxSkippedRecordsAllowed { get; set; }
        }
    }
}
