
namespace Aliyun.OSS
{
    public abstract class CreateSelectObjectMetaInputFormat
    {
        /// <summary>
        /// Specifies the compression type of the object. Valid values: None, GZIP.
        /// </summary>
        public CompressionType CompressionType { get; set; }
    }

    /// <summary>
    /// Describes how a CSV-formatted input object is formatted.
    /// </summary>
    public class CreateSelectObjectMetaCSVInputFormat : CreateSelectObjectMetaInputFormat
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
    }

    /// <summary>
    /// Describes how a JSON-formatted input object is formatted.
    /// </summary>
    public class CreateSelectObjectMetaJSONInputFormat : CreateSelectObjectMetaInputFormat
    {
        /// <summary>
        /// Specifies the type of the input JSON object. Valid values: DOCUMENT, LINES.
        /// </summary>
        public JSONType Type { get; set; }
    }

    /// <summary>
    /// The request class of the operation to create the meta of select object.
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
        /// Gets or sets the input format
        /// </summary>
        public CreateSelectObjectMetaInputFormat InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the overwrite flag
        /// </summary>
        public bool OverwriteIfExists { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="CreateSelectObjectMetaRequest" />.
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

