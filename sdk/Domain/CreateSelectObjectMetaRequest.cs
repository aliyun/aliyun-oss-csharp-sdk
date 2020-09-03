using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.OSS.Domain;
using Aliyun.OSS.Model;

namespace Aliyun.OSS
{
    public class SelectObjectMetaInputFormat
    {
        public CompressionType? CompressionTypes { get; set; }

        public InputCSV CSVs { get; set; }

        public class InputCSV
        {
            public string RecordDelimiter { get; set; }

            public string FieldDelimiter { get; set; }

            public string QuoteCharacter { get; set; }
        }

        public JSONInput JSONInputs { get; set; }

        public class JSONInput
        {
            public JsonType? Types { get; set; }
        }
    }
    /// <summary>
    /// The request class of the operation to CreateSelectObjectMeta.
    /// </summary>
    public class CreateSelectObjectMetaRequest
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
        /// Gets or sets the InputFormatType of CreateSelectObjectMetaRequest.
        /// </summary>
        public InputFormatTypes InputFormatType { get; set; }

        /// <summary>
        /// Gets or sets the InputFormats of CreateSelectObjectMetaRequest.
        /// </summary>
        public SelectObjectMetaInputFormat InputFormats { get; set; }

        public bool OverwriteIfExists { get; set; }

        /// <summary>
        /// Creates a new intance of <see cref="CreateSelectObjectMetaRequest" />.
        /// </summary>
        /// <param name="bucketName">bucket name</param>
        /// <param name="key">key</param>
        public CreateSelectObjectMetaRequest(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
        }

    }
}

