
namespace Aliyun.OSS
{
    public abstract class SelectObjectInputFormat
    {
        /// <summary>
        /// Specifies the compression type of the object. Valid values: None, GZIP.
        /// </summary>
        public CompressionType CompressionType { get; set; }
    }

    /// <summary>
    /// Describes how a CSV-formatted input object is formatted.
    /// </summary>
    public class SelectObjectCSVInputFormat : SelectObjectInputFormat
    {
        /// <summary>
        /// Specifies the value used to separate individual records.
        /// </summary>
        public string RecordDelimiter { get; set; }

        /// <summary>
        /// Specifies the value used to separate individual fields in a record.
        /// </summary>
        public string FieldDelimiter { get; set; }

        /// <summary>
        /// Specifies the value used for escaping where the field delimiter is part of the value.
        /// </summary>
        public string QuoteCharacter { get; set; }

        /// <summary>
        /// Specifies the comment character used in the object.
        /// </summary>
        public string CommentCharacter { get; set; }

        /// <summary>
        /// Specifies the query range. The following two query methods are supported: 
        /// Query by row: line-range=start-end 
        /// Query by split: split-range=start-end 
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        /// Specifies the first line of input. Valid values: None, Ignore, Use.
        /// </summary>
        public FileHeaderInfo? FileHeaderInfo { get; set; }

        /// <summary>
        /// Specifies whether the CSV object contains line breaks in quotation marks (")
        /// </summary>
        public bool? AllowQuotedRecordDelimiter { get; set; }
    }

    /// <summary>
    /// Describes how a JSON-formatted input object is formatted.
    /// </summary>
    public class SelectObjectJSONInputFormat : SelectObjectInputFormat
    {
        /// <summary>
        /// Specifies the type of the input JSON object. Valid values: DOCUMENT, LINES.
        /// </summary>
        public JSONType Type { get; set; }

        /// <summary>
        /// Specifies the query range. The following two query methods are supported: 
        /// Query by row: line-range=start-end 
        /// Query by split: split-range=start-end 
        /// This parameter can only be used when the JSON Type is LINES.
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        /// Specifies whether to parse integers and floating-point numbers in a JSON object into strings.
        /// </summary>
        public bool? ParseJsonNumberAsString { get; set; }
    }

    public abstract class SelectObjectOutputFormat
    {
        /// <summary>
        /// Specifies whether to output in raw format. Default value is fasle.
        /// </summary>
        public bool? OutputRawData { get; set; }

        /// <summary>
        /// Specifies whether to include  a CRC-32 value for each frame. 
        /// This value is used to verify frame data.
        /// </summary>
        public bool? EnablePayloadCrc { get; set; }
    }

    /// <summary>
    /// Describes how CSV-formatted results are formatted.
    /// </summary>
    public class SelectObjectCSVOutputFormat : SelectObjectOutputFormat
    {
        /// <summary>
        /// Specifies the value used to separate individual records.
        /// </summary>
        public string RecordDelimiter { get; set; }

        /// <summary>
        /// Specifies the value used to separate individual fields in a record.
        /// </summary>
        public string FieldDelimiter { get; set; }

        /// <summary>
        /// Specifies whether to include that all columns in the CSV object.
        /// </summary>
        public bool? KeepAllColumns { get; set; }

        /// <summary>
        /// Specifies whether to include the header information of the CSV object in the beginning of the returned.
        /// </summary>
        public bool? OutputHeader { get; set; }
    }

    /// <summary>
    /// Describes how JSON-formatted results are formatted.
    /// </summary>
    public class SelectObjectJSONOutputFormat : SelectObjectOutputFormat
    {
        /// <summary>
        /// Specifies the value used to separate individual records in the output.
        /// </summary>
        public string RecordDelimiter { get; set; }
    }

    public class SelectObjectOptions
    {
        /// <summary>
        /// Specifies whether to ignore rows without data.
        /// </summary>
        public bool? SkipPartialDataRecord { get; set; }

        /// <summary>
        /// Specifies the maximum allowed number of skipped rows.
        /// </summary>
        public int? MaxSkippedRecordsAllowed { get; set; }
    }

    /// <summary>
    /// The request class of the operation to select object.
    /// </summary>
    public class SelectObjectRequest
    {
        /// <summary>
        /// Gets the bucket name
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the object key.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets The SQL Expression.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Gets or sets the format of the data in the object that is being queried.
        /// </summary>
        public SelectObjectInputFormat InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the format of the data that you want the server to return in response.
        /// </summary>
        public SelectObjectOutputFormat OutputFormat { get; set; }

        /// <summary>
        /// Gets or sets the options when quering the data.
        /// </summary>
        public SelectObjectOptions Options { get; set; }

        /// <summary>
        /// Creates a new intance of <see cref="SelectObjectRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">key</param>
        public SelectObjectRequest(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
        }
    }
}

