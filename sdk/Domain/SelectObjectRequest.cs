using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.OSS.Domain;
using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    public class SelectObjectInputFormat
    {
        public CompressionType CompressionTypes { get; set; }

        public InputCSV CSVs { get; set; }

        public class InputCSV
        {
            public string RecordDelimiter { get; set; }

            public string FieldDelimiter { get; set; }

            public string QuoteCharacter { get; set; }

            public FileHeaderInfo? FileHeaderInfos { get; set; }

            public string CommentCharacter { get; set; }

            public string Range { get; set; }
        }

        public JSONInput JSONInputs { get; set; }

        public class JSONInput
        {
            public JsonType? Types { get; set; }

            public bool? ParseJsonNumberAsString { get; set; }

            public string Range { get; set; }
        }
    }

    public class SelectObjectOutputFormat
    {
        public OutputCSV CSVs { get; set; }

        public class OutputCSV
        {
            public string RecordDelimiter { get; set; }

            public string FieldDelimiter { get; set; }
        }

        public bool KeepAllColumns { get; set; }

        public bool OutputRawData { get; set; }

        public bool OutputHeader { get; set; }

        public bool EnablePayloadCrc { get; set; }

        public JSONOutput JSONOutputs { get; set; }

        public class JSONOutput
        {
            public string RecordDelimiter { get; set; }
        }
    }

    /// <summary>
    /// The request class of the operation to SelectObject.
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
        /// Gets or sets The SelectObjectRequest InputFormatType.
        /// </summary>
        public InputFormatTypes InputFormatType { get; set; }

        /// <summary>
        /// Gets or sets The SQL Expression.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Gets or sets The SelectObjectRequest InputFormats.
        /// </summary>
        public SelectObjectInputFormat InputFormats { get; set; }

        /// <summary>
        /// Gets or sets The line or split of Range .
        /// </summary>
        public bool lineRangeIsSet { get; set; }

        public long lineRangeStart { get; set; }

        public long lineRangeEnd { get; set; }

        public bool splitRangeIsSet { get; set; }

        public long splitRangeStart { get; set; }

        public long splitRangeEnd { get; set; }

        /// <summary>
        /// Gets or sets The SelectObjectRequest OutputFormats.
        /// </summary>
        public SelectObjectOutputFormat OutputFormats { get; set; }

        /// <summary>
        /// Gets or sets SkipPartialDataRecord.
        /// </summary>
        public bool SkipPartialDataRecord { get; set; }

        /// <summary>
        /// Gets or sets MaxSkippedRecordsAllowed.
        /// </summary>
        public long MaxSkippedRecordsAllowed { get; set; }

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

