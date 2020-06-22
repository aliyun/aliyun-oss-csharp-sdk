using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    public class CreateSelectObjectMetaRequestModel
    {
        public class InputFormat
        {
            [XmlElement("CompressionType")]
            public CompressionType? CompressionTypes { get; set; }
            public bool ShouldSerializeCompressionTypes()
            {
                return CompressionTypes != null;
            }

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
                public bool ShouldSerializeRecordDelimiter()
                {
                    return RecordDelimiter != null;
                }

                [XmlElement("FieldDelimiter")]
                public string FieldDelimiter { get; set; }
                public bool ShouldSerializeFieldDelimiter()
                {
                    return FieldDelimiter != null;
                }

                [XmlElement("QuoteCharacter")]
                public string QuoteCharacter { get; set; }
                public bool ShouldSerializeQuoteCharacter()
                {
                    return QuoteCharacter != null;
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
            }
        }
    }

    [XmlRoot("CsvMetaRequest")]
    public class CsvMetaRequestModel
    {
        [XmlElement("InputSerialization", IsNullable = true)]
        public CreateSelectObjectMetaRequestModel.InputFormat InputFormats { get; set; }

        public bool ShouldSerializeInputFormats()
        {
            return InputFormats != null;
        }

        [XmlElement("OverwriteIfExists")]
        public bool? OverwriteIfExists { get; set; }

        public bool ShouldSerializeOverwriteIfExists()
        {
            return OverwriteIfExists != null;
        }
    }

    [XmlRoot("JsonMetaRequest")]
    public class JsonMetaRequestModel
    {
        [XmlElement("InputSerialization", IsNullable = true)]
        public CreateSelectObjectMetaRequestModel.InputFormat InputFormats { get; set; }

        public bool ShouldSerializeInputFormats()
        {
            return InputFormats != null;
        }

        [XmlElement("OverwriteIfExists")]
        public bool? OverwriteIfExists { get; set; }

        public bool ShouldSerializeOverwriteIfExists()
        {
            return OverwriteIfExists != null;
        }
    }
}
