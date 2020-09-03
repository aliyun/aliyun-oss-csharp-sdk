using System;
using System.Xml.Serialization;

namespace Aliyun.OSS.Model
{
    public class CreateSelectObjectMetaInputFormatModel
    {
        [XmlElement("CompressionType")]
        public CompressionType CompressionTypeInfo { get; set; }

        [XmlElement("CSV")]
        public CSVModel CSV { get; set; }

        public bool ShouldSerializeInputCSV()
        {
            return CSV != null;
        }

        public class CSVModel
        {
            [XmlElement("RecordDelimiter")]
            public string RecordDelimiter { get; set; }
            public bool ShouldSerializeRecordDelimiter()
            {
                return !string.IsNullOrEmpty(RecordDelimiter);
            }

            [XmlElement("FieldDelimiter")]
            public string FieldDelimiter { get; set; }
            public bool ShouldSerializeFieldDelimiter()
            {
                return !string.IsNullOrEmpty(FieldDelimiter);
            }

            [XmlElement("QuoteCharacter")]
            public string QuoteCharacter { get; set; }
            public bool ShouldSerializeQuoteCharacter()
            {
                return !string.IsNullOrEmpty(QuoteCharacter);
            }
        }

        [XmlElement("JSON")]
        public JSONModel JSON { get; set; }
        public bool ShouldSerializeJSONInputs()
        {
            return JSON != null;
        }

        public class JSONModel
        {
            [XmlElement("Type")]
            public JSONType Type { get; set; }
        }
    }

    [XmlRoot("CsvMetaRequest")]
    public class CsvMetaRequestModel
    {
        [XmlElement("InputSerialization")]
        public CreateSelectObjectMetaInputFormatModel InputFormat { get; set; }

        [XmlElement("OverwriteIfExists")]
        public bool OverwriteIfExists { get; set; }
    }

    [XmlRoot("JsonMetaRequest")]
    public class JsonMetaRequestModel
    {
        [XmlElement("InputSerialization")]
        public CreateSelectObjectMetaInputFormatModel InputFormat { get; set; }

        [XmlElement("OverwriteIfExists")]
        public bool OverwriteIfExists { get; set; }
    }
}
