/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 */

using System;
using System.Xml.Serialization;
using Aliyun.OSS;
namespace Aliyun.OSS.Model
{
    [XmlRoot("SelectRequest")]
    public class SelectObjectRequestModel
    {
        [XmlElement("Expression")]
        public string Expression
        {
            get;
            set;
        }

        [XmlElement("ExpressionType")]
        public string ExpressionType
        {
            get { return "SQL"; }
        }

        [XmlElement("InputSerialization")]
        public InputSerializationModel InputSerialization
        {
            get;
            set;
        }

        [XmlElement("OutputSerialization")]
        public OutputSerializationModel OutputSerialization
        {
            get;
            set;
        }
    }

    public class InputSerializationModel
    {
        [XmlElement("Compression")]
        public SelectObjectRequest.CompressionType Compression
        {
            get;
            set;
        }

        [XmlElement("CSV")]
        public CsvInputFormat Csv
        {
            get;
            set;
        }

        [XmlElement("JSON")]
        public JsonInputFormat Json
        {
            get;
            set;
        }
    }

    public class OutputSerializationModel
    {
        [XmlElement("CSV")]
        public CsvOutputFormat Csv
        {
            get;
            set;
        }

        [XmlElement("JSON")]
        public JsonOutputFormat Json
        {
            get;
            set;
        }
    }

    public class CsvInputFormat
    {
        [XmlElement("FileHeaderInfo")]
        public SelectObjectRequest.HeaderInfo FileHeaderInfo
        {
            get;
            set;
        }

        [XmlElement("RecordDelimiter")]
        public string RecordDelimiter
        {
            get;
            set;
        }

        [XmlElement("FieldDelimiter")]
        public string FieldDelimiter
        {
            get;
            set;
        }

        [XmlElement("QuoteCharacter")]
        public string QuoteCharacter
        {
            get;
            set;
        }

        [XmlElement("QuoteEscapeCharacter")]
        public string QuoteEscapeCharacter
        {
            get;
            set;
        }

        [XmlElement("Comments")]
        public string Comments
        {
            get;
            set;
        }
    }

    public class JsonInputFormat
    {
        [XmlElement("Type")]
        public string Type
        {
            get;
            set;
        }
    }

    public class CsvOutputFormat
    {
        [XmlElement("QuoteFields")]
        public string QuoteFields
        {
            get;
            set;
        }

        [XmlElement("RecordDelimiter")]
        public string RecordDelimiter
        {
            get;
            set;
        }

        [XmlElement("FieldDelimiter")]
        public string FieldDelimiter
        {
            get;
            set;
        }

        [XmlElement("QuoteCharacter")]
        public string QuoteCharacter
        {
            get;
            set;
        }

        [XmlElement("QuoteEscapeCharacter")]
        public string QuoteEscapeCharacter
        {
            get;
            set;
        }
    }

    public class JsonOutputFormat
    {
        [XmlElement("RecordDelimiter")]
        public string RecordDelimiter
        {
            get;
            set;
        }
    }
}
